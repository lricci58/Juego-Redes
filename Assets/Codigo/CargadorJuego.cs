using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CargadorJuego : MonoBehaviour
{
    public ManagerJuego juego;

    void Awake()
    {
        // @TODO: comprobar que no este einstanciado antes de instanciar
        Instantiate(juego);
    }
}
