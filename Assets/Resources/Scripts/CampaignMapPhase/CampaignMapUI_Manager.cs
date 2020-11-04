using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CampaignMapUI_Manager : MonoBehaviour
{
    [Header("General UI Objects")]
    [SerializeField] private Text coins = null;
    [SerializeField] private Text playerNameText = null;
    [SerializeField] private Button endTurnButton = null;

    [Header("Attack Menu UI Objects")]
    [SerializeField] private GameObject attackMenuPanel = null;
    [SerializeField] private Button cancelAttackButton = null;
    [SerializeField] private GameObject attackerButton = null;
    [SerializeField] private GameObject defenderButton = null;
    [SerializeField] private Image playerImage = null;
    [SerializeField] private Image enemyImage = null;
    [SerializeField] private Text playerCountry = null;
    [SerializeField] private Text enemyCountry = null;

    [Header("Turn UI Objects")]
    [SerializeField] private Image[] playerImagesInTurnPanel = null;
    [SerializeField] private Image[] playerColorsInTurnPanel = null;

    [Header("BuyUnitsPanel")]
    [SerializeField] private Text[] unitPrices = null;

    public void SetPlayerNameText(string playerName) => playerNameText.text = playerName;

    public void BuyUnit(int unitType)
    {
        int playerCoins = int.Parse(coins.text);
        
        // obtiene el precio de las unidad seleccionada
        int unitPrice = int.Parse(unitPrices[unitType].text);

        if ((playerCoins - unitPrice) < 0) { return; }

        playerCoins -= unitPrice;
        coins.text = playerCoins.ToString();

        GameManager.instance.playerReserveUnits.Add(unitType);
        ReserveUnitScritp.instance.ResetReserveList();
    }

    public void ChangeImageInTurnFrame(List<Sprite> playerImages, List<Color> playerColors, int ammountOfPlayer)
    {
        for (int i = 0; i < playerImages.Count; i++)
        {
            // comprueba si el jugador existe
            if(i > ammountOfPlayer)
            {
                playerImagesInTurnPanel[i].gameObject.SetActive(false);
                continue;
            }

            playerImagesInTurnPanel[i].sprite = playerImages[i];
            playerColorsInTurnPanel[i].color = playerColors[i];
        }
    }

    public void ShowEndTurnButton(bool state)
    {
        if (endTurnButton.gameObject.activeSelf == state) { return; }

        endTurnButton.gameObject.SetActive(state);
    }

    public void CancelAttack() => ConnectionManager.instance.CmdPlayerStoppedAttacking();

    public void EndTurn()
    {
        // revisar si se merece tarjeta y darsela

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
        attackMenuPanel.SetActive(false);
    }

    public void ShowReadyButton(int playerType, bool state)
    {
        GameObject buttonType = null;

        if(playerType == 0)
            buttonType = defenderButton;
        else if (playerType == 1)
            buttonType = attackerButton;

        if (!buttonType) { return; }

        if (buttonType.activeSelf == state) { return; }

        buttonType.SetActive(state);
    }

    public void ShowCancelButton(int playerType, bool state)
    {
        if (playerType != 1) { return; }

        if (cancelAttackButton.gameObject.activeSelf == state) { return; }

        cancelAttackButton.gameObject.SetActive(state);
    }

    public void PlayerIsReadyToBattle() => ConnectionManager.instance.CmdPlayerIsReadyToBattle();
}
