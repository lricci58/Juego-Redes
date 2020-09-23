using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ControladorMapa : MonoBehaviour
{
    // almacenan las listas de prefabs
    [SerializeField] private GameObject[] unidades;
    [SerializeField] private GameObject[] mapas;
    [SerializeField] private GameObject[] rios;
    [SerializeField] private GameObject[] muros;

    [SerializeField] private LayerMask layerColision;
    [SerializeField] private LayerMask layerUnidades;

    // se usa para contener a todos los objetos del juego y dejar limpia la hierarchy
    private Transform contenedorMapa;
    private Transform contenedorUnidades;
    private Transform contenedorTiles;

    [SerializeField] private GameObject tileMovimiento;
    [SerializeField] private GameObject tileAtaque;
    private List<GameObject> listaTilesMovimiento;
    private List<GameObject> listaTilesAtaque;

    [SerializeField] private int ancho;
    [SerializeField] private int alto;
    [SerializeField] private float dimensionTile;
    private Vector3 posicionOriginal;

    // @NOTE: esta lista de unidades vendria desde la escena de mapa general
    private string[] unidadesEjercito;

    public void CrearEscenario()
    {
        // @TODO: setear las zonas de despliegue

        contenedorMapa = new GameObject("ContenedorDeMapa").transform;
        InstanciarEscenario();

        listaTilesMovimiento = new List<GameObject>();
        listaTilesAtaque = new List<GameObject>();

        contenedorTiles = new GameObject("ContenedorDeTilesDisponibles").transform;
        contenedorTiles.SetParent(contenedorMapa);
        
        // setea el offset de la grilla
        posicionOriginal = new Vector3(-dimensionTile*8, -dimensionTile*6);

        contenedorUnidades = new GameObject("ContenedorDeUnidades").transform;
        contenedorUnidades.SetParent(contenedorMapa);
        
        unidadesEjercito = new string[3];
        unidadesEjercito[0] = "InfanteriaHacha";
        unidadesEjercito[1] = "InfanteriaHacha";
        unidadesEjercito[2] = "InfanteriaHacha";
        InstanciarUnidades(unidadesEjercito);
    }

    private void InstanciarEscenario()
    {
        // @NOTE: revisar! puede que alguno de los conjuntos no lleve uno de los componentes (rios, muros, etc)

        // elije (random) que conjunto de elementos se va a cargar (temp)
        int indiceRandom = Random.Range(0, mapas.GetLength(0));

        // crea el escenario con objetos random (temp)
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

    public List<Vector2> DeterminarTilesInvalidos(List<Vector2> posicionesTiles, int posUnidadX, int posUnidadY, int radioMovimiento)
    {
        Vector2 posUnidad = new Vector2(posUnidadX, posUnidadY);
        List<Vector2> listaValidos = new List<Vector2>();
        listaValidos.AddRange(posicionesTiles);

        foreach (Vector2 posTile in posicionesTiles)
        {
            // @TODO: usar la posicion de la  unidad como "centro de tile"
            Vector3 posMundo = ObtenerCentroTile((int)posTile.x, (int)posTile.y);

            // comprueba si la posicion colisiona con algun colider en la layer de colision
            if (Physics2D.OverlapPoint(posMundo, layerColision) || Physics2D.OverlapPoint(posMundo, layerUnidades))
            {
                // para evitar errores, por si el tile ya se habia borrado antes
                if (listaValidos.Contains(posTile))
                {
                    if (posTile.y > posUnidad.y)
                        // borra todos los tiles desde el invalido en adelante
                        for (float posY = posTile.y; posY <= posUnidad.y + radioMovimiento; posY++)
                            listaValidos.Remove(new Vector2(posTile.x, posY));
                    
                    if (posTile.y < posUnidad.y)
                        for (float posY = posTile.y; posY >= posUnidad.y - radioMovimiento; posY--)
                            listaValidos.Remove(new Vector2(posTile.x, posY));
                    
                    if (posTile.x > posUnidad.x)
                        for (float posX = posTile.x; posX <= posUnidad.x + radioMovimiento; posX++)
                            listaValidos.Remove(new Vector2(posX, posTile.y));
                    
                    if (posTile.x < posUnidad.x)
                        for (float posX = posTile.x; posX >= posUnidad.x - radioMovimiento; posX--)
                            listaValidos.Remove(new Vector2(posX, posTile.y));
                }
            }
        }

        return listaValidos;
    }

    public List<Vector2> DeterminarTilesAtaque(List<Vector2> posicionesTiles, int posUnidadX, int posUnidadY)
    {
        Vector2 posUnidad = new Vector2(posUnidadX, posUnidadY);
        List<Vector2> tilesAtaque = new List<Vector2>();

        foreach (Vector2 posTile in posicionesTiles)
        {
            Vector2 tileAComprobar;
            Vector3 posMundo;

            tileAComprobar = new Vector2(posTile.x + 1, posTile.y);
            posMundo = ObtenerCentroTile((int)tileAComprobar.x, (int)tileAComprobar.y);
            if (!posicionesTiles.Contains(tileAComprobar) && !tilesAtaque.Contains(tileAComprobar) && tileAComprobar != posUnidad)
                if(!Physics2D.OverlapPoint(posMundo, layerColision))
                    tilesAtaque.Add(tileAComprobar);

            tileAComprobar = new Vector2(posTile.x - 1, posTile.y);
            posMundo = ObtenerCentroTile((int)tileAComprobar.x, (int)tileAComprobar.y);
            if (!posicionesTiles.Contains(tileAComprobar) && !tilesAtaque.Contains(tileAComprobar) && tileAComprobar != posUnidad)
                if (!Physics2D.OverlapPoint(posMundo, layerColision))
                    tilesAtaque.Add(tileAComprobar);

            tileAComprobar = new Vector2(posTile.x, posTile.y + 1);
            posMundo = ObtenerCentroTile((int)tileAComprobar.x, (int)tileAComprobar.y);
            if (!posicionesTiles.Contains(tileAComprobar) && !tilesAtaque.Contains(tileAComprobar) && tileAComprobar != posUnidad)
                if (!Physics2D.OverlapPoint(posMundo, layerColision))
                    tilesAtaque.Add(tileAComprobar);

            tileAComprobar = new Vector2(posTile.x, posTile.y - 1);
            posMundo = ObtenerCentroTile((int)tileAComprobar.x, (int)tileAComprobar.y);
            if (!posicionesTiles.Contains(tileAComprobar) && !tilesAtaque.Contains(tileAComprobar) && tileAComprobar != posUnidad)
                if (!Physics2D.OverlapPoint(posMundo, layerColision))
                    tilesAtaque.Add(tileAComprobar);
        }

        tilesAtaque.Remove(posUnidad);

        return tilesAtaque;
    }

    public void InstanciarTilesMovimiento(List<Vector2> listaPosicionesTiles)
    {
        foreach (Vector2 posTile in listaPosicionesTiles)
        {
            // obtiene la posicion en el mundo del tile
            Vector3 posTileEnMundo = ObtenerPosMundo((int)posTile.x, (int)posTile.y);
            posTileEnMundo.x += 64;
            posTileEnMundo.y += 64;
            posTileEnMundo.z = tileMovimiento.transform.position.z;

            GameObject instancia = Instantiate(tileMovimiento, posTileEnMundo, Quaternion.identity);
            listaTilesMovimiento.Add(instancia);

            instancia.transform.SetParent(contenedorTiles);
        }
    }

    public void InstanciarTilesAtaque(List<Vector2> listaPosicionesTiles)
    {
        foreach (Vector2 posTile in listaPosicionesTiles)
        {
            // obtiene la posicion en el mundo del tile
            Vector3 posTileEnMundo = ObtenerPosMundo((int)posTile.x, (int)posTile.y);
            posTileEnMundo.x += 64;
            posTileEnMundo.y += 64;
            posTileEnMundo.z = tileAtaque.transform.position.z;

            GameObject instancia = Instantiate(tileAtaque, posTileEnMundo, Quaternion.identity);
            listaTilesAtaque.Add(instancia);

            instancia.transform.SetParent(contenedorTiles);
        }
    }

    public void DestruirTiles()
    {
        foreach (GameObject tileMovimiento in listaTilesMovimiento)
            Destroy(tileMovimiento);

        foreach (GameObject tileAtaque in listaTilesAtaque)
            Destroy(tileAtaque);

        listaTilesMovimiento.Clear();
        listaTilesAtaque.Clear();
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

    public Vector3 ObtenerCentroTile(int x, int y)
    {
        return ObtenerPosMundo(x, y) + new Vector3(dimensionTile, dimensionTile) * .5f;
    }
}