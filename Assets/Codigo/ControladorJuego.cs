using UnityEngine;

public class ControladorJuego : MonoBehaviour
{
    public static ControladorJuego instancia = null;
    public ControladorMapa mapa;
    
    // en un futuro, la lista de prefabs de unidades se añadiran al mapa, no al juego
    public Unidad objUnidad;
    private Grilla grilla;

    private Vector3 posMundo;
    private bool unidadSeleccionada;
    private bool unidadEstaMoviendo;

    void Awake()
    {
        if (instancia == null)
            instancia = this;
        else if (instancia != this)
            Destroy(gameObject);

        mapa = GetComponent<ControladorMapa>();

        objUnidad.GetComponent<Unidad>();

        // @TODO: obtener estos parametros del objeto mapa
        grilla = new Grilla(16, 12, 128f, new Vector3(-128f*8,-128*6));

        Vector3 posMundo = grilla.ObtenerPosMundo(7, 5);
        objUnidad.SetPosicion(posMundo);
        
        IniciarJuego();
    }

    void IniciarJuego()
    {
        mapa.setearEscena();
    }

    void Update()
    {
        unidadSeleccionada = objUnidad.EstaSeleccionada();

        if(!objUnidad.EstaMoviendo())
            if (unidadSeleccionada)
                SeleccionarTile();

        if (objUnidad.EstaMoviendo())
            objUnidad.Mover(posMundo);
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
                posMundo = grilla.ObtenerPosMundo(xTile, yTile);
                objUnidad.DebeMover(posMundo);
            }
        }
    }
}
