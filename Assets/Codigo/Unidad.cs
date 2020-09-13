using UnityEngine;

public class Unidad : MonoBehaviour 
{
    public float tiempoMov = 200f;

    private Animator animador;

    public float vida = 0;
    public float armadura = 0;
    public float ataque = 0;
    public int radioMov = 4;

    public float offsetPosicionX = 70f;
    public float offsetPosicionY = 100f;
    private bool seleccionada = false;

    void Start()
    {
        // obtiene el componente Animator del objeto unidad
        animador = GetComponent<Animator>();
    }

    public void Mover(Vector3 posicion)
    {
        animador.SetBool("moviendo", true);
        Vector3 pos = transform.position;
        pos.x = posicion.x + offsetPosicionX;
        pos.y = posicion.y + offsetPosicionY;
        transform.position = pos;
        animador.SetBool("moviendo", false);
    }

    public bool EstaSeleccionada()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // detecta si se hizo click dentro del collider
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            if (hit)
            {
                seleccionada = !seleccionada;
            }
        }

        return seleccionada;
    }

    public void SetPosicion(Vector3 posMundo)
    {
        Vector3 pos = transform.position;
        pos.x = posMundo.x + offsetPosicionX;
        pos.y = posMundo.y + offsetPosicionY;
        transform.position = pos;
    }

    // crear metodos de ataque, ser golpeado, morir, etc
}
