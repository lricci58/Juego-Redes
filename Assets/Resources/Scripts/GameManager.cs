using UnityEngine;
using Mirror;
using System;
using System.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance = null;
    List<Color32> playerColors = new List<Color32>();

    /* [NonSerialized] */
    public int playerBattleSide = 2;
    /* [NonSerialized] */ public List<int> playerReserveUnits;
    [NonSerialized] public List<string> misPaises = new List<string>();
    private Color playerColor;

    void Awake()
    {
        // se asegura que solo exista una instancia del controlador de juego
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // mantiene el objeto como singleton
        DontDestroyOnLoad(gameObject);

        // comprueba si la instancia de ventana esta en un host
        if (!ConnectionManager.instance.isServer) { return; }

        playerColors.Add(new Color32(76, 116, 255, 255));
        playerColors.Add(new Color32(200, 0, 0, 255));
        playerColors.Add(new Color32(198, 165, 0, 255));
        playerColors.Add(new Color32(188, 28, 161, 255));
        playerColors.Add(new Color32(38, 127, 0, 255));
        playerColors.Add(new Color32(108, 36, 175, 255));
        playerColors.Add(new Color32(165, 88, 24, 255));
        playerColors.Add(new Color32(255, 109, 0, 255));

        int contadorTurno = Random.Range(0, NetworkServer.connections.Count);

        // crea el orden de la lista
        for (int turn = 0; turn < NetworkServer.connections.Count; turn++)
        {
            ConnectionManager.instance.TargetSetYourTurnNumber(NetworkServer.connections[contadorTurno], turn);
            ConnectionManager.instance.TargetSetYourColor(NetworkServer.connections[contadorTurno], playerColors[turn]);

            if (++contadorTurno > NetworkServer.connections.Count - 1) { contadorTurno = 0; }
        }

        List<GameObject> listaPaisesEnMapa = GameObject.FindGameObjectsWithTag("Country").ToList();
        int cantPaises = listaPaisesEnMapa.Count;
        int numJugador = 0;

        for (int i = 0; i < cantPaises; i++)
        {
            int paisRandom = Random.Range(0, listaPaisesEnMapa.Count);
            // indica al jugador que pais le toco
            ConnectionManager.instance.TargetAddCountry(NetworkServer.connections[numJugador], listaPaisesEnMapa[paisRandom].name);
            listaPaisesEnMapa.RemoveAt(paisRandom);

            // mueve al siguiente jugador en la lista
            if (++numJugador > NetworkServer.connections.Count - 1) { numJugador = 0; }
        }
    }

    public void SetPlayerColor(Color newColor) => playerColor = newColor;

    public void AddCountry(string pais)
    {
        misPaises.Add(pais);
        ConnectionManager.instance.CmdPaintCountry(pais, playerColor);
    }
}
