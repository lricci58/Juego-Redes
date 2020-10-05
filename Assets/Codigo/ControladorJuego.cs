using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorJuego : MonoBehaviour
{
    public static ControladorJuego instancia = null;

    public int[] listaUnidades;

    void Awake()
    {
        // se asegura que solo exista una instancia del controlador de juego
        if (instancia == null)
            instancia = this;
        else if (instancia != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
