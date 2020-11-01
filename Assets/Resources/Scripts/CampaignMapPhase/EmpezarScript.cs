using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EmpezarScript : MonoBehaviour
{
    public Button unitButton;
    private string mainMapSceneName = "CampaignMapScene";
    
    void Start()
    {
        unitButton.onClick.AddListener(ClickBoton);
    }

    // Update is called once per frame
    private void ClickBoton() => SceneManager.LoadScene(mainMapSceneName);
}
