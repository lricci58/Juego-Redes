using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public int[] unitList;

    void Awake()
    {
        // se asegura que solo exista una instancia del controlador de juego
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
