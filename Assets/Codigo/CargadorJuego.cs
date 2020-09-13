using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CargadorJuego : MonoBehaviour
{
    public Juego juego;

    void Awake()
    {
        if (Juego.instancia == null)
            Instantiate(juego);
    }
}
