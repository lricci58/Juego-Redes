using UnityEngine;

public class BattleLoader : MonoBehaviour
{
    public BattleManager battleManager;

    void Awake()
    {
        if (BattleManager.instance == null)
            Instantiate(battleManager);
    }
}
