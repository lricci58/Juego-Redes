using System.Collections.Generic;
using System.Net;
using UnityEditor.PackageManager;
using UnityEngine;

public class Juego : MonoBehaviour
{
    public static Juego instancia = null;
    public ControladorMapa mapa;
    
    // en un futuro, la lista de prefabs de unidades se añadiran al mapa, no al juego
    public Unidad objUnidad;

    private List<Unidad> ejercito1;
    private List<Unidad> ejercito2;
    private Grilla grilla;

    private bool unidadSeleccionada;

    void Awake()
    {
        if (instancia == null)
            instancia = this;
        else if (instancia != this)
            Destroy(gameObject);

        mapa = GetComponent<ControladorMapa>();

        ejercito1 = new List<Unidad>();
        ejercito1.Add(objUnidad.GetComponent<Unidad>());

        // @TODO: obtener estos parametros del objeto mapa
        grilla = new Grilla(16, 12, 128f, new Vector3(-128f*8,-128*6));

        Vector3 posMundo = grilla.ObtenerPosMundo(8, 8);
        ejercito1[0].SetPosicion(posMundo);
        IniciarJuego();
    }

    void IniciarJuego()
    {
        mapa.setearEscena();
    }

    void Update()
    {
        // @TODO: 
        // calcular el tile en el que se clickeo segun la posicion del mouse al hacer click
        // Input.mousePosition;
        // calcular el tile de la unidad actual segun la posicion de la misma
        // calcular la distancia hacia el tile en 'x' y en 'y'

        unidadSeleccionada = ejercito1[0].EstaSeleccionada();

        if (unidadSeleccionada)
            SeleccionarTile();

        // ejercito1[0].Mover(0, 0);
    }

    void SeleccionarTile()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            // obtiene la posicion del mouse dentro del juego
            Vector3 mousePosMundo = Camera.main.ScreenToWorldPoint(mousePos);

            int xTile, yTile;
            // obtiene
            bool clickeoEnGrilla = grilla.DetectarClick(mousePosMundo, out xTile, out yTile);
            
            if (clickeoEnGrilla)
            {
                Vector3 posMundo = grilla.ObtenerPosMundo(xTile, yTile);
                
                print(posMundo);
                ejercito1[0].Mover(posMundo);
            }
        }
    }
}
