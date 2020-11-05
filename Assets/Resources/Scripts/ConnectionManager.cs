using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class ConnectionManager : NetworkBehaviour
{
    public static ConnectionManager instance = null;

    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    [NonSerialized] public bool isReady = false;
    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    [NonSerialized] public string playerDisplayName = "Cargando...";

    [NonSerialized] public Image playerDisplayImage = null;
    [NonSerialized] public Image playerDisplayColor = null;

    private NetworkManagerLobby room;
    public NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(this);

        if (!isLocalPlayer) { return; }

        instance = this;
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(MainMenu.instance.playerName);
    }

    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);
        HandleReadyToStart(false);

        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);

        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue)
    {
        if (SceneManager.GetActiveScene().name == "MainMenuScene")
            UpdateDisplay();
        // else if (SceneManager.GetActiveScene().name == "CampaignMapScene")
        // actualizar el estado visual de los jugadores
    }

    public void HandleDisplayNameChanged(string oldvalue, string newValue) => UpdateDisplay();

    public void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach (ConnectionManager player in Room.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        // reinicia el estado de todos los jugadores en la sala
        for (int i = 0; i < LobbyScript.instance.playerNames.GetLength(0); i++)
        {
            LobbyScript.instance.playerNames[0].text = "Esperando...";
            LobbyScript.instance.playerReadyIcons[i].gameObject.SetActive(false);
            //LobbyScript.instance.playerImages[i].gameObject.SetActive(false);
            //LobbyScript.instance.playerColors[i].gameObject.SetActive(false);
        }

        // actualiza el estado de los jugadores en la UI propia
        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            LobbyScript.instance.playerNames[i].text = Room.RoomPlayers[i].playerDisplayName;

            if (!LobbyScript.instance.playerNames[i].text.Equals("Esperando..."))
            {
                LobbyScript.instance.playerReadyIcons[i].gameObject.SetActive(true);
                LobbyScript.instance.playerReadyIcons[i].sprite = Room.RoomPlayers[i].isReady ? LobbyScript.instance.readyIcon : LobbyScript.instance.notReadyIcon;
            }
            else
            {
                LobbyScript.instance.playerReadyIcons[i].gameObject.SetActive(false);
                //LobbyScript.instance.playerImages[i].gameObject.SetActive(false);
                //LobbyScript.instance.playerColors[i].gameObject.SetActive(false);
            }
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isServer) { return; }

        LobbyScript.instance.startGameButton.interactable = readyToStart;
    }

    [Command]
    public void CmdSetDisplayName(string newDisplayName) => playerDisplayName = newDisplayName;

    [Command]
    public void CmdReadyUp()
    {
        isReady = !isReady;

        if (SceneManager.GetActiveScene().name == "MainMenuScene")
            Room.NotifyPlayersOfReadyState();
        else if (SceneManager.GetActiveScene().name == "CampaignMapScene")
            if (IsEveryOneReady())
            {
                foreach (ConnectionManager player in Room.RoomPlayers)
                    player.isReady = false;

                RpcChangeScene("BattleScene");
            }
    }

    private bool IsEveryOneReady()
    {
        // comprueba que los 2 jugadores esten listos
        int readyCounter = 0;
        foreach (ConnectionManager player in Room.RoomPlayers)
            if (player.isReady) { readyCounter++; }

        if (readyCounter == 2) { return true; }

        return false;
    }

    [Command]
    public void CmdRequestingAutoBattle()
    {
        // send request to player (maybe )
    }

    [Command]
    public void CmdStartGame()
    {
        // comprueba que la persona que este tratando de iniciar juego sea el lider
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient) { return; }

        foreach (ConnectionManager player in Room.RoomPlayers)
            player.isReady = false;

        RpcChangeScene("CampaignMapScene");    
    }

    [Command]
    public void CmdSpawnObject(int index, Vector3 unitLocalPosition)
    {
        GameObject originalPrefab = BattleManager.instance.map.unitPrefabs[index];
        GameObject instance = Instantiate(originalPrefab, unitLocalPosition, Quaternion.identity);
        instance.transform.position = unitLocalPosition;

        // @NOTE: a veces el cliente ejecuta el comando de mas

        // spawnea la unidad y otorga la autoridad del objeto al cliente del parametro
        NetworkServer.Spawn(instance, connectionToClient);
    }

    [Command]
    public void CmdEndedDeployFase() => RpcUpdateDesployCount();

    [ClientRpc]
    public void RpcUpdateDesployCount() => BattleManager.instance.endedDeployFaseCount++;

    [Command]
    public void CmdCheckUnitOwner(NetworkIdentity unitIdentity)
    {
        if (connectionToClient.clientOwnedObjects.Contains(unitIdentity))
            TargetUpdateArmy(connectionToClient, unitIdentity);
    }

    [TargetRpc]
    public void TargetUpdateArmy(NetworkConnection conn, NetworkIdentity unitIdentity)
    {
        BattleManager.instance.AddUnitToArmy(unitIdentity.GetComponent<UnitScript>());
    }

    [Command]
    public void CmdAttackUnit(GameObject unitObject, float damage)
    {
        UnitScript unit = unitObject.GetComponent<UnitScript>();

        if (unit.currentHealth > 0)
            unit.currentHealth -= damage;
    }

    [Command]
    public void CmdCountryWasSelected(string selectedName, string[] borderingNames) => RpcUpdateSelectedCountryOnClients(selectedName, borderingNames);

    [ClientRpc]
    public void RpcUpdateSelectedCountryOnClients(string selectedName, string[] borderingNames)
    {
        MapManager.instancia.ActualizarEstadoPaises(selectedName, borderingNames);
    }

    [Command]
    public void CmdEndTurn(int playerBattleSide) => RpcPlayerEndedTurn(playerBattleSide, NetworkServer.connections.Count - 1);

    [ClientRpc]
    public void RpcPlayerEndedTurn(int playerBattleSide, int amountPlayers)
    {
        if (SceneManager.GetActiveScene().name == "CampaignMapScene")
        {
            // cambia de turno
            if (++MapManager.instancia.turnoActual > amountPlayers) { MapManager.instancia.turnoActual = 0; }

            // deselecciona cualquier pais que pueda estar seleccionado
            MapManager.instancia.HayPaisSeleccionado();

            // muestra el boton de terminar de turno en su turno
            if (MapManager.instancia.miTurno == MapManager.instancia.turnoActual)
                MapManager.instancia.canvas.ShowEndTurnButton(true);
            else
                MapManager.instancia.canvas.ShowEndTurnButton(false);

            for (int i = 0; i < MapManager.instancia.turnList.Count - 1; i++)
                MapManager.instancia.turnList[i] = MapManager.instancia.turnList[i + 1];

            MapManager.instancia.turnList[MapManager.instancia.turnList.Count - 1] = 0;

            // MapManager.instancia.UpdateVisualTurnOrder(MapManager.instancia.turnList);
        }
        else if (SceneManager.GetActiveScene().name == "BattleScene")
        {
            // si se termino el turno del defensor (0), empieza el del atacante(0++)
            if (playerBattleSide == 0)
                BattleManager.instance.currentTurn++;
            else if (playerBattleSide == 1)
                BattleManager.instance.currentTurn--;

            // muestra el boton de terminar de turno en su turno
            if (BattleManager.instance.myTurnNumber == BattleManager.instance.currentTurn)
                BattleManager.instance.canvas.ShowEndTurnButton(true);
            else
                BattleManager.instance.canvas.ShowEndTurnButton(false);
        }
    }

    [Command]
    public void CmdPlayerAttacked(string attackedCountry, string attackerCountry)
    {
        // busca el jugador que esta siendo atacado
        RpcSearchPlayersInvolvedInAttack(attackedCountry, attackerCountry);
    }

    [ClientRpc]
    public void RpcSearchPlayersInvolvedInAttack(string attackedCountry, string attackerCountry)
    {
        int playerType = 2;
        // busca al dueño del pais atacado
        if (GameManager.instance.misPaises.Contains(attackedCountry))
            playerType = 0;    
        else if (GameManager.instance.misPaises.Contains(attackerCountry))
            playerType = 1;

        if (playerType == 2) { return; }

        // deselecciona al pais seleccionado y sus limitrofes
        MapManager.instancia.HayPaisSeleccionado();

        // abre el menu de ataque en modo atacante
        MapManager.instancia.DesplegarMenuAtaque(null, null, attackedCountry, attackerCountry, playerType);
    }

    [Command]
    public void CmdChangeCountryOwners(string conqueredCountry, string attackerCountry)
    {
        RpcSearchNewCountryOwner(conqueredCountry, attackerCountry);
    }

    [ClientRpc]
    public void RpcSearchNewCountryOwner(string conqueredCountry, string attackerCountry)
    {
        if (!GameManager.instance.misPaises.Contains(attackerCountry)) { return; }

        GameManager.instance.AddCountry(conqueredCountry);
    }

    [Command]
    public void CmdPlayerStoppedAttacking() => RpcCloseAttackMenu();

    [ClientRpc]
    public void RpcCloseAttackMenu() => MapManager.instancia.OcultarMenuAtaque();

    [ClientRpc]
    public void RpcChangeScene(string sceneName) => SceneManager.LoadScene(sceneName);

    [TargetRpc]
    public void TargetAddCountry(NetworkConnection conn, string nombrePais) => GameManager.instance.AddCountry(nombrePais);

    [TargetRpc]
    public void TargetSetYourTurnNumber(NetworkConnection conn, int turnNumber)
    {
        MapManager.instancia.miTurno = turnNumber;  
        RpcUpdateTurnList(turnNumber);

        // muestra el boton de terminar de turno en su turno
        if (MapManager.instancia.miTurno == MapManager.instancia.turnoActual)
            MapManager.instancia.canvas.ShowEndTurnButton(true);
    }

    [TargetRpc]
    public void TargetSetYourColor(NetworkConnection conn, Color playerColor) => GameManager.instance.SetPlayerColor(playerColor);

    [Command]
    public void CmdPaintCountry(string pais, Color color) => RpcPaintCountry(pais, color);

    [ClientRpc]
    public void RpcPaintCountry(string pais, Color color)
    {
        GameObject.Find(pais).GetComponent<Pais>().ChangeOriginalColor(color);
    }

    [ClientRpc]
    public void RpcUpdateTurnList(int turnNumber) => MapManager.instancia.turnList.Add(turnNumber);
}
