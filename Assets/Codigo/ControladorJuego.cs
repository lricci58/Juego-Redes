using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorJuego : MonoBehaviour
{
    public static ControladorJuego instancia = null;
    public ControladorBatalla batalla;

    public string[] unidadesEjercito;

    void Awake()
    {
        // se asegura que solo exista una instancia del controlador de juego
        if (instancia == null)
            instancia = this;
        else if (instancia != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        // @NOTE: esto solo debe hacerse en Update(), cuando comience una batalla
        if (ControladorBatalla.instancia == null)
            Instantiate(batalla);
    }

    //void Update()
    //{
        
    //}
}
