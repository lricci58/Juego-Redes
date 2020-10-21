using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class SeleccionMapa : MonoBehaviour
{
    [SerializeField] private int numUnidades;
    [SerializeField] private string[] limitrofes;

    private void OnMouseDown()
    {
        // si el pais seleccionado es limitrofe, es un ataque
        if (tag.Equals("Bordering"))
        {
            // @TODO: poner cartel de ataque y eleccion de tropas y despues animacion antes de cargar escena

            // setea el jugador como atacante (1)
            GameManager.instance.playerBattleSide = 1;

            ConnectionManager.instance.CmdPlayerAttacked();
        }
        // en caso de ser un pais normal, lo selecciona
        else
        {
            // al seleccionar el pais le avisa a los clientes
            ConnectionManager.instance.CmdCountryWasSelected(gameObject.name, limitrofes);
        }
    }
}


