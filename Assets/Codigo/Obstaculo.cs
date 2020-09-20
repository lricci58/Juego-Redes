using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstaculo : MonoBehaviour
{
    void Start()
    {
        ControladorJuego.instancia.AgregarObstaculo(this);
    }

    void Update()
    {
        
    }
}
