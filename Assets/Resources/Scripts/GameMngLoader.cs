using UnityEngine;

public class GameMngLoader : MonoBehaviour
{
    public GameManager gameManager;

    void Awake()
    {
        // instancia el GameManager si no existe
        if (GameManager.instance == null)
            Instantiate(gameManager);
    }
}
