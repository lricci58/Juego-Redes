using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    private static GameObject deploymentPanel;

    void Awake()
    {
        deploymentPanel = GameObject.Find("DeploymentPanel");
    }

    public void ShowDeploymentPanel(bool state)
    {
        if(state != deploymentPanel.activeSelf)
            deploymentPanel.SetActive(state);
    }
}
