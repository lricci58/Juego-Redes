using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance = null;

    [SerializeField] private NetworkManagerLobby networkManager = null;

    [SerializeField] private GameObject optionsMenuPanel = null;
    [SerializeField] private GameObject lobbyPanel = null;
    [SerializeField] private GameObject changeNameButton = null;
    [SerializeField] private GameObject inputNamePanel = null;

    [SerializeField] private InputField nameInputField = null;
    [NonSerialized] public string playerName = null;
    [NonSerialized] public Image playerImage = null;
    [NonSerialized] public Image playerColor = null;

    private void Start() => instance = this;

    public void HostLobby()
    {
        if (playerName == null)
        {
            inputNamePanel.SetActive(true);
            return;
        }

        networkManager.StartHost();

        optionsMenuPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        changeNameButton.SetActive(false);
    }

    public void SaveName()
    {
        playerName = nameInputField.text;

        // save name on txt or something
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
