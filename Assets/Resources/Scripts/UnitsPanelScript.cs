using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitsPanelScript : MonoBehaviour
{
    [SerializeField] private GameObject otherPanel;

    [SerializeField] private Text panelHeader;
    [SerializeField] private Text[] ammountOfUnits;

    public void SetPanelHeader(string newHeaderText) => panelHeader.text = newHeaderText;

    public void UpdateUnitsPanel(List<int> unitList)
    {
        // busca la cantidad de cada tipo de soldado en las reservas
        int[] unitsCounter = new int[5];
        foreach (int unitType in unitList)
            unitsCounter[unitType]++;

        // actualiza los textos de los botones con la cantidad de cada tipo de soldado
        for (int i = 0; i < ammountOfUnits.GetLength(0); i++)
            ammountOfUnits[i].text = unitsCounter[i].ToString();
    }

    public void PassUnitToOtherPanel(int unitType)
    {
        // recibe las cantidad de unidades en este panel del tipo que se quiere pasar
        int newAmmountOfUnits = int.Parse(ammountOfUnits[unitType].text);

        // comprueba que haya unidades para pasar
        if (newAmmountOfUnits <= 0 || otherPanel.activeSelf == false) { return; }

        // actualiza las listas de unidades segun el tipo de panel (reserva, guarnicion, al combate)
        if (panelHeader.text == "Unidades al combate")
        {
            GameManager.instance.unitsToBattle.Remove(unitType);
            MapManager.instancia.CheckIfPlayerCanGoToBattle();
        }
        else
        {
            // desde "guarnicion de" en adelante esta el nombre del pais
            string selectedCountryName = panelHeader.text.Substring(14);
            GameObject selectedCountry = GameObject.Find(selectedCountryName);

            if (selectedCountry)
                selectedCountry.GetComponent<Pais>().countryGarrison.Remove(unitType);
            else
                GameManager.instance.playerReserveUnits.Remove(unitType);
        }

        // actualiza el texto del panel
        newAmmountOfUnits--;
        ammountOfUnits[unitType].text = newAmmountOfUnits.ToString();

        // actualiza el otro panel
        otherPanel.GetComponent<UnitsPanelScript>().RecieveUnitFromOtherPanel(unitType);
    }

    public void RecieveUnitFromOtherPanel(int unitType)
    {
        int newAmmountOfUnits = int.Parse(ammountOfUnits[unitType].text) + 1;
        ammountOfUnits[unitType].text = newAmmountOfUnits.ToString();

        if (panelHeader.text == "Unidades al combate")
        {
            GameManager.instance.unitsToBattle.Add(unitType);
            MapManager.instancia.CheckIfPlayerCanGoToBattle();
        }
        else
        {
            string otherCountryName = panelHeader.text.Substring(14);
            GameObject otherCountry = GameObject.Find(otherCountryName);

            if (otherCountry)
                otherCountry.GetComponent<Pais>().countryGarrison.Add(unitType);
            else
                GameManager.instance.playerReserveUnits.Add(unitType);
        }
    }

    public void SelectUnitForDeployment(int unitType)
    {
        // vacia la lista de arriba a abajo
        BattleManager.instance.deployUnitType = unitType;
    }
}
