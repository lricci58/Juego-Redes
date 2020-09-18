using System.Collections.Generic;
using UnityEngine;

public class ControladorJuego : MonoBehaviour
{
    public static ControladorJuego instancia = null;
    public ControladorMapa mapa;
    public GameObject tileDisponible;

    private Grilla grilla;
    private List<Unidad> ejercito;
    private List<Obstaculo> obstaculos;
    private List<GameObject> listaTilesDisponibles;
    private Transform contenedorTiles;

    private Vector3 posMundo;
    private bool puedeMover = false;
    private bool seleccionandoTile = false;

    void Awake()
    {
        // se asegura que solo exista una instancia del controlador de juego
        if (instancia == null)
            instancia = this;
        else if (instancia != this)
            Destroy(gameObject);

        listaTilesDisponibles = new List<GameObject>();

        contenedorTiles = new GameObject("ContenedorDeTilesDisponibles").transform;

        // @TODO: obtener estos parametros del objeto mapa
        grilla = new Grilla(16, 12, 127f, new Vector3(-127f * 8, -127 * 6));

        // crea la lista de unidades
        ejercito = new List<Unidad>();

        // obtiene el script del mapa
        mapa = GetComponent<ControladorMapa>();
        IniciarJuego();
    }

    void IniciarJuego()
    {
        // este metodo deberia cargar el mapa, la grilla, los GameObjects, etc
        mapa.CrearEscenario();
    }

    public void AgregarUnidad(Unidad componenteScript)
    {
        ejercito.Add(componenteScript);
    }

    public void AgregarObstaculo(Obstaculo componenteScript)
    {
        obstaculos.Add(componenteScript);
    }

    void Update()
    {
        // @TODO: spawnear la unidad en el tile seleccionado

        // comprueba si la unidad se selecciono
        bool estaSeleccionada = ejercito[0].EstaSeleccionada();

        if (estaSeleccionada)
            // comprueba si se hizo click en un tile
            SeleccionarTile();

        if (puedeMover && estaSeleccionada)
            ejercito[0].DebeMover(posMundo);

        // el metodo mueve a la unidad si esta debe hacerlo
        ejercito[0].Mover(posMundo);
    }

    void SeleccionarTile()
    {
        // detecta cualquier click del mouse
        if (Input.GetMouseButtonDown(0))
        {
            // obtiene la posicion del mouse dentro del juego
            Vector3 posMouseMundo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // devuelve el tile en el que se hizo click
            bool clickEnGrilla = grilla.ObtenerPosGrilla(posMouseMundo, out int tileX, out int tileY);

            // obtiene el tile en el que esta la unidad
            Vector3 posUnidad = ejercito[0].GetPosicion();
            grilla.ObtenerPosGrilla(posUnidad, out int tileUnidadX, out int tileUnidadY);

            // crea la matriz de tiles disponibles para la unidad
            List<Vector2> tilesDisponibles = ejercito[0].DeterminarMovimiento(tileUnidadX, tileUnidadY);

            // comprueba que el tile clickeado este dentro de la matriz de disponibles
            bool clickEnTileValido = ejercito[0].ClickTileEsValido(tileX, tileY);

            if (!seleccionandoTile)
            {
                // instancia los tiles disponibles en las posiciones indicadas
                InstanciarTilesDisponibles(tilesDisponibles);

                seleccionandoTile = true;
            }

            if (clickEnGrilla && clickEnTileValido)
                puedeMover = true;
            else
                puedeMover = false;

            if (puedeMover)
            {
                // obtiene la posicion del tile en el mundo
                posMundo = grilla.ObtenerPosMundo(tileX, tileY);

                // destruye los objetos
                DestruirTilesDisponibles(tilesDisponibles);

                seleccionandoTile = false;
            }
        }
    }

    void InstanciarTilesDisponibles(List<Vector2> tilesDisponibles)
    {
        // instancia cada tile en su posicion
        foreach (Vector2 posTile in tilesDisponibles)
        {
            // obtiene la posicion de los tiles en el mundo (x, y)
            Vector3 posMundoTile = grilla.ObtenerPosMundo((int)posTile.x, (int)posTile.y);

            posMundoTile.x += 64;
            posMundoTile.y += 64;
            // setea la posicion 'z' para que sea igual a la del prefab
            posMundoTile.z = tileDisponible.transform.position.z;

            // instancia el prefab con la posicion indicada
            GameObject tileInstanciado = Instantiate(tileDisponible, posMundoTile, Quaternion.identity);
            listaTilesDisponibles.Add(tileInstanciado);
            // setea el padre de la instacia
            tileInstanciado.transform.SetParent(contenedorTiles);
        }
    }

    void DestruirTilesDisponibles(List<Vector2> tilesDisponibles)
    {
        foreach (GameObject tileDisponible in listaTilesDisponibles)
            Destroy(tileDisponible);

        tilesDisponibles.Clear();
        listaTilesDisponibles.Clear();
    }
}
