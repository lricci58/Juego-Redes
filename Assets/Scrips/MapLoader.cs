using System;
using System.Collections.Generic;
using UnityEngine;
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

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float tileSize;
    private Vector3 initialPosition;

    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private LayerMask unitLayer;

    // almacenan las listas de prefabs
    public GameObject[] unitPrefabs;
    public GameObject[] mapPrefabs;
    public GameObject[] riverPrefabs;
    public GameObject[] wallPrefabs;

    [NonSerialized] public int[] unitTypesList;

    public void SetScene()
    {
        // @TODO: setear las dos zonas de despliegue

        // setea el offset de la grilla
        initialPosition = new Vector3(-tileSize * 8, -tileSize * 6);

        InstantiateScene();
        InstantiateUnits();

        movementTilesList = new List<GameObject>();
        attackTilesList = new List<GameObject>();
        tileContainer = new GameObject("TileContainer").transform;
    }

    private void InstantiateScene()
    {
        // @TODO: realizar el sistema de cargado de mapas aleatoreo

        // elije que conjunto de elementos se va a cargar (temp)
        int indiceRandom = Random.Range(0, mapPrefabs.GetLength(0));

        mapContainer = new GameObject("MapContainer").transform;

        // crea el escenario con objetos random (temp)
        InstantiateFromArray(mapPrefabs, indiceRandom, mapContainer);
        InstantiateFromArray(riverPrefabs, indiceRandom, mapContainer);
        InstantiateFromArray(wallPrefabs, indiceRandom, mapContainer);
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
        {
            try {
                // ControladorConexion.instancia.CmdSpawnObjeto(listaUnidades[i]);
                InstantiateFromArray(unitPrefabs, unitTypesList[i], unitContainer);
            }
            catch (IndexOutOfRangeException) {
                Debug.LogError("unit not found... [MapLoader -> InstantiateUnits() : void] || Iteration nº"+i);
            }
        }
    }

    private void InstantiateFromArray(GameObject[] prefabArray, int index, Transform parentContainer)
    {
        // instancia el objeto en la posicion elejida
        GameObject originalPrefab = prefabArray[index];
        GameObject instance = Instantiate(originalPrefab, originalPrefab.transform.position, Quaternion.identity);

        // establece al contenedor como padre del objeto
        instance.transform.SetParent(parentContainer);
    }

    public List<Vector2> SetCollisionTiles(List<Vector2> tilePositions, int unitXPosition, int unitYPosition, int movementRadius)
    {
        Vector2 unitPos = new Vector2(unitXPosition, unitYPosition);
        List<Vector2> validTilesList = new List<Vector2>();
        validTilesList.AddRange(tilePositions);

        foreach (Vector2 tilePos in tilePositions)
        {
            // @TODO: usar la posicion de la  unidad como "centro de tile"
            Vector3 worldPos = GetTileCenter((int)tilePos.x, (int)tilePos.y);

            // comprueba si la posicion colisiona con algun colider en la layer de colision
            if (Physics2D.OverlapPoint(worldPos, collisionLayer) || Physics2D.OverlapPoint(worldPos, unitLayer))
            {
                // para evitar errores, por si el tile ya se habia borrado antes
                if (validTilesList.Contains(tilePos))
                {
                    if (tilePos.y > unitPos.y)
                        // borra todos los tiles desde el invalido en adelante
                        for (float posY = tilePos.y; posY <= unitPos.y + movementRadius; posY++)
                            validTilesList.Remove(new Vector2(tilePos.x, posY));

                    if (tilePos.y < unitPos.y)
                        for (float posY = tilePos.y; posY >= unitPos.y - movementRadius; posY--)
                            validTilesList.Remove(new Vector2(tilePos.x, posY));

                    if (tilePos.x > unitPos.x)
                        for (float posX = tilePos.x; posX <= unitPos.x + movementRadius; posX++)
                            validTilesList.Remove(new Vector2(posX, tilePos.y));

                    if (tilePos.x < unitPos.x)
                        for (float posX = tilePos.x; posX >= unitPos.x - movementRadius; posX--)
                            validTilesList.Remove(new Vector2(posX, tilePos.y));
                }
            }
        }

        return validTilesList;
    }

    public List<Vector2> SetAttackTiles(List<Vector2> tilePositions, int unitXPosition, int unitYPosition)
    {
        Vector2 unitPosition = new Vector2(unitXPosition, unitYPosition);
        List<Vector2> attackTiles = new List<Vector2>();

        foreach (Vector2 posTile in tilePositions)
        {
            Vector2 tileToCheck;
            Vector3 worldPos;

            // this... is... gross... :( REDO

            tileToCheck = new Vector2(posTile.x + 1, posTile.y);
            worldPos = GetTileCenter((int)tileToCheck.x, (int)tileToCheck.y);
            if (!tilePositions.Contains(tileToCheck) && !attackTiles.Contains(tileToCheck) && tileToCheck != unitPosition)
                if (!Physics2D.OverlapPoint(worldPos, collisionLayer))
                    attackTiles.Add(tileToCheck);

            tileToCheck = new Vector2(posTile.x - 1, posTile.y);
            worldPos = GetTileCenter((int)tileToCheck.x, (int)tileToCheck.y);
            if (!tilePositions.Contains(tileToCheck) && !attackTiles.Contains(tileToCheck) && tileToCheck != unitPosition)
                if (!Physics2D.OverlapPoint(worldPos, collisionLayer))
                    attackTiles.Add(tileToCheck);

            tileToCheck = new Vector2(posTile.x, posTile.y + 1);
            worldPos = GetTileCenter((int)tileToCheck.x, (int)tileToCheck.y);
            if (!tilePositions.Contains(tileToCheck) && !attackTiles.Contains(tileToCheck) && tileToCheck != unitPosition)
                if (!Physics2D.OverlapPoint(worldPos, collisionLayer))
                    attackTiles.Add(tileToCheck);

            tileToCheck = new Vector2(posTile.x, posTile.y - 1);
            worldPos = GetTileCenter((int)tileToCheck.x, (int)tileToCheck.y);
            if (!tilePositions.Contains(tileToCheck) && !attackTiles.Contains(tileToCheck) && tileToCheck != unitPosition)
                if (!Physics2D.OverlapPoint(worldPos, collisionLayer))
                    attackTiles.Add(tileToCheck);
        }

        attackTiles.Remove(unitPosition);

        return attackTiles;
    }

    public void InstantiateMovementTiles(List<Vector2> tilePositions)
    {
        foreach (Vector2 tilePos in tilePositions)
        {
            // obtiene la posicion en el mundo del tile
            Vector3 worldPos = GetWorldPos((int)tilePos.x, (int)tilePos.y);
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
            Vector3 worldPos = GetWorldPos((int)tilePos.x, (int)tilePos.y);
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

    public Vector3 GetWorldPos(int x, int y)
    {
        return new Vector3(x, y) * tileSize + initialPosition;
    }

    public bool GetGridTile(Vector3 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPos - initialPosition).x / tileSize);
        y = Mathf.FloorToInt((worldPos - initialPosition).y / tileSize);

        if (x >= 0 && y >= 0 && x < width && y < height)
            return true;

        return false;
    }

    public Vector3 GetTileCenter(int x, int y)
    {
        return GetWorldPos(x, y) + new Vector3(tileSize, tileSize) * .5f;
    }
}