using UnityEngine;
using UnityEngine.UI;

public class CancelButtonScript : MonoBehaviour
{
    public Button cancelButton;

    void Start()
    {
        cancelButton.onClick.AddListener(ClickBoton);
    }

    private void ClickBoton()
    {
        ConnectionManager.instance.CmdPlayerStoppedAttacking();
    }
}
