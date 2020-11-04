using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountryGarrisonPanel : MonoBehaviour
{
    [Header("PanelUI")]
    [SerializeField] private Text headerText = null;
    [SerializeField] private Text[] ammountOfUnits;

    public void ShowCountryPanel(string countryName, List<int> unitList)
    {
        headerText.text = "Guanicion de " + countryName;

        // busca la cantidad de cada tipo de soldado en las reservas
        int[] unitsCounter = new int[5];
        foreach (int unitType in unitList)
            unitsCounter[unitType]++;

        // actualiza los textos de los botones con la cantidad de cada tipo de soldado
        for (int i = 0; i < ammountOfUnits.GetLength(0); i++)
            ammountOfUnits[i].text = unitsCounter[i].ToString();
    }
}
