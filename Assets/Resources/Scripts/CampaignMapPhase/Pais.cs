using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class Pais : MonoBehaviour
{
    [SerializeField] private string[] limitrofes;

    [NonSerialized] public List<int> garrison;

    [NonSerialized] public Color colorOriginal;

    [SerializeField] private int oroPorTurno;
    



    private void OnMouseDown()
    {
        if (MapManager.instancia.miTurno != MapManager.instancia.turnoActual) { return; }
        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        // si el pais seleccionado es limitrofe, es un ataque
        if (tag == "Bordering")
        {
            string atacante = GameObject.FindGameObjectWithTag("Selected").name;
            string atacado = gameObject.name;

            // deselecciona al pais seleccionado y sus limitrofes
            MapManager.instancia.HayPaisSeleccionado();

            // abre el menu de ataque
            MapManager.instancia.DesplegarMenuAtaque(null, null, atacante, atacado, 1);

            // avisa al jugador atacado
            ConnectionManager.instance.CmdPlayerAttacked(atacado, atacante);
        }
        // en caso de ser un pais sin seleccionar o limitrofe...
        else
        {
            // comprueba que el pais  sea del jugador
            if (!GameManager.instance.misPaises.Contains(gameObject.name)) { return; }

            List<string> limitrofesAtacables = new List<string>();
            foreach (string limitrofe in limitrofes)
            {
                // si el pais limitrofe es propio, no lo añade
                if (GameManager.instance.misPaises.Contains(limitrofe)) { continue; }
                
                limitrofesAtacables.Add(limitrofe);
            }

            // al seleccionar el pais le avisa a los clientes
            ConnectionManager.instance.CmdCountryWasSelected(gameObject.name, limitrofesAtacables.ToArray());
        }
    }
    public void CambiarColorOriginal(Color nuevoColor)
    {
        colorOriginal = nuevoColor;
        CambiarAOriginal();
    }
    public void CambiarAOriginal()
    {
       GetComponent<SpriteRenderer>().color = colorOriginal;
    }

}




