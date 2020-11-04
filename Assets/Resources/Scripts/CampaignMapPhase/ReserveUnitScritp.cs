using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReserveUnitScritp : MonoBehaviour
{
    public static ReserveUnitScritp instance = null;

    [SerializeField] private Text[] ammountOfUnits;

    void Start() => instance = this;

    void OnEnable() => ResetReserveList();

    public void ResetReserveList()
    {
        List<int> unitList = GameManager.instance.playerReserveUnits;

        // busca la cantidad de cada tipo de soldado en las reservas
        int[] unitsCounter = new int[5];
        foreach (int unitType in unitList)
            unitsCounter[unitType]++;

        // actualiza los textos de los botones con la cantidad de cada tipo de soldado
        for (int i = 0; i < ammountOfUnits.GetLength(0); i++)
            ammountOfUnits[i].text = unitsCounter[i].ToString();
    }

    public bool IsActive() => gameObject.activeSelf;
}
