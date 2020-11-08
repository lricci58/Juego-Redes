using UnityEngine;

public class GameMngLoader : MonoBehaviour
{
    [SerializeField] private GameManager gameManager = null;

    void Awake()
    {
        // instancia el GameManager si no existe
        if (GameManager.instance != null) { return; }

        Instantiate(gameManager);
    }
}
