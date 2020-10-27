using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapLoader : MonoBehaviour
{
    // se usa para contener a todos los objetos del juego y dejar limpia la hierarchy
    private Transform mapContainer;
    private Transform tileContainer;
    [NonSerialized] public Transform unitContainer;
    [NonSerialized] public Transform allyUnitContainer;
    [NonSerialized] public Transform enemyUnitContainer;

    [SerializeField] private GameObject movementTile;
    [SerializeField] private GameObject attackTile;
    private List<GameObject> movementTilesList;
    private List<GameObject> attackTilesList;

    public LayerMask unitsLayer;

    // almacenan las listas de prefabs
    public GameObject[] unitPrefabs;
    public GameObject[] smallMapPrefabs;
    public GameObject[] mediumMapPrefabs;
    public GameObject[] bigMapPrefabs;
    public GameObject[] largeMapPrefabs;

    [NonSerialized] public Tilemap ground;
    [NonSerialized] public Tilemap collision;
    [NonSerialized] public Tilemap deployZone;

    [NonSerialized] public int[] unitTypesList;

    public void SetScene()
    {
        int randomMap = Random.Range(0, 3);

        InstantiateMap(smallMapPrefabs);

        // instancia un mapa de tamaño aleatoreo
        //if (randomMap == 0 && smallMapPrefabs.GetLength(0) > 0)
        //    InstantiateMap(smallMapPrefabs);
        //else if(randomMap == 1 && mediumMapPrefabs.GetLength(0) > 0)
        //    InstantiateMap(mediumMapPrefabs);
        //else if (randomMap == 2 && bigMapPrefabs.GetLength(0) > 0)
        //    InstantiateMap(bigMapPrefabs);
        //else if (randomMap == 3 && largeMapPrefabs.GetLength(0) > 0)
        //    InstantiateMap(largeMapPrefabs);

        InstantiateUnits();

        movementTilesList = new List<GameObject>();
        attackTilesList = new List<GameObject>();
        tileContainer = new GameObject("TileContainer").transform;
    }

    private void InstantiateMap(GameObject[] mapPrefabsToInstantiate)
    {
        // elije un mapa de la lista de cargados
        int randomMap = Random.Range(0, mapPrefabsToInstantiate.GetLength(0));

        mapContainer = new GameObject("MapContainer").transform;

        InstantiateFromArray(mapPrefabsToInstantiate, randomMap, mapContainer);

        // si es un espectador, no hace nada
        if (GameManager.instance.playerBattleSide == 2) { return; }

        // obtiene cada capa del tilemap
        ground = GameObject.Find("AvailableGround").GetComponent<Tilemap>();
        collision = GameObject.Find("Collision").GetComponent<Tilemap>();
        string deployZone1 = "";
        string deployZone2 = "";
        if (GameManager.instance.playerBattleSide == 0)
        {
            deployZone1 = "DeployZone1";
            deployZone2 = "DeployZone2";
        }
        if (GameManager.instance.playerBattleSide == 1)
        {
            deployZone1 = "DeployZone2";
            deployZone2 = "DeployZone1";
        }

        deployZone = GameObject.Find(deployZone1).GetComponent<Tilemap>();
        GameObject.Find(deployZone2).SetActive(false);
    }

    private void InstantiateUnits()
    {
        unitTypesList = GameManager.instance.unitList;
        
        unitContainer = new GameObject("UnitContainer").transform;
        allyUnitContainer = new GameObject("AlliedUnitsContainer").transform;
        enemyUnitContainer = new GameObject("EnemyUnitsContainer").transform;
        
        allyUnitContainer.SetParent(unitContainer);
        enemyUnitContainer.SetParent(unitContainer);

        // recorre la lista de nombres, instanciando las unidades segun el nombre
        for (int i = 0; i < unitTypesList.GetLength(0); i++)
            InstantiateFromArray(unitPrefabs, unitTypesList[i], unitContainer);
    }

    private void InstantiateFromArray(GameObject[] prefabArray, int index, Transform parentContainer)
    {
        // instancia el objeto en la posicion elejida
        GameObject originalPrefab = prefabArray[index];
        GameObject instance = Instantiate(originalPrefab, originalPrefab.transform.position, Quaternion.identity);

        // establece al contenedor como padre del objeto
        instance.transform.SetParent(parentContainer);
    }

    public void InstantiateMovementTiles(List<Vector2> tilePositions)
    {
        foreach (Vector2 tilePos in tilePositions)
        {
            // obtiene la posicion en el mundo del tile
            Vector3 worldPos = GetWorldPos(new Vector3Int((int)tilePos.x, (int)tilePos.y, 0));
            worldPos.x += 64;
            worldPos.y += 64;
            worldPos.z = movementTile.transform.position.z;

            GameObject instancia = Instantiate(movementTile, worldPos, Quaternion.identity);
            movementTilesList.Add(instancia);

            instancia.transform.SetParent(tileContainer);
        }
    }

    public void InstantiateAttackTiles(List<Vector2> tilePositions)
    {
        foreach (Vector2 tilePos in tilePositions)
        {
            // obtiene la posicion en el mundo del tile
            Vector3 worldPos = GetWorldPos(new Vector3Int((int)tilePos.x, (int)tilePos.y, 0));
            worldPos.x += 64;
            worldPos.y += 64;
            worldPos.z = attackTile.transform.position.z;

            GameObject instancia = Instantiate(attackTile, worldPos, Quaternion.identity);
            attackTilesList.Add(instancia);

            instancia.transform.SetParent(tileContainer);
        }
    }

    public void DestroyTiles()
    {
        foreach (GameObject movementTile in movementTilesList)
            Destroy(movementTile);

        foreach (GameObject attackTile in attackTilesList)
            Destroy(attackTile);

        movementTilesList.Clear();
        attackTilesList.Clear();
    }

    public bool CanMoveToPos(Vector3 worldPos)
    {
        Vector3Int gridPos = ground.WorldToCell(worldPos);

        if (!ground.HasTile(gridPos) || collision.HasTile(gridPos))
            return false;

        return true;
    }

    public bool CanDeployInPos(Vector3Int gridPos)
    {
        if (!deployZone.HasTile(gridPos))
            return false;

        return true;
    }

    public Vector3 GetWorldPos(Vector3Int gridPos)
    {
        return ground.CellToWorld(gridPos);
    }

    public Vector3Int GetGridTile(Vector3 worldPos)
    {
        return ground.WorldToCell(worldPos);
    }
}