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

    [SerializeField] private GameObject countryUIPanel = null;

    private void OnMouseDown()
    {
        if (MapManager.instancia.miTurno != MapManager.instancia.turnoActual) { return; }
        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        // comprueba si el pais clickeado es limitrofe (es un ataque)
        if (tag == "Bordering")
        {
            string atacante = GameObject.FindGameObjectWithTag("Selected").name;
            string atacado = gameObject.name;

            // deselecciona al pais seleccionado y sus limitrofes
            MapManager.instancia.HayPaisSeleccionado();

            // abre el menu de ataque en modo atacante
            MapManager.instancia.DesplegarMenuAtaque(null, null, atacante, atacado, 1);

            // avisa al jugador atacado
            ConnectionManager.instance.CmdPlayerAttacked(atacado, atacante);
        }
        // comprueba si el pais sea del jugador
        else if (GameManager.instance.misPaises.Contains(gameObject.name))
        {
            // actualiza y muestra el panel de unidades
            countryUIPanel.SetActive(true);
            countryUIPanel.GetComponent<CountryGarrisonPanel>().ShowCountryPanel(gameObject.name, countryGarrison);

            List<int> originList = null;
            GameObject selectedCountry = GameObject.FindGameObjectWithTag("Selected");

            // indica de donde a donde se transladan las unidades (de las reservas o la guarnicion del pais seleccionado)
            if (selectedCountry)
                originList = selectedCountry.GetComponent<Pais>().countryGarrison;
            else
                originList = GameManager.instance.playerReserveUnits;

            /* @NOTE: problemas con el sistema de seleccion: 
             * hay un solo panel de guarnicion. 
             * la forma de revisar si un boton fue seleccionado es ineficiente
             */

            if (originList != null)
                if (IsPlayerAddingUnitsToCountry(originList)) { return; }

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

    private bool IsPlayerAddingUnitsToCountry(List<int> originList)
    {
        for (int i = 1; i <= 5; i++)
        {
            GameObject boton = GameObject.Find("UnitFrameImage (" + i + ")");

            if (!boton) { continue; }

            // comprueba que haya algun boton seleccionado
            if (!boton.GetComponent<ReserveUnitButton>().selected) { continue; }

            int unitType = i - 1;

            // comprueba si hay unidades suficientes para agregar
            if(originList.Contains(unitType))
            {
                originList.Remove(unitType);
                countryGarrison.Add(unitType);
            }

            // actualiza el panel de unidades 
            countryUIPanel.GetComponent<CountryGarrisonPanel>().ShowCountryPanel(gameObject.name, countryGarrison);

            // esto se deberia hacer solo al cerrar la ventana
            boton.GetComponent<ReserveUnitButton>().UnselectReserveUnit();

            // actualiza la UI de las unidades de reserva
            ReserveUnitScritp.instance.ResetReserveList();

            return true;
        }

        return false;
    }

    public void Unselect() => countryUIPanel.SetActive(false);

    public void ChangeOriginalColor(Color nuevoColor)
    {
        originalCountryColor = nuevoColor;
        ChangeColorToOriginal();
    }

    public void ChangeColorToOriginal() => GetComponent<SpriteRenderer>().color = originalCountryColor;
}


