using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class Pais : MonoBehaviour
{
    [Header("Country Data")]
    [SerializeField] private string[] borderingCountries;
    [SerializeField] private int coinsPerTurn = 0;
    [NonSerialized] public List<int> countryGarrison = new List<int>();
    [NonSerialized] public Color originalCountryColor;

    [SerializeField] private GameObject countryGarrisonPanel = null;
    [SerializeField] private GameObject otherCountryGarrisonPanel = null;

    private void OnMouseDown()
    {
        if (MapManager.instancia.miTurno != MapManager.instancia.turnoActual) { return; }
        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        // comprueba si el pais clickeado es limitrofe (es un ataque)
        if (tag == "Bordering")
        {
            string atacante = GameObject.FindGameObjectWithTag("Selected").name;
            string atacado = gameObject.name;

            // avisa al jugador atacado
            ConnectionManager.instance.CmdPlayerAttacked(atacado, atacante);
        }
        // comprueba si el pais sea del jugador
        else if (GameManager.instance.misPaises.Contains(gameObject.name))
        {
            // busca por un pais seleccionado
            GameObject selectedCountry = GameObject.FindGameObjectWithTag("Selected");
            GameObject countryPanel = null;

            // si no hay pais seleccionado, se selecciona el pais
            if (!selectedCountry)
            {
                countryPanel = countryGarrisonPanel;

                // si el pais no tiene unidades no puede ser seleccionado
                if (countryGarrison.Count > 0) 
                {
                    List<string> limitrofesAtacables = new List<string>();
                    foreach (string limitrofe in borderingCountries)
                    {
                        // si el pais limitrofe es propio, no lo añade
                        if (GameManager.instance.misPaises.Contains(limitrofe)) { continue; }

                        limitrofesAtacables.Add(limitrofe);
                    }

                    // al seleccionar el pais le avisa a los clientes
                    ConnectionManager.instance.CmdCountryWasSelected(gameObject.name, limitrofesAtacables.ToArray());
                }
            }
            // si lo hay, es que se esta seleccionando un limitrofe para pasar unidades
            else if (borderingCountries.ToList().Contains(selectedCountry.name))
                countryPanel = otherCountryGarrisonPanel;

            if (!countryPanel) { return; }

            countryPanel.SetActive(true);
            countryPanel.GetComponent<UnitsPanelScript>().SetPanelHeader("Guarnicion de " + gameObject.name);
            countryPanel.GetComponent<UnitsPanelScript>().UpdateUnitsPanel(countryGarrison);
        }
    }

    public void Unselect()
    {
        countryGarrisonPanel.SetActive(false);
        otherCountryGarrisonPanel.SetActive(false);
    }

    public void ChangeOriginalColor(Color nuevoColor)
    {
        originalCountryColor = nuevoColor;
        ChangeColorToOriginal();
    }

    public void ChangeColorToOriginal() => GetComponent<SpriteRenderer>().color = originalCountryColor;
}


