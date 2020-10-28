using UnityEngine;
using System.Collections.Generic;
using Mirror;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Pais : MonoBehaviour
{
    [SerializeField] private string[] limitrofes;

    [SerializeField] private GameObject paisAtacante;
    [SerializeField] private GameObject paisDefensor;

    [SerializeField] private GameObject menu;

    [SerializeField] private GameObject defender;
    [SerializeField] private GameObject atacar;

    [NonSerialized] public List<int> garrison;

    private void OnMouseDown()
    {
        if (MapManager.instancia.miTurno != MapManager.instancia.turnoActual) { return; }
        if (EventSystem.current.IsPointerOverGameObject()) { return; }
        if (!GameManager.instance.misPaises.Contains(gameObject.name)) { return; }

        // si el pais seleccionado es limitrofe, es un ataque
        if (tag == "Bordering")
        {
            // @TODO: poner cartel de ataque y eleccion de tropas y despues animacion antes de cargar escena
            
            if (menu.activeSelf)
            {
                menu.SetActive(false);
                atacar.SetActive(false);
            }
            else
            {
                menu.SetActive(true);
                atacar.SetActive(true);
                paisAtacante.GetComponent<Text>().text = GameObject.FindGameObjectWithTag("Selected").name;
                paisDefensor.GetComponent<Text>().text = this.name;
            }
        }
        // en caso de ser un pais normal, lo selecciona
        else
        {
            // al seleccionar el pais le avisa a los clientes
            ConnectionManager.instance.CmdCountryWasSelected(gameObject.name, limitrofes);
        }
    }
}


