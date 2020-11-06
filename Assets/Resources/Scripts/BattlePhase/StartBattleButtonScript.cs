using UnityEngine;
using UnityEngine.UI;

public class StartBattleButtonScript : MonoBehaviour
{
    public Button startBattleButton;

    // Start is called before the first frame update
    void Start()
    {
        startBattleButton.onClick.AddListener(ClickBoton);
    }

    private void ClickBoton()
    {
        // oculta la UI de despliegue
        BattleManager.instance.canvas.ShowDeploymentPanel(false);
        BattleManager.instance.canvas.ShowReadyButton(false);
        BattleManager.instance.canvas.ShowWaitingText(true);

        ConnectionManager.instance.CmdEndedDeployFase();
    }
}
