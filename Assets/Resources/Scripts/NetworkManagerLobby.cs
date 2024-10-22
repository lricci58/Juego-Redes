﻿using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerLobby : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [SerializeField] private ConnectionManager connectionManagerPrefab = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public List<ConnectionManager> RoomPlayers { get; } = new List<ConnectionManager>();

    public override void OnClientConnect(NetworkConnection conn)
    {
        // hace la logica base
        base.OnClientConnect(conn);
        
        // ejecuta nuestro evento
        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        // hace la logica base
        base.OnClientDisconnect(conn);

        // ejecuta nuestro evento
        OnClientDisconnected?.Invoke();
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        conn.Disconnect();
        base.OnClientError(conn, errorCode);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        // comprueba si entran mas clientes o si estamos en la escena adecuada
        if (numPlayers == maxConnections || SceneManager.GetActiveScene().name != "MainMenuScene")
        {
            // se desconecta el cliente que esta intentando entrar
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        NotifyPlayersOfReadyState();
        base.OnServerDisconnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().name == "MainMenuScene")
        {
            // instancia el prefab de jugador
            ConnectionManager connectionManagerInstance = Instantiate(connectionManagerPrefab);

            // se añade a la conexion para que identifique a su instancia de ventana
            NetworkServer.AddPlayerForConnection(conn, connectionManagerInstance.gameObject);
        }
    }

    public override void OnStopServer() => RoomPlayers.Clear();

    public override void OnStopClient() => RoomPlayers.Clear();

    public void NotifyPlayersOfReadyState()
    {
        foreach (ConnectionManager player in RoomPlayers)
            player.HandleReadyToStart(IsReadyToStart());
    }

    private bool IsReadyToStart()
    {
        // comprueba que haya al menos un minimo de jugadores
        if (numPlayers < minPlayers) { return false; }

        // comprueba si hay algun jugador que no este listo
        foreach (ConnectionManager player in RoomPlayers)
            if (!player.isReady) { return false; }

        return true;
    }
}
