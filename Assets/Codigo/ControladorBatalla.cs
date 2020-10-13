using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControladorBatalla : NetworkBehaviour
{
    public static ControladorBatalla instancia = null;

    public CargadorMapa mapa;
    [SerializeField] private GameObject objetoCanvas;
    private ControladorUI canvas;

    private List<Unidad> ejercito;
    private List<Unidad> listaUnidadesDespliegue;
    private List<Unidad> unidades;
    private Vector3 posMundo;
    private Unidad unidadElegida;
    private Unidad unidadObjetivo;
    private bool seleccionandoTile = false;

    private int numeroUnidad = -1;
    private bool despliegue = true;
    private bool todosListos = false;
    [SyncVar (hook = nameof(ActualizarDesplegados))] public int desplegados = 0;

    void Start()
    {
        instancia = this;

        ejercito = new List<Unidad>();
        listaUnidadesDespliegue = new List<Unidad>();
        // lista para todas las unidades, aliadas y enemigas
        unidades = new List<Unidad>();

        IniciarJuego();
    }

    private void IniciarJuego()
    {
        Instantiate(objetoCanvas);
        canvas = objetoCanvas.GetComponent<ControladorUI>();
        canvas.MostrarPanelDespliegue(true);

        mapa = GetComponent<CargadorMapa>();
        mapa.CrearEscenario();
    }

    public void AgregarUnidad(Unidad componenteScript)
    {
        

        if (!todosListos)
        {
            ejercito.Add(componenteScript);
            listaUnidadesDespliegue.Add(componenteScript);
            componenteScript.gameObject.SetActive(false);
        }
        // comprueba que el dueño del objeto sea el cliente local
        else
        {
            unidades.Add(componenteScript);
            ControladorConexion.instancia.CmdUnidadValida(componenteScript.GetComponent<NetworkIdentity>());
        }
    }

    public void AgregarUnidadAEjercito(Unidad componenteScript) => ejercito.Add(componenteScript);

    void Update()
    {
        if (!ControladorConexion.instancia.isLocalPlayer) { return; }
        if (!ControladorConexion.instancia.hasAuthority) { return; }

        FaseControlBatalla();

        FaseDespliegue();

        FaseFinDespliegue();
    }

    #region ControlJuego

    private void FaseControlBatalla()
    {
        // comprueba que estemos en la fase de batalla
        if (!todosListos) { return; }

        // @TODO: comprobar que sea su turno

        if (unidadElegida == null)
        {
            foreach (Unidad unidad in ejercito)
            {
                if (unidad.SeSelecciono())
                {
                    unidadElegida = unidad;
                    break;
                }
            }
        }

        if (unidadElegida != null)
        {
            if (!unidadElegida.EstaMoviendo()) { SeleccionarTile(unidadElegida); }

            // mueve la unidad si debe hacerlo
            unidadElegida.Mover(posMundo);

            // ejecuta el ataque si existe un objetivo
            if (unidadObjetivo != null)
            {
                unidadElegida.Atacar(unidadObjetivo);
                if (unidadObjetivo.EstaMuerta()) { ejercito.Remove(unidadObjetivo); }
            }

            // comprueba si la unidad deja de estar seleccionada
            if (!unidadElegida.EstaSeleccionada() && !unidadElegida.EstaMoviendo()) { unidadElegida = null; }
        }
    }

    private void FaseDespliegue()
    {
        // comprueba que estemos en la fase de despliegue
        if (!despliegue) { return; }

        if (numeroUnidad != -1)
        {
            DesplegarUnidad(listaUnidadesDespliegue[numeroUnidad]);

            despliegue = false;
            foreach (Unidad unidad in listaUnidadesDespliegue)
                if (!unidad.gameObject.activeSelf)
                    despliegue = true;

            if (!despliegue)
            {
                canvas.MostrarPanelDespliegue(false);
                ControladorConexion.instancia.CmdJugadorTerminoDespliegue();
            }
        }
        else
            numeroUnidad = DeterminarUnidadDespliegue();
    }

    private void FaseFinDespliegue()
    {
        // comprueba que los 2 jugadores hayan desplegado
        if (TodosDesplegaron() && !todosListos)
        {
            todosListos = true;

            listaUnidadesDespliegue.AddRange(ejercito);
            ejercito.Clear();

            for (int i = 0; i < listaUnidadesDespliegue.Count; i++)
            {
                Unidad unidad = listaUnidadesDespliegue[i];

                // re-spawnea (reemplaza) las unidades instanciadas pero en la red
                ControladorConexion.instancia.CmdSpawnObjeto(unidad.tipoUnidad, unidad.transform.position);
                // destruye las unidades que fueron reemplazadas
                Destroy(unidad.gameObject);
                listaUnidadesDespliegue.Remove(unidad);
            }

            // setea el contenedor padre de las unidades
            foreach (GameObject objetoUnidad in GameObject.FindGameObjectsWithTag("Unidad"))
                objetoUnidad.transform.SetParent(mapa.contenedorUnidades);
        }
    }

    #endregion

    #region Despliegue

    private int DeterminarUnidadDespliegue()
    {
        // obtiene botones activos (visibles)
        List<GameObject> botonesUnidad = GameObject.FindGameObjectsWithTag("Boton").ToList();

        for (int i = 0; i < botonesUnidad.Count; i++)
        {
            try
            {
                if (botonesUnidad[i].GetComponent<BotonUnidad>().seleccionada)
                {
                    // botonesUnidad[i].SetActive(false);
                    return i;
                }
            }
            catch (NullReferenceException)
            {
                return -1;
            }
        }

        return -1;
    }

    private void DesplegarUnidad(Unidad unidad)
    {
        // obtiene botones activos (visibles)
        List<GameObject> botonesUnidad = GameObject.FindGameObjectsWithTag("Boton").ToList();

        if (ClickEnGrilla(out int tileX, out int tileY))
        {
            // comprueba que el click no haya sido sobre otra unidad
            foreach (Unidad otraUnidad in ejercito)
            {
                mapa.ObtenerPosGrilla(otraUnidad.ObtenerPosicion(), out int otraUnidadTileX, out int otraUnidadTileY);

                // comprueba si la posicion de despleigue conincide con la de otra unidad ya desplegada
                if (otraUnidadTileX == tileX && otraUnidadTileY == tileY)
                {
                    // 'deselecciona' el boton
                    botonesUnidad[numeroUnidad].GetComponent<BotonUnidad>().Deseleccionar();
                    numeroUnidad = -1;

                    return;
                }
            }

            // oculta el boton
            botonesUnidad[numeroUnidad].SetActive(false);

            Vector3 posDespliegue = mapa.ObtenerPosMundo(tileX, tileY);
            unidad.Desplegar(posDespliegue);
            unidad.gameObject.SetActive(true);
            listaUnidadesDespliegue.Remove(unidad);
            numeroUnidad = -1;
        }
    }

    private void ActualizarDesplegados(int oldValue, int newValue) => desplegados = newValue;

    private bool TodosDesplegaron()
    {
        if (desplegados == 2)
            return true;
        else
            return false;
    }

    #endregion

    #region Control Unidades

    private void SeleccionarTile(Unidad unidad)
    {
        if (!ClickEnGrilla(out int tileX, out int tileY)) { return; }

        // obtiene el tile(x, y) en el que esta la unidad
        mapa.ObtenerPosGrilla(unidad.ObtenerPosicion(), out int tileUnidadX, out int tileUnidadY);

        unidad.DeterminarRadioTiles(tileUnidadX, tileUnidadY);
        // obtiene los tiles de movimiento y ataque de una unidad
        unidad.CambiarTilesMovimiento(mapa.DeterminarTilesInvalidos(unidad.ObtenerTilesMovimiento(), tileUnidadX, tileUnidadY, unidad.ObtenerRadioMovimiento()));
        unidad.CambiarTilesAtaque(mapa.DeterminarTilesAtaque(unidad.ObtenerTilesMovimiento(), tileUnidadX, tileUnidadY));
        List<Vector2> tilesMovimiento = unidad.ObtenerTilesMovimiento();
        List<Vector2> tilesAtaque = unidad.ObtenerTilesAtaque();

        // elimina los tiles de ataque en los que hay unidades aliadas
        //foreach (Unidad otraUnidad in ejercito)
        //{
        //    Vector2 posOtraUnidad = new Vector2(otraUnidad.ObtenerPosicion().x, otraUnidad.ObtenerPosicion().y);
        //    if (tilesAtaque.Contains(posOtraUnidad))
        //    {
        //        tilesAtaque.Remove(otraUnidad.ObtenerPosicion());
        //    }
        //}                

        // comprueba que el click no haya sido sobre el tile de la unidad
        if (tileX != tileUnidadX || tileY != tileUnidadY)
        {
            if (unidad.ClickEnTileMovimiento(tileX, tileY))
            {
                posMundo = mapa.ObtenerPosMundo(tileX, tileY);
                // determina la direccion de movimiento segun la posicion del tile destino
                unidad.DeterminarDireccionMovimiento(posMundo);
            }
            else if (unidad.ClickEnTileAtaque(tileX, tileY))
            {
                bool debeAtacar = false;
                foreach (Unidad posibleObjetivo in unidades)
                {
                    mapa.ObtenerPosGrilla(posibleObjetivo.ObtenerPosicion(), out int tileUnidadObjX, out int tileUnidadObjY);
                    if (tileX == tileUnidadObjX && tileY == tileUnidadObjY)
                    {
                        // comprueba que la unidad no este en el ejercito aliado
                        if (!ejercito.Contains(posibleObjetivo))
                        {

                            unidadObjetivo = posibleObjetivo;
                            unidad.AltAtaque(true);
                            debeAtacar = true;
                            break;
                        }
                    }
                }

                if (debeAtacar)
                {
                    unidad.DeterminarDireccionMovimiento(mapa.ObtenerPosMundo(tileX, tileY));

                    // asegura que al hacer click en un tile de ataque la unidad mueva al tile anterior
                    if (tileX > tileUnidadX)
                        tileX--;
                    else if (tileX < tileUnidadX)
                        tileX++;

                    if (tileY > tileUnidadY)
                        tileY--;
                    else if (tileY < tileUnidadY)
                        tileY++;

                    posMundo = mapa.ObtenerPosMundo(tileX, tileY);
                }
                else { unidad.AltSeleccion(false); }
            }
            else { unidad.AltSeleccion(false); }

            mapa.DestruirTiles();
            tilesMovimiento.Clear();
            tilesAtaque.Clear();
            seleccionandoTile = false;
        }
        else if (!seleccionandoTile)
        {
            // crea los tiles de movimiento
            mapa.InstanciarTilesMovimiento(tilesMovimiento);
            mapa.InstanciarTilesAtaque(tilesAtaque);
            seleccionandoTile = true;
        }
    }

    private bool ClickEnGrilla(out int x, out int y)
    {
        x = 0;
        y = 0;
        if (Input.GetMouseButtonDown(0))
        {
            // obtiene la posicion del mouse dentro del juego
            Vector3 posMouseMundo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return mapa.ObtenerPosGrilla(posMouseMundo, out x, out y);
        }

        return false;
    }

    #endregion
}