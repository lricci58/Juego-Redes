using UnityEngine;

public class ControladorCamara : MonoBehaviour
{
    public float velocidadMovimientoCam = 500f;
    public float factorZoom = 400f;
    

    private Camera cam;
    private float finZoom;
    private float velocidadZoom = 10f;
    private float minZoom = 200f;
    private float maxZoom = 450f;

    void Start()
    {
        cam = Camera.main;
        finZoom = cam.orthographicSize;
    }

    void Update()
    {
        MoverCamara();
        ZoomCamara();
    }

    void MoverCamara()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey("w"))
        {
            pos.y += velocidadMovimientoCam * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            pos.y -= velocidadMovimientoCam * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            pos.x -= velocidadMovimientoCam * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            pos.x += velocidadMovimientoCam * Time.deltaTime;
        }

        transform.position = pos;
    }

    void ZoomCamara()
    {
        // obtiene la "cantidad de scroll" que se hizo
        float CantidadScroll = Input.GetAxis("Mouse ScrollWheel");

        // calcula la cantidad de zoom a la que debe mover la camara
        finZoom -= CantidadScroll * factorZoom;
        // limita el zoom a los valores ingresados
        finZoom = Mathf.Clamp(finZoom, minZoom, maxZoom);
        // hace el movimiento de la camara en la velocidad indicada
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, finZoom, Time.deltaTime * velocidadZoom);
    }
}