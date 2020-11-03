using System;
using UnityEngine.UI;
using UnityEngine;
using Mirror;

public class LobbyScript : MonoBehaviour
{
    public static LobbyScript instance = null;

    [SerializeField] private GameObject optionsMenuPanel = null;

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

        if (startGameButton.gameObject.activeSelf)
            startGameButton.interactable = false;
    }

    public void ClickedOnReady() => ConnectionManager.instance.CmdReadyUp();

    public void ClickedOnStartGame() => ConnectionManager.instance.CmdStartGame();

    public void LeaveRoom()
    {
        // desconecta al jugador
        if (ConnectionManager.instance.isServer)
            ConnectionManager.instance.Room.StopServer();
        else
            ConnectionManager.instance.Room.StopClient();

        // cambia de menu
        optionsMenuPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
