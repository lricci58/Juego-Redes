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

    public void CrearEscenario()
    {
        // @TO keep DOing: setear los obstaculos del mapa, la grilla y las zonas de despliegue

        // crea el contenedor, un objeto con el nombre "ContenedorDePrefabsDeMapa"
        contenedorMapa = new GameObject("ContenedorDePrefabsDeMapa").transform;

        // obtiene un conjunto random de objetos del escenario
        ObtenerEscenario();

        // esta lista de unidades vendria desde la escena de mapa general
        string[] unidadesEjercito = new string[1];
        unidadesEjercito[0] = "InfanteriaHacha";

        // instancia las unidades de cada ejercito
        InstanciarUnidades(unidadesEjercito);
    }

    private void ObtenerEscenario()
    {
        // elije (random) que conjunto de elementos se va a cargar
        int indiceRandom = Random.Range(0, mapas.GetLength(0));

        // crea el escenario con objetos random (temporal)
        InstanciarDesdeArray(mapas, indiceRandom);
        InstanciarDesdeArray(rios, indiceRandom);
        InstanciarDesdeArray(muros, indiceRandom);
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
                InstanciarDesdeArray(unidades, posicionTipoUnidad);
            }
            // en caso de que el argumento no sea valido
            catch {
                Debug.LogError("No se encontro la unidad (" + listaNombresUnidad[i] + ") que se desea instanciar... Controlador Mapa -> InstanciarUnidad() : GameObject");
            }
        }
    }

    private void InstanciarDesdeArray(GameObject[] arrayObjetos, int indice)
    {
        // instancia el objeto en la posicion elejida
        GameObject objetoAInstanciar = arrayObjetos[indice];
        GameObject instancia = Instantiate(objetoAInstanciar, objetoAInstanciar.transform.position, Quaternion.identity);

        // establece al contenedor como padre del objeto
        instancia.transform.SetParent(contenedorMapa);
    }
}