using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance = null;

    [SerializeField] private float movementVelocity = 500f;
    [SerializeField] private float zoomFactor = 400f;
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float heighLimit = 210;
    [SerializeField] private float widthLimit = 310;

    private Camera cam;
    private float cameraHeight;
    private float cameraWidth;

    private float zoomEnd;
    private float zoomMin = 200f;
    private float zoomMax = 450f;

    private Vector2 mapLimit;
    private float mapBorder = 10f;

    void Start()
    {
        instance = this;

        cam = Camera.main;
        zoomEnd = cam.orthographicSize;

        mapLimit = new Vector2(widthLimit, heighLimit);
    }

    void Update()
    {
        CameraMovementManager();
        CameraZoomManager();
    }

    private void CameraMovementManager()
    {
        Vector3 cameraPosition = transform.position;

        // al mantener tecla
        if (Input.GetKey("w"))
        {
            // modifica el vector temporal que contiene la posicion de la camara
            cameraPosition.y += movementVelocity * Time.deltaTime;
        }
        else if (Input.GetKey("s"))
        {
            cameraPosition.y -= movementVelocity * Time.deltaTime;
        }

        if (Input.GetKey("a"))
        {
            cameraPosition.x -= movementVelocity * Time.deltaTime;
        }
        else if (Input.GetKey("d"))
        {
            cameraPosition.x += movementVelocity * Time.deltaTime;
        }

        // @TODO: corregir el clamp al hacer zoom, este debe depender directamente del tamaño de la camara
        cameraPosition.x = Mathf.Clamp(cameraPosition.x, -mapLimit.x + 100f, mapLimit.x - 100f);
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, -mapLimit.y - 100f, mapLimit.y + 100f);

        // actualiza la posicion de la camara
        transform.position = cameraPosition;
    }

    private void CameraZoomManager()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // calcula la cantidad de zoom
        zoomEnd -= scroll * zoomFactor;
        // limita el zoom
        zoomEnd = Mathf.Clamp(zoomEnd, zoomMin, zoomMax);

        // ejecuta el movimiento suave
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomEnd, zoomSpeed * Time.deltaTime);

        //cameraHeight = 2f * cam.orthographicSize;
        //cameraWidth = cameraHeight * cam.aspect;
    }

    public void SmoothZoomTo(float scroll) {}

    public void SmoothMovementTo(Vector2 position) {}
}