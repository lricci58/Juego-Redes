using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitButtonScript : MonoBehaviour
{
    public Button endTurnButton;

    void Start()
    {
        endTurnButton.onClick.AddListener(ClickBoton);
    }

    private void ClickBoton() => Application.Quit();
}
