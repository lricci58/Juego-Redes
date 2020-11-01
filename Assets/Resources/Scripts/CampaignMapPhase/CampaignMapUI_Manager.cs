using UnityEngine;
using UnityEngine.UI;

public class CampaignMapUI_Manager : MonoBehaviour
{
    private static GameObject cancelAttackButton;

    private static GameObject garrisonPanel;
    private static GameObject toBattlePanel;

    private static GameObject vsFramesPanel;
    private static GameObject playerIcon;
    private static GameObject enemyIcon;
    private static GameObject playerText;
    private static GameObject enemyText;
    private static GameObject attackerButton;
    private static GameObject defenderButton;

    private static GameObject endTurnButton;
    private static GameObject endPhaseButton;

    void Awake()
    {
        cancelAttackButton = GameObject.Find("CancelAttackButton");

        garrisonPanel = GameObject.Find("GarrisonPanel");
        toBattlePanel = GameObject.Find("ToBattlePanel");

        vsFramesPanel = GameObject.Find("VsFramesPanel");
        playerIcon = GameObject.Find("PlayerImage");
        enemyIcon = GameObject.Find("EnemyImage");
        playerText = GameObject.Find("PlayerCountryNameText");
        enemyText = GameObject.Find("EnemyCountryNameText");
        attackerButton = GameObject.Find("AttackerButton");
        defenderButton = GameObject.Find("DefenderButton");

        endTurnButton = GameObject.Find("EndTurnButton");
        endPhaseButton = GameObject.Find("EndPhaseButton");
    }

    public void ShowCancelAttackButton(bool state)
    {
        if (cancelAttackButton.activeSelf != state)
            cancelAttackButton.SetActive(state);
    }

    public void ChangePlayerImage(Sprite newImage)
    {
        if (newImage == null) { return; }
        
        playerIcon.GetComponent<Image>().sprite = newImage;
    }

    public void ChangeEnemyImage(Sprite newImage)
    {
        if (newImage == null) { return; }

        enemyIcon.GetComponent<Image>().sprite = newImage;
    }

    public void ChangePlayerCountry(string newText)
    {
        if (newText == null) { newText = "null"; }

        playerText.GetComponent<Text>().text = newText;
    }

    public void ChangeEnemyCountry(string newText)
    {
        if (newText == null) { newText = "null"; }

        enemyText.GetComponent<Text>().text = newText;
    }

    public void ShowGarrisonPanel(bool state)
    {
        if (garrisonPanel.activeSelf != state)
            garrisonPanel.SetActive(state);
    }

    public void ShowToBattlePanel(bool state)
    {
        if (toBattlePanel.activeSelf != state)
            toBattlePanel.SetActive(state);
    }

    public void ShowVsFramesPanel(bool state)
    {
        if (vsFramesPanel.activeSelf != state)
            vsFramesPanel.SetActive(state);
    }

    public void ShowAttackerButton(bool state)
    {
        if (attackerButton.activeSelf != state)
            attackerButton.SetActive(state);
    }

    public void ShowDefenderButton(bool state)
    {
        if (defenderButton.activeSelf != state)
            defenderButton.SetActive(state);
    }

    public void ShowEndTurnButton(bool state)
    {
        if (endTurnButton.activeSelf != state)
            endTurnButton.SetActive(state);
    }

    public void ShowEndPhaseButton(bool state)
    {
        if (endPhaseButton.activeSelf != state)
            endPhaseButton.SetActive(state);
    }
}
