using UnityEngine;
using UnityEngine.UI;

public class EndTurnButtonScript : MonoBehaviour
{
    public Button endTurnButton;

    void Start()
    {
        endTurnButton.onClick.AddListener(ClickBoton);
    }

    private void ClickBoton()
    {
        // resetea las unidades luego de terminar el turno
        foreach (UnitScript unit in BattleManager.instance.army)
            unit.ResetUnitsInArmy();

        ConnectionManager.instance.CmdEndTurn(GameManager.instance.playerBattleSide);
    }
}
