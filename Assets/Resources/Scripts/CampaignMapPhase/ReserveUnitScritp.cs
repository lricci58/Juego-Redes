using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReserveUnitScritp : MonoBehaviour
{
    public static ReserveUnitScritp instance = null;

    [SerializeField] private Text[] ammountOfUnits;

    void Start()
    {
        instance = this;
    }

    void OnEnable() => ResetReserveList();

    public void ResetReserveList()
    {
        // destruye todos los posibles botones existentes
        for (int i = 0; i < 3; i++)
        {
            GameObject buttonPrefab = GameObject.Find("UnitFrameImage(Clone)");
            if (buttonPrefab)
                Destroy(buttonPrefab);
        }

        List<int> unitList = GameManager.instance.playerReserveUnits;

        int paladinCounter = 0;
        int archerCounter = 0;
        int huskarlCounter = 0;

        foreach (int unitType in unitList)
        {
            switch (unitType)
            {
                case 0:
                    paladinCounter++;
                    break;
                case 1:
                    archerCounter++;
                    break;
                case 2:
                    huskarlCounter++;
                    break;
            }
        }

        ammountOfUnits[0].text = paladinCounter.ToString();
        ammountOfUnits[1].text = archerCounter.ToString();
        ammountOfUnits[2].text = huskarlCounter.ToString();
    }
}
