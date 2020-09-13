using UnityEngine;

public class Grilla
{
    // En este script se debe crear la grilla y detectar la posicion en la que el mouse presiona

    private int ancho;
    private int alto;
    private float dimensionTile = 128f;
    private Vector3 posicionOriginal;
    private int[,] listaTiles;

    public Grilla(int ancho, int alto, float dimensionTile, Vector3 posicionOriginal)
    {
        this.ancho = ancho;
        this.alto = alto;
        this.dimensionTile = dimensionTile;
        this.posicionOriginal = posicionOriginal;

        listaTiles = new int[ancho, alto];

        for (int x = 0; x < listaTiles.GetLength(0); x++)
        {
            for (int y = 0; y < listaTiles.GetLength(1); y++)
            {
                // dibuja la grilla
                Debug.DrawLine(ObtenerPosMundo(x,y), ObtenerPosMundo(x, y + 1), Color.white, 100f);
                Debug.DrawLine(ObtenerPosMundo(x, y), ObtenerPosMundo(x + 1, y), Color.white, 100f);
            }
        }
        Debug.DrawLine(ObtenerPosMundo(ancho, 0), ObtenerPosMundo(ancho, alto), Color.white, 100f);
        Debug.DrawLine(ObtenerPosMundo(0, alto), ObtenerPosMundo(ancho, alto), Color.white, 100f);
    }

    public Vector3 ObtenerPosMundo(int x, int y)
    {
        return new Vector3(x, y) * dimensionTile + posicionOriginal;
    }

    public bool DetectarClick(Vector3 mousePosMundo, out int x, out int y)
    {
        x = Mathf.FloorToInt((mousePosMundo - posicionOriginal).x / dimensionTile);
        y = Mathf.FloorToInt((mousePosMundo - posicionOriginal).y / dimensionTile);

        if (x >= 0 && y >= 0 && x < ancho && y < alto)
            return true;
        
        return false;
    }
}
