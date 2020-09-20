using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class ControladorJuego : MonoBehaviour
{
    public static ControladorJuego instancia = null;
    [SerializeField] private ControladorMapa mapa;

    private List<Unidad> ejercito;

    private Vector3 posMundo;

    void Awake()
    {
        // se asegura que solo exista una instancia del controlador de juego
        if (instancia == null)
            instancia = this;
        else if (instancia != this)
            Destroy(gameObject);

        ejercito = new List<Unidad>();

        mapa = GetComponent<ControladorMapa>();
        IniciarJuego();
    }

    void IniciarJuego()
    {
        // carga el mapa, la grilla, los obstaculos, las unidades, etc
        mapa.CrearEscenario();
    }

    public void AgregarUnidad(Unidad componenteScript)
    {
        ejercito.Add(componenteScript);
    }

    void Update()
    {
        // @TODO: crear y manejar las zonas de despliegue
        // @TODO: crear el bucle para los turnos

        Unidad unidad = ejercito[0];

        if (unidad.EstaSeleccionada())
            SeleccionarTile(unidad);

        // mueve la unidad si debe hacerlo
        unidad.Mover(posMundo);
    }

    void SeleccionarTile(Unidad unidad)
    {
        if (Input.GetMouseButtonDown(0))
        {
            // obtiene la posicion del mouse dentro del juego
            Vector3 posMouseMundo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            bool clickEnGrilla = mapa.ObtenerPosGrilla(posMouseMundo, out int tileX, out int tileY);

            if (clickEnGrilla)
            {
                // obtiene el tile(x, y) en el que esta la unidad
                mapa.ObtenerPosGrilla(unidad.ObtenerPosicion(), out int tileUnidadX, out int tileUnidadY);

                unidad.DeterminarRadioTiles(tileUnidadX, tileUnidadY);
                unidad.CambiarRadioTiles(mapa.DeterminarTilesInvalidos(unidad.ObtenerRadioTiles(), tileUnidadX, tileUnidadY, unidad.ObtenerPasosTotales()));
                List<Vector2> radioTiles = unidad.ObtenerRadioTiles();

                // comprueba que el click no haya sido sobre el tile de la unidad
                if (tileX != tileUnidadX || tileY != tileUnidadY)
                {
                    if (unidad.ClickEnTileMovimiento(tileX, tileY))
                    {
                        // obtiene la posicion del tile a la que mover
                        posMundo = mapa.ObtenerPosMundo(tileX, tileY);
                        unidad.DeterminarDireccionMovimiento(posMundo);

                        mapa.DestruirTilesDeMovimiento(radioTiles);
                    }
                    else
                    {
                        mapa.DestruirTilesDeMovimiento(radioTiles);
                        unidad.ToggleSeleccion(false);
                    }
                }
                // @NOTE: solo deberia instanciar, la primera vez que se hace click sobre la unidad
                else
                    mapa.InstanciarTilesDeMovimiento(radioTiles);
            }
        }
    }
}
