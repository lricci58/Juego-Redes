using UnityEngine;

public class UnitButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject buttonObject;
    [SerializeField] private Sprite[] unitImages;
    [SerializeField] private Transform buttonContaniner;

    private UnitButtonScript button;

    void Start()
    {
        button = buttonObject.GetComponent<UnitButtonScript>();
        AddUnitButton();
    }

    private void AddUnitButton()
    {
        // lista que guarda los tipos de cada unidad en el ejercito
        int[] unitList = GameManager.instance.unitList;
        for (int i = 0; i < unitList.GetLength(0); i++)
        {
            int unitType = unitList[i];
            string unitName = "Not Found";

            if (unitType == 0)
                unitName = "Paladin";

            else if (unitType == 1)
                unitName = "Arquero";

            else if (unitType == 2)
                unitName = "Huscarle";

            else if (unitType == 3)
                unitName = "Amazona";

            else if (unitType == 4)
                unitName = "Esqueleto Guerrero";

            else if (unitType == 5)
                unitName = "Piromante";

            button.CrearBoton(unitName, unitImages[unitType]);

            GameObject instance = Instantiate(buttonObject);
            instance.transform.SetParent(buttonContaniner);
        }
    }
}
