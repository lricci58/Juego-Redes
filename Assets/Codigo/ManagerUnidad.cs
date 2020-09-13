using UnityEngine;

public class ManagerUnidad : MonoBehaviour 
{
    public float tiempoMov = 200f;

    private Animator animador;

    private float vida;
    private float armadura;
    private float ataque;

    private int radioMov;

    void Start()
    {
        // instancia al animador del componente que se esta usandi
        animador = GetComponent<Animator>();
    }

    public void Mover(int distX, int distY)
    {
        while (distX > 0)
        {
            transform.Translate(tiempoMov * Time.deltaTime, 0, 0);
            if (true)
            {
                distX--;
            }
        }
    }

    // crear metodos de ataque, ser golpeado, morir, etc
}
