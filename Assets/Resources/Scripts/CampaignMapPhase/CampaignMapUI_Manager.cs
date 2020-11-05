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

    [SerializeField] private GameObject reserveUnitsPanel = null;

    [Header("Attack Menu UI Objects")]
    [SerializeField] private GameObject attackMenuPanel = null;
    [SerializeField] private Button cancelAttackButton = null;
    [SerializeField] private GameObject readyButton = null;

    [SerializeField] private GameObject garrisonPanel = null;
    [SerializeField] private GameObject toBattlePanel = null;

    [SerializeField] private Image playerImage = null;
    [SerializeField] private Image enemyImage = null;
    [SerializeField] private Text playerCountry = null;
    [SerializeField] private Text enemyCountry = null;
    [SerializeField] private Text playerName = null;
    [SerializeField] private Text enemyName = null;

    [Header("Turn UI Objects")]
    [SerializeField] private Image[] playerImagesInTurnPanel = null;
    [SerializeField] private Image[] playerColorsInTurnPanel = null;

    [Header("BuyUnitsPanel")]
    [SerializeField] private Text[] unitPrices = null;

    public void SetPlayerNameText(string playerName) => playerNameText.text = playerName;

    public void BuyUnit(int unitType)
    {
        int playerCoins = int.Parse(coins.text);
        int unitPrice = int.Parse(unitPrices[unitType].text);

        if ((playerCoins - unitPrice) < 0) { return; }

        playerCoins -= unitPrice;
        coins.text = playerCoins.ToString();

        GameManager.instance.playerReserveUnits.Add(unitType);
        ShowReserveUnitsPanel(true);
    }

    public void ShowReserveUnitsPanel(bool state)
    {
        if(reserveUnitsPanel.activeSelf != state)
            reserveUnitsPanel.SetActive(state);

        // actualiza la lista del panel de reservas
        reserveUnitsPanel.GetComponent<UnitsPanelScript>().SetPanelHeader("Unidades de Reserva");
        reserveUnitsPanel.GetComponent<UnitsPanelScript>().UpdateUnitsPanel(GameManager.instance.playerReserveUnits);
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

    public void ShowAttackMenu(Sprite playerSprite, Sprite enemySprite, string playerCountryName, string enemyCounrtyName)
    {
        ShowEndTurnButton(false);
        attackMenuPanel.SetActive(true);

        string panelHeaderText = "Guarnicion de " + playerCountryName;
        List<int> garrisonList = GameObject.Find(playerCountryName).GetComponent<Pais>().countryGarrison;
        
        garrisonPanel.GetComponent<UnitsPanelScript>().SetPanelHeader(panelHeaderText);
        garrisonPanel.GetComponent<UnitsPanelScript>().UpdateUnitsPanel(garrisonList);

        // setea las imagenes y nombres de los paises en cuestion
        playerImage.sprite = playerSprite;
        enemyImage.sprite = enemySprite;
        playerCountry.text = playerCountryName;
        enemyCountry.text = enemyCounrtyName;
        playerName.text = playerNameText.text;
    }

    public void HideAttackMenu()
    {
        attackMenuPanel.SetActive(false);
        GameManager.instance.unitsToBattle.Clear();
    }

    public void CheckIfPlayerCanGoToBattle()
    {
        // comprueba que el panel de "al combate" tenga al menos una unidad
        if (GameManager.instance.unitsToBattle.Count <= 0) { return; }

        readyButton.GetComponent<Button>().interactable = true;
    }

    public void ShowCancelButton(int playerType, bool state)
    {
        if (playerType != 1) { return; }

        if (cancelAttackButton.gameObject.activeSelf == state) { return; }

        cancelAttackButton.gameObject.SetActive(state);
    }

    public void PlayerIsReadyToBattle() => ConnectionManager.instance.CmdReadyUp();

    public void PlayerIsRequestingAutoBattle() => ConnectionManager.instance.CmdRequestingAutoBattle();
}
