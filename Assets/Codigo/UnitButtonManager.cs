using UnityEngine;

public class UnitButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject buttonObject;
    [SerializeField] private Sprite[] unitImages;
    [SerializeField] private Transform buttonContaniner;

    private UnitButton button;

    void Start()
    {
        button = buttonObject.GetComponent<UnitButton>();
        AddUnitButton();
    }

    private void AddUnitButton()
    {
        int[] unitList = GameManager.instance.unitList;

        for (int i = 0; i < unitList.GetLength(0); i++)
        {
            int unitType = unitList[i];

            if (unitType == 0)
                button.CrearBoton("Infantería con Hacha", unitImages[unitType]);

            else if (unitType == 1)
                button.CrearBoton("Infantería con Espada", unitImages[unitType]);

            GameObject instance = Instantiate(buttonObject);
            instance.transform.SetParent(buttonContaniner);
        }
    }
}
