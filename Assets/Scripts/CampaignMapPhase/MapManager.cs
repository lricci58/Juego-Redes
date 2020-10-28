using System;
using UnityEngine;
using Mirror;

public class MapManager : NetworkBehaviour
{
    public static MapManager instancia;

    [SerializeField] private GameObject textNumUnidades;

    private Color colorOriginal = new Color(1f, 1f, 1f);
    private Color colorSeleccionado = new Color(.7f, .7f, .7f);
    private Color colorLimitrofe = new Color(1f, 0f, 0f);

    void Start() => instancia = this;

    [NonSerialized] public int miTurno;

    [NonSerialized] [SyncVar] public int turnoActual = 0;

    void Update()
    {
        if (miTurno != turnoActual) { return; }
    }

    public void ActualizarEstadoPaises(string nombrePaisSeleccionado, string[] nombrePaisesLimitrofes)
    {
        GameObject posibleSeleccionado = GameObject.FindGameObjectWithTag("Selected");
        GameObject[] posiblesLimitrofes = GameObject.FindGameObjectsWithTag("Bordering");

        // primero comprueba si hay algun pais que ya este seleccionado con sus limitrofes
        if (posibleSeleccionado != null)
        {
            posibleSeleccionado.GetComponent<SpriteRenderer>().color = colorOriginal;
            posibleSeleccionado.tag = "Untagged";

            foreach (GameObject posibleLimitrofe in posiblesLimitrofes)
            {
                if (posibleLimitrofe != null)
                {
                    posibleLimitrofe.GetComponent<SpriteRenderer>().color = colorOriginal;
                    posibleLimitrofe.tag = "Untagged";
                }
            }

            // que vuelva porque ya habia un pais seleccionado
            return;
        }

        // luego de "deseleccionar" el anterior pais, se ocupa de "seleccionar" el pais debido

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
}
