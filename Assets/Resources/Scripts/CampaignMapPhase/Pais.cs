using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class Pais : MonoBehaviour
{
    [SerializeField] private string[] limitrofes;

    [NonSerialized] public List<int> garrison = new List<int>();
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

            if (IsPlayerAddingUnitsToCountry()) { return; }

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

    private bool IsPlayerAddingUnitsToCountry()
    {
        for (int i = 1; i <= 3; i++)
        {
            GameObject boton = GameObject.Find("UnitFrameImage ("+i+")");

            if (!boton) { continue; }

            // comprueba que haya algun boton seleccionado
            if (!boton.GetComponent<ReserveUnitButton>().selected) { continue; }

            // agrega la unidad especifica a la lista del pais
            garrison.Add(i - 1);
            GameManager.instance.playerReserveUnits.Remove(i - 1);

            boton.GetComponent<ReserveUnitButton>().UnselectReserveUnit();

            // actualiza la UI de las unidades de reserva
            ReserveUnitScritp.instance.ResetReserveList();

            return true;
        }

        return false;
    }

    public void ChangeOriginalColor(Color nuevoColor)
    {
        colorOriginal = nuevoColor;
        ChangeColorToOriginal();
    }

    public void ChangeColorToOriginal() => GetComponent<SpriteRenderer>().color = colorOriginal;
}


