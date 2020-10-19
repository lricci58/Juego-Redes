using UnityEngine;
using Mirror;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public static MapManager instancia;

    private Color colorDeseleccionado = new Color(1, 1, 1);
    private Color colorSeleccionado = new Color(0.7f, 0.7f, 0.7f);
    private Color limitrofe = new Color(1f, 0f, 0f);

    void Start() => instancia = this;

    public void ActualizarEstadoPaises(string selectedCountryName, string[] borderingCountryName)
    {
        foreach (string countryName in borderingCountryName)
            GameObject.Find(countryName).GetComponent<SpriteRenderer>().color = limitrofe;

        GameObject.Find(selectedCountryName).GetComponent<SpriteRenderer>().color = colorSeleccionado;
    }
}
