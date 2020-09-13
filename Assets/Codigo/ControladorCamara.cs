using UnityEngine;

public class ControladorCamara : MonoBehaviour
{
    public float VelocidadMovimientoCam = 500f;
    public float FactorZoom = 400f;
    

    private Camera Cam;
    private float FinZoom;
    private float VelocidadZoom = 10f;
    private float minZoom = 200f;
    private float maxZoom = 450f;

    void Start()
    {
        Cam = Camera.main;
        FinZoom = Cam.orthographicSize;
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
            pos.y += VelocidadMovimientoCam * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            pos.y -= VelocidadMovimientoCam * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            pos.x -= VelocidadMovimientoCam * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            pos.x += VelocidadMovimientoCam * Time.deltaTime;
        }

        transform.position = pos;
    }

    void ZoomCamara()
    {
        // obtiene la "cantidad de scroll" que se hizo
        float CantidadScroll = Input.GetAxis("Mouse ScrollWheel");

        // calcula la cantidad de zoom a la que debe mover la camara
        FinZoom -= CantidadScroll * FactorZoom;
        // limita el zoom a los valores ingresados
        FinZoom = Mathf.Clamp(FinZoom, minZoom, maxZoom);
        // hace el movimiento de la camara en la velocidad indicada
        Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, FinZoom, Time.deltaTime * VelocidadZoom);
    }
}