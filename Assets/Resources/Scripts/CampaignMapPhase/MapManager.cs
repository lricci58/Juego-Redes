using System;
using UnityEngine;
using Mirror;
using UnityEngine.UIElements;

public class MapManager : NetworkBehaviour
{
    public static MapManager instancia = null;

    [SerializeField] private GameObject textNumUnidades;

    [SerializeField] private GameObject canvasObject;
    [NonSerialized] public CampaignMapUI_Manager canvas;

    [NonSerialized] public int miTurno;
    [NonSerialized] [SyncVar] public int turnoActual = 0;

    private Color colorOriginal = new Color(1f, 1f, 1f);
    private Color colorSeleccionado = new Color(.7f, .7f, .7f);
    private Color colorLimitrofe = new Color(1f, 0f, 0f);

    void Start()
    {
        instancia = this;

        Instantiate(canvasObject);
        canvas = canvasObject.GetComponent<CampaignMapUI_Manager>();
        canvas.ShowCancelAttackButton(false);
        canvas.ShowGarrisonPanel(false);
        canvas.ShowToBattlePanel(false);
        canvas.ShowVsFramesPanel(false);
        canvas.ShowAttackerButton(false);
        canvas.ShowDefenderButton(false);
        
        canvas.ShowEndPhaseButton(false);
        canvas.ShowEndTurnButton(false);
    }

    void Update()
    {
        if (miTurno != turnoActual) { return; }
    }

    public void DesplegarMenuAtaque(Sprite imagenJugador, Sprite imagenEnemigo, string paisJugador, string paisEnemigo, int tipoJugador)
    {
        canvas.ShowEndPhaseButton(false);

        // instanciar boton cancelar ataque en caso de ser atacante
        if (tipoJugador == 1)
            canvas.ShowCancelAttackButton(true);

        canvas.ShowGarrisonPanel(true);
        canvas.ShowToBattlePanel(true);

        canvas.ShowVsFramesPanel(true);

        canvas.ChangePlayerImage(imagenJugador);
        canvas.ChangeEnemyImage(imagenEnemigo);

        canvas.ChangePlayerCountry(paisJugador);
        canvas.ChangeEnemyCountry(paisEnemigo);

        // instanciar boton defensor
        if (tipoJugador == 0)
            canvas.ShowDefenderButton(true);
        // instanciar boton atacante
        else if (tipoJugador == 1)
            canvas.ShowAttackerButton(true);
    }

    public void OcultarMenuAtaque()
    {
        canvas.ShowCancelAttackButton(false);
        canvas.ShowGarrisonPanel(false);
        canvas.ShowToBattlePanel(false);
        canvas.ShowVsFramesPanel(false);
        canvas.ShowAttackerButton(false);
        canvas.ShowDefenderButton(false);

        if (miTurno == turnoActual)
            canvas.ShowEndPhaseButton(true);
    }

    public void ActualizarEstadoPaises(string nombrePaisSeleccionado, string[] nombrePaisesLimitrofes)
    {
        // comprueba si hay un pais seleccionado, en cuyo caso lo deselecciona
        if (HayPaisSeleccionado()) { return; }

        GameObject paisSeleccionado = GameObject.Find(nombrePaisSeleccionado);
        paisSeleccionado.GetComponent<SpriteRenderer>().color = colorSeleccionado;
        paisSeleccionado.tag = "Selected";

        foreach (string nombrePaisLimitrofe in nombrePaisesLimitrofes)
        {
            GameObject paisLimitrofe = GameObject.Find(nombrePaisLimitrofe);
            paisLimitrofe.GetComponent<SpriteRenderer>().color = colorLimitrofe;
            paisLimitrofe.tag = "Bordering";
        }
    }

    public bool HayPaisSeleccionado()
    {
        GameObject posibleSeleccionado = GameObject.FindGameObjectWithTag("Selected");
        GameObject[] posiblesLimitrofes = GameObject.FindGameObjectsWithTag("Bordering");

        if (posibleSeleccionado == null) { return false; }
        
        // deselecciona al pais
        posibleSeleccionado.GetComponent<SpriteRenderer>().color = colorOriginal;
        posibleSeleccionado.tag = "Untagged";

        // y a sus limitrofes
        foreach (GameObject posibleLimitrofe in posiblesLimitrofes)
        {
            if (posibleLimitrofe == null) { continue; }

            posibleLimitrofe.GetComponent<SpriteRenderer>().color = colorOriginal;
            posibleLimitrofe.tag = "Untagged";
        }

        return true;
    }
}
