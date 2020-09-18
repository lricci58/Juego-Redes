using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ControladorMapa : MonoBehaviour
{
    // almacenan las listas de prefabs
    public GameObject[] unidades;
    public GameObject[] mapas;
    public GameObject[] rios;
    public GameObject[] muros;

    // se usa para contener a todos los objetos del juego y dejar limpia la hierarchy
    private Transform contenedorMapa;
    private Transform contenedorUnidades;
    private Transform contenedorTiles;

    [SerializeField] private GameObject tileMovimiento;
    private List<GameObject> listaTilesDeMovimiento;

    [SerializeField] private int ancho = 16;
    [SerializeField] private int alto = 12;
    [SerializeField] private float dimensionTile = 127f;
    private Vector3 posicionOriginal;

    // @NOTE: esta lista de unidades vendria desde la escena de mapa general
    private string[] unidadesEjercito;

    public void CrearEscenario()
    {
        // @TODO: setear los obstaculos del mapa y las zonas de despliegue

        contenedorMapa = new GameObject("ContenedorDeMapa").transform;
        InstanciarEscenario();

        listaTilesDeMovimiento = new List<GameObject>();
        contenedorTiles = new GameObject("ContenedorDeTilesDisponibles").transform;
        contenedorTiles.SetParent(contenedorMapa);
        
        // setea el offset de la grilla
        posicionOriginal = new Vector3(-dimensionTile*8, -dimensionTile*6);

        contenedorUnidades = new GameObject("ContenedorDeUnidades").transform;
        contenedorUnidades.SetParent(contenedorMapa);
        
        unidadesEjercito = new string[1];
        unidadesEjercito[0] = "InfanteriaHacha";
        InstanciarUnidades(unidadesEjercito);
    }

    private void InstanciarEscenario()
    {
        // elije (random) que conjunto de elementos se va a cargar
        int indiceRandom = Random.Range(0, mapas.GetLength(0));

        // crea el escenario con objetos random (temporal)
        InstanciarDesdeArray(mapas, indiceRandom, contenedorMapa);
        InstanciarDesdeArray(rios, indiceRandom, contenedorMapa);
        InstanciarDesdeArray(muros, indiceRandom, contenedorMapa);
    }

    public void InstanciarUnidades(string[] listaNombresUnidad)
    {
        // recorre la lista de nombres, instanciando las unidades segun el nombre
        for (int i = 0; i < listaNombresUnidad.GetLength(0); i++)
        {
            int posicionTipoUnidad = -1;

            if (listaNombresUnidad[i] == "InfanteriaHacha")
                posicionTipoUnidad = 0;
            
            else if (listaNombresUnidad[i] == "Infanteria_1")
                posicionTipoUnidad = 1;
            
            else if (listaNombresUnidad[i] == "Infanteria_2")
                posicionTipoUnidad = 2;

            try {
                InstanciarDesdeArray(unidades, posicionTipoUnidad, contenedorUnidades);
            }
            // en caso de que el argumento no sea valido
            catch {
                Debug.LogError("No se encontro la unidad (" + listaNombresUnidad[i] + ") que se desea instanciar... Controlador Mapa -> InstanciarUnidad() : GameObject");
            }
        }
    }

    private void InstanciarDesdeArray(GameObject[] arrayObjetos, int indice, Transform contenedorPadre)
    {
        // instancia el objeto en la posicion elejida
        GameObject objetoAInstanciar = arrayObjetos[indice];
        GameObject instancia = Instantiate(objetoAInstanciar, objetoAInstanciar.transform.position, Quaternion.identity);

        // establece al contenedor como padre del objeto
        instancia.transform.SetParent(contenedorPadre);
    }

    public void InstanciarTilesDeMovimiento(List<Vector2> listaPosicionesTiles)
    {
        // Instancia los tiles disponibles en las posiciones del parametro

        foreach (Vector2 posTile in listaPosicionesTiles)
        {
            // obtiene la posicion en el mundo del tile
            Vector3 posTileEnMundo = ObtenerPosMundo((int)posTile.x, (int)posTile.y);
            posTileEnMundo.x += 64;
            posTileEnMundo.y += 64;
            posTileEnMundo.z = tileMovimiento.transform.position.z;

            GameObject instancia = Instantiate(tileMovimiento, posTileEnMundo, Quaternion.identity);
            listaTilesDeMovimiento.Add(instancia);
            
            instancia.transform.SetParent(contenedorTiles);
        }
    }

    public void DestruirTilesDeMovimiento(List<Vector2> listaPosicionesTiles)
    {
        foreach (GameObject tileDeMovimiento in listaTilesDeMovimiento)
            Destroy(tileDeMovimiento);

        listaPosicionesTiles.Clear();
        listaTilesDeMovimiento.Clear();
    }

    public Vector3 ObtenerPosMundo(int x, int y)
    {
        return new Vector3(x, y) * dimensionTile + posicionOriginal;
    }

    public bool ObtenerPosGrilla(Vector3 posMundo, out int x, out int y)
    {
        x = Mathf.FloorToInt((posMundo - posicionOriginal).x / dimensionTile);
        y = Mathf.FloorToInt((posMundo - posicionOriginal).y / dimensionTile);

        if (x >= 0 && y >= 0 && x < ancho && y < alto)
            return true;

        return false;
    }
}