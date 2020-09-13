using System.Collections.Generic;
using UnityEngine;

public class ManagerJuego : MonoBehaviour
{
    public ManagerMapa mapa;

    // private List<UnidadManager> ejercito;
    private ManagerUnidad infanteriaHacha;

    void Awake()
    {
        // @TODO: guardar elementos que, de otra forma, se eliminarian entre escenas

        mapa = GetComponent<ManagerMapa>();
        infanteriaHacha = GetComponent<ManagerUnidad>();
        
        IniciarJuego();
    }

    void IniciarJuego()
    {
        mapa.setearEscena();
    }

    void Update()
    {
        // @TODO: 
        // calcular el tile en el que se clickeo segun la posicion del mouse al hacer click
        // Input.mousePosition;
        // calcular el tile de la unidad actual segun la posicion de la misma
        // calcular la distancia hacia el tile en 'x' y en 'y'

        int distX = 0;
        int distY = 0;

        ClickMouse();

        if (distX != 0 || distY != 0)
            infanteriaHacha.Mover(distX, distY);
    }

    void ClickMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos;
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);


        }
    }
}
