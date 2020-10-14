using UnityEngine;

public class GameLoader : MonoBehaviour
{
    public GameManager gameManager;

    void Awake()
    {
        if (GameManager.instance == null)
            Instantiate(gameManager);
    }
}
