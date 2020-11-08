using UnityEngine;
using UnityEngine.UI;

public class BattleUI_Manager : MonoBehaviour
{
    [SerializeField] private GameObject deploymentPanel;
    [SerializeField] private GameObject readyButton;
    [SerializeField] private GameObject endTurnButton;
    [SerializeField] private GameObject waitingText;

    public void ShowDeploymentPanel(bool state)
    {
        if (deploymentPanel.activeSelf == state) { return; }

        deploymentPanel.SetActive(state);

        UpdateDeploymentPanel();
    }

    public void UpdateDeploymentPanel()
    {
        deploymentPanel.GetComponent<UnitsPanelScript>().UpdateUnitsPanel(GameManager.instance.unitsToBattle);
    }

    public void ShowReadyButton(bool state)
    {
        if (readyButton.activeSelf == state) { return; }

        readyButton.SetActive(state);
    }

    public void ShowEndTurnButton(bool state)
    {
        if (endTurnButton.activeSelf == state) { return; }

        endTurnButton.SetActive(state);
    }

    public void ShowWaitingText(bool state)
    {
        if (waitingText.activeSelf == state) { return; }

        waitingText.SetActive(state);
    }

    public void PlayerIsReady() => ConnectionManager.instance.CmdEndedDeployFase(); // ConnectionManager.instance.CmdReadyUp();

    public void EndTurn()
    {
        // resetea las unidades luego de terminar el turno
        foreach (UnitScript unit in BattleManager.instance.army)
            unit.ResetUnitsInArmy();

        ConnectionManager.instance.CmdEndTurn(GameManager.instance.playerBattleSide);
    }
}
