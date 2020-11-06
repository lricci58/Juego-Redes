using System;
using UnityEngine.UI;
using UnityEngine;
using Mirror;

public class LobbyScript : MonoBehaviour
{
    public static LobbyScript instance = null;

    public GameObject optionsMenuPanel = null;

    public Text[] playerNames = null;
    public Image[] playerReadyIcons = null;
    public Image[] playerImages = null;
    public Image[] playerColors = null;

    public Button startGameButton = null;

    public Sprite notReadyIcon = null;
    public Sprite readyIcon = null;

    private void Start()
    {
        instance = this;

        try
        {
            if (ConnectionManager.instance.isServer)
            {
                startGameButton.gameObject.SetActive(true);
                startGameButton.interactable = false;
            }
            else
                startGameButton.gameObject.SetActive(false);
        }
        catch (NullReferenceException)
        {
            Debug.Log("Nothing happened here... see? no errors :)");
        }
    }

    public void ClickedOnReady() => ConnectionManager.instance.CmdReadyUp();

    public void ClickedOnStartGame() => ConnectionManager.instance.CmdStartGame();
}
