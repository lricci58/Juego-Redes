using System.Collections.Generic;
using UnityEngine;

public class ControladorJuego : MonoBehaviour
{
    public static ControladorJuego instancia = null;
    public ControladorMapa mapa;
    

    private Grilla grilla;
    private List<Unidad> ejercito;
    
    private Vector3 posMundo;
    private bool clickeoEnGrilla;

    void Awake()
    {
        // se asegura que solo exista una instancia del controlador de juego
        if (instancia == null)
            instancia = this;
        else if (instancia != this)
            Destroy(gameObject);

        // @TODO: obtener estos parametros del objeto mapa
        grilla = new Grilla(16, 12, 128f, new Vector3(-128f * 8, -128 * 6));

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

    void Update()
    {
        // @TODO: spawnear la unidad en el tile seleccionado

        // comprueba si la unidad se selecciono
        bool estaSeleccionada = ejercito[0].EstaSeleccionada();

        if (estaSeleccionada)
            // comprueba si se hizo click en un tile
            TileSeleccionado();

        if (clickeoEnGrilla && estaSeleccionada)
            ejercito[0].DebeMover(posMundo);

        // el metodo mueve a la unidad si esta debe hacerlo
        ejercito[0].Mover(posMundo);
    }

    void TileSeleccionado()
    {
        // detecta cualquier click del mouse
        if (Input.GetMouseButtonDown(0))
        {
            // obtiene la posicion del mouse dentro del juego
            Vector3 mousePosMundo = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // detecta el click sobre algun tile y devuelve su posicion
            clickeoEnGrilla = grilla.DetectarClick(mousePosMundo, out int xTile, out int yTile);
            
            if (clickeoEnGrilla)
                // obtiene la posicion del tile en el mundo
                posMundo = grilla.ObtenerPosMundo(xTile, yTile);
        }
    }
}
