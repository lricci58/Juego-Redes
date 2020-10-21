using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    /* [NonSerialized] */ public int playerBattleSide = 2;
    /* [NonSerialized] */ public int[] unitList;

    void Awake()
    {
        // se asegura que solo exista una instancia del controlador de juego
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // mantiene el objeto como singleton
        DontDestroyOnLoad(gameObject);
    }
}
