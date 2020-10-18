using UnityEngine;
using UnityEngine.UI;

public class StartBattleButton : MonoBehaviour
{
    public Button startBattleButton;

    // Start is called before the first frame update
    void Start()
    {
        startBattleButton.onClick.AddListener(ClickBoton);
    }

    private void ClickBoton()
    {
        // oculta la ui de despliegue
        BattleManager.instance.canvas.ShowDeploymentPanel(false);
        BattleManager.instance.canvas.ShowStartBattleButton(false);
        BattleManager.instance.canvas.ShowWaitingText(true);

        ConnectionManager.instance.CmdEndedDeployFase();
    }
}
