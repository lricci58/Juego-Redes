using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class ControladorJuego : MonoBehaviour
{
    public static ControladorJuego instancia = null;
    [SerializeField] private ControladorMapa mapa;

    private List<Unidad> ejercito;
    private List<Obstaculo> obstaculos;

    private Vector3 posMundo;

    void Awake()
    {
        // se asegura que solo exista una instancia del controlador de juego
        if (instancia == null)
            instancia = this;
        else if (instancia != this)
            Destroy(gameObject);

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
        // @TODO: spawnear la unidad en el tile seleccionado (crear zonas de despliegue)

        Unidad unidad = ejercito[0];

        // @TODO: hacer un check para todas las unidades del ejercito, y ver si se selecciona alguna
        // en cuyo caso, esa sera la utilizada para 'SeleccionarTile()' y 'Mover()'

        // check si la unidad se selecciono
        if (unidad.EstaSeleccionada())
            // para seleccionar en un tile disponible para mover
            SeleccionarTile(unidad);

        // mueve a la unidad si esta debe hacerlo
        unidad.Mover(posMundo);
    }

    void SeleccionarTile(Unidad unidad)
    {
        if (Input.GetMouseButtonDown(0))
        {
            // obtiene la posicion del mouse dentro del juego
            Vector3 posMouseMundo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // check si el click fue sobre la grilla, y sobre que tile
            bool clickEnGrilla = mapa.ObtenerPosGrilla(posMouseMundo, out int tileX, out int tileY);

            if (clickEnGrilla)
            {
                // obtiene el tile (x, y) en el que esta la unidad
                mapa.ObtenerPosGrilla(unidad.ObtenerPosicion(), out int tileUnidadX, out int tileUnidadY);

                // obtiene la lista de las posiciones de los tiles de movimiento alrededor de la unidad
                List<Vector2> posicionesTiles = unidad.DeterminarRadioTiles(tileUnidadX, tileUnidadY);

                // check si alguna de las posiciones colisiona con un obstaculo, en cuyo caso se remueve la posicion
                posicionesTiles = mapa.CheckColisiones(posicionesTiles);

                // @TODO: solo permitir tilesDisponibles que esten dentro del rango de movimiento del jugador && no atraviese un obstaculos

                // comprueba que el click no haya sido sobre el tile de la unidad
                if (tileX != tileUnidadX || tileY != tileUnidadY)
                {
                    // check si hizo click sobre un tile de movimiento
                    if (unidad.ClickEnTileMovimiento(tileX, tileY))
                    {
                        // obtiene la posicion del tile a la que mover
                        posMundo = mapa.ObtenerPosMundo(tileX, tileY);
                        unidad.DeterminarDireccionMovimiento(posMundo);

                        // destruye los tiles de radio de movimiento
                        mapa.DestruirTilesDeMovimiento(posicionesTiles);
                    }
                    else
                    {
                        mapa.DestruirTilesDeMovimiento(posicionesTiles);
                        unidad.DesSeleccionar();
                    }
                }
                else
                {
                    mapa.InstanciarTilesDeMovimiento(posicionesTiles);
                }
            }
        }
    }
}
