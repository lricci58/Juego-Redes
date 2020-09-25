using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControladorBatalla : MonoBehaviour
{
    public static ControladorBatalla instancia = null;

    [SerializeField] private CargadorMapa mapa;
    [SerializeField] private GameObject objetoCanvas;
    private ControladorUI canvas;

    private List<Unidad> ejercito;
    private List<Unidad> listaUnidadesDespliegue;
    private Vector3 posMundo;
    private Unidad unidadElegida;
    private Unidad unidadObjetivo;
    private bool seleccionandoTile = false;

    private bool despliegue = true;
    private int numeroUnidad = -1;

    void Awake()
    {
        if (instancia == null)
            instancia = this;
        else if (instancia != this)
            Destroy(gameObject);

        IniciarJuego();
    }

    private void IniciarJuego()
    {
        Instantiate(objetoCanvas);
        canvas = objetoCanvas.GetComponent<ControladorUI>();

        ejercito = new List<Unidad>();
        listaUnidadesDespliegue = new List<Unidad>();

        mapa = GetComponent<CargadorMapa>();
        mapa.CrearEscenario();
    }

    public void AgregarUnidad(Unidad componenteScript)
    {
        ejercito.Add(componenteScript);
        listaUnidadesDespliegue.Add(componenteScript);
        componenteScript.gameObject.SetActive(false);
    }

    void Update()
    {
        if (despliegue)
        {
            canvas.MostrarPanelDespliegue(true);
            if (numeroUnidad != -1)
            {
                DesplegarUnidad(listaUnidadesDespliegue[numeroUnidad]);
                
                despliegue = false;
                
                foreach (Unidad unidad in listaUnidadesDespliegue)
                    if (!unidad.gameObject.activeSelf)
                        despliegue = true;
            }
            else
                numeroUnidad = DeterminarUnidadDespliegue();
        }
        else
        {
            canvas.MostrarPanelDespliegue(false);
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
                if (!unidadElegida.EstaMoviendo())
                    SeleccionarTile(unidadElegida);

                // mueve la unidad si debe hacerlo
                unidadElegida.Mover(posMundo);
                if(unidadObjetivo != null)
                {
                    unidadElegida.Atacar(unidadObjetivo);
                    if (unidadObjetivo.EstaMuerta())
                    {    
                        // Destroy(unidadObjetivo.gameObject);
                        ejercito.Remove(unidadObjetivo);
                    }
                }   

                if (!unidadElegida.EstaSeleccionada() && !unidadElegida.EstaMoviendo())
                    unidadElegida = null;
            }
        }
    }

    private int DeterminarUnidadDespliegue()
    {
        List<GameObject> botonesUnidad = GameObject.FindGameObjectsWithTag("Boton").ToList();
        for (int i = 0; i < botonesUnidad.Count; i++)
        {
            try
            {
                if (botonesUnidad[i].GetComponent<BotonUnidad>().seleccionada)
                {
                    Destroy(botonesUnidad[i]);
                    return i;
                }
            }
            catch
            {
                return -1;
            }
        }

        return -1;
    }

    private void DesplegarUnidad(Unidad unidad)
    {
        if(ClickEnGrilla(out int tileX, out int tileY))
        {
            Vector3 posDespliegue = mapa.ObtenerPosMundo(tileX, tileY);
            unidad.Desplegar(posDespliegue);
            unidad.gameObject.SetActive(true);
            listaUnidadesDespliegue.Remove(unidad);
            numeroUnidad = -1;
        }
    }

    private void SeleccionarTile(Unidad unidad)
    {
        if (ClickEnGrilla(out int tileX, out int tileY))
        {
            // obtiene el tile(x, y) en el que esta la unidad
            mapa.ObtenerPosGrilla(unidad.ObtenerPosicion(), out int tileUnidadX, out int tileUnidadY);

            unidad.DeterminarRadioTiles(tileUnidadX, tileUnidadY);
            // obtiene los tiles de movimiento y ataque de una unidad
            unidad.CambiarTilesMovimiento(mapa.DeterminarTilesInvalidos(unidad.ObtenerTilesMovimiento(), tileUnidadX, tileUnidadY, unidad.ObtenerRadioMovimiento()));
            unidad.CambiarTilesAtaque(mapa.DeterminarTilesAtaque(unidad.ObtenerTilesMovimiento(), tileUnidadX, tileUnidadY));
            List<Vector2> tilesMovimiento = unidad.ObtenerTilesMovimiento();
            List<Vector2> tilesAtaque = unidad.ObtenerTilesAtaque();

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
                    foreach (Unidad posibleObjetivo in ejercito)
                    {
                        mapa.ObtenerPosGrilla(posibleObjetivo.ObtenerPosicion(), out int tileUnidadObjX, out int tileUnidadObjY);
                        if (tileX == tileUnidadObjX && tileY == tileUnidadObjY)
                        {
                            unidadObjetivo = posibleObjetivo;
                            unidad.ToggleAtaque(true);
                            debeAtacar = true;
                            break;
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
                    else
                        unidad.ToggleSeleccion(false);
                }
                else
                    unidad.ToggleSeleccion(false);

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
}
