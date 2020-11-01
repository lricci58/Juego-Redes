using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby networkManager = null;

    [SerializeField] private GameObject optionsMenuPanel = null;
    [SerializeField] private GameObject lobbyPanel = null;
    [SerializeField] private GameObject changeNameButton = null;
    [SerializeField] private GameObject inputNamePanel = null;
    [SerializeField] private GameObject inputIpPanel = null;
    [SerializeField] private GameObject startButton = null;
    [SerializeField] private InputField ipAddressInputField = null;
    [SerializeField] private Button joinButton = null;

    private void OnEnable()
    {
        NetworkManagerLobby.OnClientConnected += HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby()
    {
        if (MainMenu.instance.playerName == null)
        {
            inputNamePanel.SetActive(true);
            return;
        }

        string ipAddress = ipAddressInputField.text;

        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        // oculata el pop up para poner ip, el menu principal, y muestra el menu de sala
        inputIpPanel.SetActive(false);
        optionsMenuPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        startButton.SetActive(false);

        changeNameButton.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;

        changeNameButton.SetActive(true);
    }
}
