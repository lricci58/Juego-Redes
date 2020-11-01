using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby networkManager = null;

    [SerializeField] private GameObject optionsMenuPanel = null;
    [SerializeField] private GameObject lobbyPanel = null;

    public void HostLobby()
    {
        networkManager.StartHost();

        optionsMenuPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }
}
