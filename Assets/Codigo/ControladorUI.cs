using UnityEngine;

public class ControladorUI : MonoBehaviour
{
    private static GameObject panelDespliegue;

    void Awake()
    {
        panelDespliegue = GameObject.Find("PanelDespliegue");
    }

    public void MostrarPanelDespliegue(bool estado)
    {
        if(estado != panelDespliegue.activeSelf)
            panelDespliegue.SetActive(estado);
    }
}
