using UnityEngine;
using Mirror;
using System;
using System.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance = null;
    List<Color> playerColors = new List<Color>();

    /* [NonSerialized] */ public int playerBattleSide = 2;
    /* [NonSerialized] */ public List<int> armyToBattle;
    [NonSerialized] public List<string> misPaises;
    private Color playerColor;
    void Awake()
    {
        misPaises = new List<string>();
      

        // se asegura que solo exista una instancia del controlador de juego
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        // mantiene el objeto como singleton
        DontDestroyOnLoad(gameObject);

        // comprueba si la instancia de ventana esta en un host
        if (!ConnectionManager.instance.isServer) { return; }
        playerColors.Add(new Color(0.253f, 0.148f, 0.61f));
        playerColors.Add(new Color(0.76f, 0.116f, 0.255f));
        playerColors.Add(new Color(0.153f, 0f, 0f));
        playerColors.Add(new Color(1f, 0.213f, 0.1f));
        playerColors.Add(new Color(0.188f, 0.28f, 0.161f));
        playerColors.Add(new Color(0.49f, 0.164f, 0f));
        playerColors.Add(new Color(1f, 0.93f, 0.228f));


        // @TODO: hacer animacion de pintar pais

        int contadorTurno = Random.Range(0, NetworkServer.connections.Count);

        // crea el orden de la lista
        for (int turn = 0; turn < NetworkServer.connections.Count; turn++)
        {
            ConnectionManager.instance.TargetSetYourTurnNumber(NetworkServer.connections[contadorTurno], turn);
            ConnectionManager.instance.TargetSetYourColor(NetworkServer.connections[contadorTurno], playerColors[turn]);

            if (++contadorTurno > NetworkServer.connections.Count - 1) { contadorTurno = 0; }
        }

        List<GameObject> listaPaisesEnMapa = GameObject.FindGameObjectsWithTag("pais").ToList();
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
    public void SetPlayerColor(Color newColor)

    {
        playerColor = newColor;
    }

    public void AgregarPais(string pais)
    {
        misPaises.Add(pais);
        ConnectionManager.instance.CmdPintarPais(pais, playerColor);

    }
    
}
