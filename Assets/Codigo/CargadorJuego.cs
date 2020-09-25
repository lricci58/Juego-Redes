using UnityEngine;

public class CargadorJuego : MonoBehaviour
{
    public ControladorJuego juego;

    void Awake()
    {
        if (ControladorJuego.instancia == null)
            Instantiate(juego);
    }
}
