using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class SeleccionMapa : MonoBehaviour
{
    [SerializeField] private int numUnidades;
    [SerializeField] private string[] limitrofes;

    private string battleSceneName = "BattleScene";

    private void OnMouseDown()
    {
        // si el pais ya esta seleccionado, no hace nada (para que no lo deseleccione)
        // if (tag == "Selected") { return; }

        // si el pais seleccionado es limitrofe, es un ataque
        if (tag == "Bordering")
        {
            // @TODO: poner cartel de ataque y eleccion de tropas y despues animacion antes de cargar escena

            SceneManager.LoadScene(battleSceneName);
        }
        // en caso de ser un pais normal, lo selecciona
        else
        {
            // al seleccionar el pais le avisa a los clientes
            ConnectionManager.instance.CmdCountryWasSelected(gameObject.name, limitrofes);
        }
    }
}


