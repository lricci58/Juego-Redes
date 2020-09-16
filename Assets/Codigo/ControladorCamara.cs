using UnityEngine;

public class ControladorCamara : MonoBehaviour
{
    // @TODO: revisar el asset RTS Camera, puede que cumpla con todas las features de este script y mas...

    [SerializeField] private float velocidadMovimientoCam = 500f;
    [SerializeField] private float factorZoom = 400f;
    [SerializeField] private float velocidadZoom = 100f;
    [SerializeField] private float limAlto = 210;
    [SerializeField] private float limAncho = 310;

    private Camera cam;
    private float finZoom;
    private float minZoom = 200f;
    private float maxZoom = 450f;

    private Vector2 limMapa;
    private float bordeMapa = 10f;

    void Start()
    {
        cam = Camera.main;
        finZoom = cam.orthographicSize;

        limMapa = new Vector2(limAncho, limAlto);
    }

    void Update()
    {      
        MoverCamara();
        ZoomCamara();
    }

    void MoverCamara()
    {
        Vector3 posicionCamara = transform.position;

        // al mantener tecla
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - bordeMapa)
        {
            // modifica el vector temporal que contiene la posicion de la camara
            posicionCamara.y += velocidadMovimientoCam * Time.deltaTime;
        }
        else if (Input.GetKey("s") || Input.mousePosition.y <= bordeMapa)
        {
            posicionCamara.y -= velocidadMovimientoCam * Time.deltaTime;
        }

        if (Input.GetKey("a") || Input.mousePosition.x <= bordeMapa)
        {
            posicionCamara.x -= velocidadMovimientoCam * Time.deltaTime;
        }
        else if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - bordeMapa)
        {
            posicionCamara.x += velocidadMovimientoCam * Time.deltaTime;
        }

        // @TODO: corregir el clamp al hacer zoom, este debe depender directamente del tamaño de la camara
        posicionCamara.x = Mathf.Clamp(posicionCamara.x, -limMapa.x, limMapa.x);
        posicionCamara.y = Mathf.Clamp(posicionCamara.y, -limMapa.y, limMapa.y + 150f);

        // actualiza la posicion de la camara
        transform.position = posicionCamara;
    }

    void ZoomCamara()
    {
        // obtiene la "cantidad de scroll" que se hizo
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // calcula la cantidad de zoom
        finZoom -= scroll * factorZoom;
        // limita el zoom
        finZoom = Mathf.Clamp(finZoom, minZoom, maxZoom);
        // ejecuta el movimiento suave
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, finZoom, velocidadZoom * Time.deltaTime);
    }
}