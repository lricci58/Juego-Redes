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
        if (miTurno != turnoActual) { canvas.CanBuyUnits(false); return; }

        canvas.CanBuyUnits(true);
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
        if(tipoJugador == 0)
        {
            // si el defensor no tiene unidades en el pais es un auto-conquest
            if (GameObject.Find(paisJugador).GetComponent<Pais>().countryGarrison.Count <= 0)
            {
                // hace el swap de color y dueño de pais entre los involucrados
                GameManager.instance.misPaises.Remove(paisJugador);
                ConnectionManager.instance.CmdChangeCountryOwners(paisJugador, paisEnemigo);

                // desactiva la ventana de ataque
                ConnectionManager.instance.CmdPlayerStoppedAttacking();
                
                return;
            }
        }

        GameManager.instance.countryInvolvedInBattle = paisJugador;

        // muestra el panel de ataque
        canvas.ShowAttackMenu(imagenJugador, imagenEnemigo, paisJugador, paisEnemigo);

        // instanciar boton cancelar ataque en caso de ser atacante
        canvas.ShowCancelButton(tipoJugador, true);

        // setea el tipo de jugador (atacante o defensor)
        GameManager.instance.playerBattleSide = tipoJugador;
    }

    public void OcultarMenuAtaque()
    {
        canvas.HideAttackMenu();

        if (miTurno == turnoActual)
            canvas.ShowEndTurnButton(true);

        // devuelve el tipo de jugador en batalla a espectador
        GameManager.instance.playerBattleSide = 2;
        GameManager.instance.countryInvolvedInBattle = "";
    }

    public void ActualizarEstadoPaises(string nombrePaisSeleccionado, string[] nombrePaisesLimitrofes)
    {
        // comprueba si hay un pais seleccionado, en cuyo caso lo deselecciona
        if (HayPaisSeleccionado()) { return; }

        GameObject paisSeleccionado = GameObject.Find(nombrePaisSeleccionado);
        // paisSeleccionado.GetComponent<SpriteRenderer>().color -= colorSeleccionado;
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

        // comprueba si existe algun pais seleccionado
        if (!posibleSeleccionado) { return false; }

        // deselecciona al pais seleccionado
        // posibleSeleccionado.GetComponent<Pais>().ChangeColorToOriginal();
        posibleSeleccionado.tag = "Country";

        GameObject[] posiblesLimitrofes = GameObject.FindGameObjectsWithTag("Bordering");
        // deselecciona a sus limitrofes
        foreach (GameObject posibleLimitrofe in posiblesLimitrofes)
        {
            if (!posibleLimitrofe) { continue; }

            posibleLimitrofe.GetComponent<Pais>().ChangeColorToOriginal();
            posibleLimitrofe.tag = "Country";
        }

        posibleSeleccionado.GetComponent<Pais>().Unselect();

        return true;
    }

    public void UnselectCountry() => HayPaisSeleccionado();

    public void CheckIfPlayerCanGoToBattle()
    {
        bool canGoToBattle = false;

        // comprueba que el panel de "al combate" tenga al menos una unidad
        if (GameManager.instance.unitsToBattle.Count > 0)
            canGoToBattle = true;

        canvas.CanBeReadyToBattle(canGoToBattle);
    }
}
