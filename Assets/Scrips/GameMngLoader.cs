using UnityEngine;

public class GameMngLoader : MonoBehaviour
{
    public GameManager gameManager;

    void Awake()
    {
        if (GameManager.instance == null)
            Instantiate(gameManager);
    }
}
