using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CampaignMapUI_Manager : MonoBehaviour
{
    [Header("General UI Objects")]
    [SerializeField] private Text coins = null;
    [SerializeField] private Text playerNameText = null;
    [SerializeField] private Button endTurnButton = null;
    [SerializeField] private Button buyUnitsButton = null;
    [SerializeField] private GameObject reserveUnitsPanel = null;
    [SerializeField] private GameObject[] greenArrowButtons = null;

    [Header("Attack Menu UI Objects")]
    [SerializeField] private GameObject attackMenuPanel = null;
    [SerializeField] private Button cancelAttackButton = null;
    [SerializeField] private Button readyButton = null;

    [SerializeField] private GameObject garrisonPanel = null;
    [SerializeField] private GameObject toBattlePanel = null;

    [SerializeField] private Image playerImage = null;
    [SerializeField] private Image enemyImage = null;
    [SerializeField] private Text playerCountry = null;
    [SerializeField] private Text enemyCountry = null;
    [SerializeField] private Text playerName = null;
    [SerializeField] private Text enemyName = null;
    [SerializeField] private Image[] playerIcons = null;

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
        // @TODO: revisar si se merece tarjeta y darsela

        ShowEndTurnButton(false);
        CanUseGreenArrowButtons(false);
        CanBuyUnits(false);

        ConnectionManager.instance.CmdEndTurn(GameManager.instance.playerBattleSide);
    }

    public void ShowAttackMenu(Sprite playerSprite, Sprite enemySprite, string playerCountryName, string enemyCountryName)
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
        enemyCountry.text = enemyCountryName;
        playerName.text = playerNameText.text;
        // enemyName.text = ;
    }

    public void UpdateAttackMenuDisplay(int playerToUpdate, Sprite newReadyIcon)
    {
        playerIcons[playerToUpdate].sprite = newReadyIcon;
    }

    public void HideAttackMenu()
    {
        attackMenuPanel.SetActive(false);
        cancelAttackButton.gameObject.SetActive(false);
        GameManager.instance.unitsToBattle.Clear();
    }

    public void CanBeReadyToBattle(bool state)
    {
        if (readyButton.interactable == state) { return; }

        readyButton.interactable = state;
    }

    public void CanBuyUnits(bool state)
    {
        if (buyUnitsButton.interactable == state) { return; }

        buyUnitsButton.interactable = state;
    }

    public void ShowCancelButton(int playerType, bool state)
    {
        // solo debe mostrarselo al atacante
        if (playerType != 1) { return; }

        if (cancelAttackButton.gameObject.activeSelf == state) { return; }

        cancelAttackButton.gameObject.SetActive(state);
    }

    public void PlayerIsReadyToBattle() => ConnectionManager.instance.CmdReadyUp();

    public void PlayerIsRequestingAutoBattle() => ConnectionManager.instance.CmdRequestingAutoBattle();

    public void CanUseGreenArrowButtons(bool state)
    {
        // cambia el estado interactibilidad de los botones
        foreach (GameObject button in greenArrowButtons)
            button.SetActive(state);
    }
}
