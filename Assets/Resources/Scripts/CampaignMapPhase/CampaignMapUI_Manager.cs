using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CampaignMapUI_Manager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject generalMapUI = null;
    [SerializeField] private GameObject attackMenuPanel = null;

    [Header("Attack Menu")]
    [SerializeField] private Image playerImage = null;
    [SerializeField] private Image enemyImage = null;
    [SerializeField] private Text playerCountry = null;
    [SerializeField] private Text enemyCountry = null;

    [Header("UI Buttons")]
    [SerializeField] private Button endTurnButton = null;
    [SerializeField] private Button cancelButton = null;
    [SerializeField] private GameObject attackerButton = null;
    [SerializeField] private GameObject defenderButton = null;

    [Header("Turn UI")]
    [SerializeField] private Image player1Image = null;
    [SerializeField] private Image player2Image = null;
    [SerializeField] private Image player3Image = null;
    [SerializeField] private Image player4Image = null;

    [SerializeField] private Image player1Color = null;
    [SerializeField] private Image player2Color = null;
    [SerializeField] private Image player3Color = null;
    [SerializeField] private Image player4Color = null;

    [Header("General UI")]
    [SerializeField] private Text coins = null;
    [SerializeField] private Text playerNameText = null;

    public void SetPlayerNameText(string playerName) => playerNameText.text = playerName;

    public void BuyUnit(int unitType)
    {
        int playerCoins = int.Parse(coins.text);
        
        // obtiene los precios de las unidades segun su tipo
        int unitPrice = 0;
        switch (unitType)
        {
            case 0:
                unitPrice = 185;
                break;
            case 1:
                unitPrice = 225;
                break;
            case 2:
                unitPrice = 385;
                break;
        }

        if ((playerCoins - unitPrice) < 0) { return; }

        playerCoins -= unitPrice;
        coins.text = playerCoins.ToString();

        GameManager.instance.playerReserveUnits.Add(unitType);
        ReserveUnitScritp.instance.ResetReserveList();
    }

    public void ChangeImageInTurnFrame(List<Image> playerImages, List<Image> playerColors)
    {
        player1Image.sprite = playerImages[0].sprite;
        player2Image.sprite = playerImages[1].sprite;
        player3Image.sprite = playerImages[2].sprite;
        player4Image.sprite = playerImages[3].sprite;

        player1Color.color = playerColors[0].color;
        player2Color.color = playerColors[1].color;
        player3Color.color = playerColors[2].color;
        player4Color.color = playerColors[3].color;

        if (player3Image == null) { player3Image.gameObject.SetActive(false); }
        if (player4Image == null) { player4Image.gameObject.SetActive(false); }
        if (player3Color == null) { player3Color.gameObject.SetActive(false); }
        if (player4Color == null) { player4Color.gameObject.SetActive(false); }
    }

    public void ShowEndTurnButton(bool state)
    {
        if (endTurnButton.gameObject.activeSelf != state)
            endTurnButton.gameObject.SetActive(state);
    }

    public void CancelAttack()
    {
        ConnectionManager.instance.CmdPlayerStoppedAttacking();
    }

    public void EndTurn()
    {
        // revisar si se merece tarjeta y darsela
        Debug.Log("here");

        ConnectionManager.instance.CmdEndTurn(GameManager.instance.playerBattleSide);
    }

    public void ShowAttackMenu(Sprite playerSprite, Sprite enemySprite, string playerCounrtyName, string enemyCounrtyName)
    {
        ShowEndTurnButton(false);
        attackMenuPanel.SetActive(true);

        // setea las imagenes y nombres de los paises en cuestion
        playerImage.sprite = playerSprite;
        enemyImage.sprite = enemySprite;
        playerCountry.text = playerCounrtyName;
        enemyCountry.text = enemyCounrtyName;
    }

    public void HideAttackMenu()
    {
        ShowEndTurnButton(true);
        attackMenuPanel.SetActive(false);
    }

    public void ShowReadyButton(int playerType, bool state)
    {
        GameObject buttonType = null;

        if(playerType == 0)
            buttonType = attackerButton;
        else if (playerType == 1)
            buttonType = defenderButton;

        if (buttonType == null) { return; }

        if (buttonType.activeSelf == state) { return; }

        buttonType.SetActive(state);
    }

    public void ShowCancelButton(int playerType, bool state)
    {
        if (playerType != 1) { return; }

        if (cancelButton.gameObject.activeSelf == state) { return; }

        cancelButton.gameObject.SetActive(state);
    }
}
