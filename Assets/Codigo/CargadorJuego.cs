using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CargadorJuego : MonoBehaviour
{
    public ControladorJuego juego;

    void Awake()
    {
        if (ControladorJuego.instancia == null)
            Instantiate(juego);
    }
}
