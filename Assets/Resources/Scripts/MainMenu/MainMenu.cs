using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance = null;

    [SerializeField] private NetworkManagerLobby networkManager = null;

    [Header("Main Menu Panels")]
    [SerializeField] private GameObject optionsMenuPanel = null;
    [SerializeField] private GameObject lobbyPanel = null;

    [Header("Change Name UI")]
    [SerializeField] private GameObject changeNameButton = null;
    [SerializeField] private GameObject inputNamePanel = null;

    [SerializeField] private InputField nameInputField = null;
    [SerializeField] private Text mainMenuName = null;
    [NonSerialized] public string playerName = "";
    [NonSerialized] public Image playerImage = null;
    [NonSerialized] public Image playerColor = null;

    void Start() => instance = this;

    void OnEnable()
    {
        playerName = ReadFromTxt();
        mainMenuName.text = playerName;

        if (playerName == "")
            inputNamePanel.SetActive(true);
    }

    public void HostLobby()
    {
        if (playerName == "")
        {
            inputNamePanel.SetActive(true);
            return;
        }

        networkManager.StartHost();

        optionsMenuPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        changeNameButton.SetActive(false);
    }

    public void SaveName()
    {
        playerName = nameInputField.text;
        mainMenuName.text = playerName;

        // guarda el nombre en un txt
        WriteToTxt(playerName);
    }

    static void WriteToTxt(string nameToSave)
    {
        string path = "Assets/Resources/PlayerData/player_data.txt";

        // @TODO: crear el archivo si no existe
        // File.Create("Assets/Resources/PlayerData/");

        // borra todas las lineas
        File.WriteAllText(path, String.Empty);

        // escribe texto en el txt
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(nameToSave);
        writer.Close();
    }

    static string ReadFromTxt()
    {
        string path = "Assets/Resources/PlayerData/player_data.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        string nameInText = reader.ReadToEnd();
        reader.Close();

        return nameInText;
    }

    public void QuitGame() => Application.Quit();
}
