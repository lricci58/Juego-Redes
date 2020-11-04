using System;
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapManager : NetworkBehaviour
{
    public static MapManager instancia = null;

    public CampaignMapUI_Manager canvas;

    [NonSerialized] public int miTurno;
    [NonSerialized] [SyncVar] public int turnoActual = 0;
    [NonSerialized] public List<int> turnList = new List<int>();
    
    private Color colorSeleccionado = new Color(0.3f, 0.3f, 0.3f);
    private Color colorLimitrofe = new Color(0.3f, 0.3f, 0.3f);

    void Start() 
    {
        instancia = this;

        canvas.SetPlayerNameText(ConnectionManager.instance.playerDisplayName);
    }

    void Update()
    {
        if (miTurno != turnoActual) { return; }
    }

    public void UpdateVisualTurnOrder(List<int> turnList)
    {
        List<Sprite> playerImages = new List<Sprite>();
        List<Color> playerColors = new List<Color>();

        // crea las listas de 
        foreach (ConnectionManager player in ConnectionManager.instance.Room.RoomPlayers)
        {
            playerImages.Add(player.playerDisplayImage.sprite);
            playerColors.Add(player.playerDisplayColor.color);
        }

        // ordenar las imagenes segun la turnList

        canvas.ChangeImageInTurnFrame(playerImages, playerColors, ConnectionManager.instance.Room.RoomPlayers.Count);
    }

    public void DesplegarMenuAtaque(Sprite imagenJugador, Sprite imagenEnemigo, string paisJugador, string paisEnemigo, int tipoJugador)
    {
        // muestra el panel de ataque
        canvas.ShowAttackMenu(imagenJugador, imagenEnemigo, paisJugador, paisEnemigo);

        // instanciar boton defensor o atacante segun el tipo de jugador
        canvas.ShowReadyButton(tipoJugador, true);

        // instanciar boton cancelar ataque en caso de ser atacante
        canvas.ShowCancelButton(tipoJugador, true);
    }

    public void OcultarMenuAtaque()
    {
        canvas.HideAttackMenu();
        if (miTurno == turnoActual)
            canvas.ShowEndTurnButton(true);
    }

    public void ActualizarEstadoPaises(string nombrePaisSeleccionado, string[] nombrePaisesLimitrofes)
    {
        // comprueba si hay un pais seleccionado, en cuyo caso lo deselecciona
        if (HayPaisSeleccionado()) { return; }

        GameObject paisSeleccionado = GameObject.Find(nombrePaisSeleccionado);
        paisSeleccionado.GetComponent<SpriteRenderer>().color -= colorSeleccionado;
        paisSeleccionado.tag = "Selected";

        foreach (string nombrePaisLimitrofe in nombrePaisesLimitrofes)
        {
            GameObject paisLimitrofe = GameObject.Find(nombrePaisLimitrofe);
            paisLimitrofe.GetComponent<SpriteRenderer>().color += colorLimitrofe;
            paisLimitrofe.tag = "Bordering";
        }
    }

    public bool HayPaisSeleccionado()
    {
        GameObject posibleSeleccionado = GameObject.FindGameObjectWithTag("Selected");
        GameObject[] posiblesLimitrofes = GameObject.FindGameObjectsWithTag("Bordering");

        // comprueba si existe algun pais seleccionado
        if (!posibleSeleccionado) { return false; }

        // deselecciona al pais seleccionado
        posibleSeleccionado.GetComponent<Pais>().ChangeColorToOriginal();
        posibleSeleccionado.tag = "Country";

        // deselecciona a sus limitrofes
        foreach (GameObject posibleLimitrofe in posiblesLimitrofes)
        {
            if (posibleLimitrofe == null) { continue; }

            posibleLimitrofe.GetComponent<Pais>().ChangeColorToOriginal();
            posibleLimitrofe.tag = "Country";
        }

        posibleSeleccionado.GetComponent<Pais>().Unselect();

        return true;
    }

    public void DeseleccionarPais()
    {
        GameObject posibleSeleccionado = GameObject.FindGameObjectWithTag("Selected");
        GameObject[] posiblesLimitrofes = GameObject.FindGameObjectsWithTag("Bordering");
        
        if (!posibleSeleccionado) { return; }

        // deselecciona al pais seleccionado
        posibleSeleccionado.GetComponent<Pais>().ChangeColorToOriginal();
        posibleSeleccionado.tag = "Country";

        // deselecciona a sus limitrofes
        foreach (GameObject posibleLimitrofe in posiblesLimitrofes)
        {
            if (posibleLimitrofe == null) { continue; }

            posibleLimitrofe.GetComponent<Pais>().ChangeColorToOriginal();
            posibleLimitrofe.tag = "Country";
        }

        posibleSeleccionado.GetComponent<Pais>().Unselect();
    }
}
