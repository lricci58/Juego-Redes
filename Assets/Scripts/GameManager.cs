using UnityEngine;
using Mirror;
using System;
using System.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance = null;

    /* [NonSerialized] */ public int playerBattleSide = 2;
    /* [NonSerialized] */ public List<int> armyToBattle;
    [NonSerialized] public List<string> misPaises;

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

        // @TODO: hacer animacion de pintar pais

        int contadorTurno = Random.Range(0, NetworkServer.connections.Count);

        // crea el orden de la lista
        for (int turn = 0; turn < NetworkServer.connections.Count; turn++)
        {
            ConnectionManager.instance.TargetSetYourTurnNumber(NetworkServer.connections[contadorTurno], turn);

            if (++contadorTurno > NetworkServer.connections.Count - 1) { contadorTurno = 0; }
        }
    }
}
