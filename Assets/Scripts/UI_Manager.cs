using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    private static GameObject deploymentPanel;
    private static GameObject startBattleButton;
    private static GameObject waitingText;

    void Awake()
    {
        deploymentPanel = GameObject.Find("DeploymentPanel");
        startBattleButton = GameObject.Find("StartBattleButton");
        waitingText = GameObject.Find("TextoEspera");
    }

    public void ShowDeploymentPanel(bool state)
    {
        if(deploymentPanel.activeSelf != state)
            deploymentPanel.SetActive(state);
    }

    public void ShowStartBattleButton(bool state)
    {
        if (startBattleButton.activeSelf != state)
            startBattleButton.SetActive(state);
    }

    public void ShowWaitingText(bool state)
    {
        if (waitingText.activeSelf != state)
            waitingText.SetActive(state);
    }
}
