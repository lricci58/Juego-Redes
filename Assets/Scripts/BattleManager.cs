using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : NetworkBehaviour
{
    public static BattleManager instance = null;

    public MapLoader map;
    [SerializeField] private GameObject canvasObject;
    [NonSerialized] public UI_Manager canvas;

    [NonSerialized] public List<UnitScript> army;
    private List<UnitScript> deployUnitList;
    private List<UnitScript> enemyArmy;
    private Vector3 worldPos;
    private UnitScript selectedUnit;
    private UnitScript targetUnit;
    private bool selectingTile = false;

    private int deployUnitIndex = -1;
    [NonSerialized] public bool deployFase = true;
    private bool battleFase = false;
    [NonSerialized] [SyncVar] public int endedDeployFaseCount = 0;
    [NonSerialized] [SyncVar] public int turnNumber = 0;
    [NonSerialized] public int myTurnNumber;

    void Start()
    {
        instance = this;

        myTurnNumber = GameManager.instance.playerBattleSide;

        army = new List<UnitScript>();
        deployUnitList = new List<UnitScript>();
        // lista para todas las unidades, aliadas y enemigas
        enemyArmy = new List<UnitScript>();

        InitGame();
    }

    private void InitGame()
    {
        Instantiate(canvasObject);
        canvas = canvasObject.GetComponent<UI_Manager>();
        canvas.ShowDeploymentPanel(true);
        canvas.ShowStartBattleButton(true);
        canvas.ShowEndTurnButton(false);
        canvas.ShowWaitingText(false);
        
        map = GetComponent<MapLoader>();
        map.SetScene();
    }

    public void UnitInstantiated(UnitScript unit)
    {
        if (battleFase)
        {
            enemyArmy.Add(unit);
            // setea el contenedor padre de las unidades
            unit.transform.SetParent(map.unitContainer);
            unit.transform.SetParent(map.enemyUnitContainer);

            // comprueba si el cliente local es dueño de la unidad instanciada
            ConnectionManager.instance.CmdCheckUnitOwner(unit.GetComponent<NetworkIdentity>());
        }
        else
        {
            deployUnitList.Add(unit);
            unit.transform.SetParent(map.allyUnitContainer);

            unit.gameObject.SetActive(false);
        }
    }

    public void AddUnitToArmy(UnitScript unit)
    {
        enemyArmy.Remove(unit);
        army.Add(unit);
        unit.transform.SetParent(map.allyUnitContainer);
    }

    void Update()
    {
        if (!ConnectionManager.instance.isLocalPlayer) { return; }
        if (!ConnectionManager.instance.hasAuthority) { return; }

        BattleFaseManager();

        DeployFaseManager();

        // comprueba que los 2 jugadores hayan desplegado
        if (EveryoneDeployed() && !battleFase)
            DeployFaseEnded();
    }

    #region ControlJuego

    private void BattleFaseManager()
    {
        // comprueba que estemos en la fase de batalla
        if (!battleFase) { return; }

        // comprueba que sea su turno
        if (turnNumber != myTurnNumber) { return; }

        // comprueba si una unidad fue seleccionada
        if (selectedUnit == null)
        {
            foreach (UnitScript unit in army)
            {
                if (unit.CanMove() && unit.Selected())
                {
                    selectedUnit = unit;
                    break;
                }
            }
        }

        // maneja la unidad seleccionada
        if (selectedUnit != null)
        {
            if (!selectedUnit.IsMoving())
                SelectTile(selectedUnit);

            // mueve la unidad si debe hacerlo
            selectedUnit.Move(worldPos);

            // ejecuta el ataque si existe un objetivo
            if (targetUnit != null)
            {
                selectedUnit.Attack(targetUnit);
                
                if (targetUnit.IsDead())
                    enemyArmy.Remove(targetUnit);

                targetUnit = null;
            }

            // comprueba si la unidad deja de estar seleccionada
            if (!selectedUnit.IsSelected() && !selectedUnit.IsMoving()) { selectedUnit = null; }
        }

        if (army.Count == 0)
        {
            // you lost, bitch
        }
    }

    private void DeployFaseManager()
    {
        // comprueba que estemos en la fase de despliegue
        if (!deployFase) { return; }

        if (deployUnitIndex != -1)
        {
            DeployUnit(deployUnitList[deployUnitIndex]);

            deployFase = false;
            foreach (UnitScript unit in deployUnitList)
                if (!unit.gameObject.activeSelf)
                    deployFase = true;
        }
        else
            deployUnitIndex = SetDeployUnitIndex();
    }

    private void DeployFaseEnded()
    {
        battleFase = true;

        canvas.ShowWaitingText(false);
        if (myTurnNumber == turnNumber)
            canvas.ShowEndTurnButton(true);

        // CameraManager.instance.SmoothZoomTo(.3f);
        // CameraManager.instance.SmoothMovementTo(new Vector3(0, 0, 0));

        //deployUnitList.AddRange(army);
        //army.Clear();

        for (int i = 0; i < deployUnitList.Count; i++)
        {
            UnitScript unitToReespawn = deployUnitList[i];

            // re-spawnea (reemplaza) las unidades instanciadas pero en la red
            ConnectionManager.instance.CmdSpawnObject(unitToReespawn.unitType, unitToReespawn.transform.position);

            // destruye las unidades que fueron reemplazadas
            Destroy(unitToReespawn.gameObject);
        }
    }

    #endregion

    #region Control Unidades

    private void SelectTile(UnitScript unit)
    {
        if (!ClickOnGrid(out int xTile, out int yTile)) { return; }

        // obtiene el tile(x, y) en el que esta la unidad
        map.GetGridTile(unit.GetPosition(), out int unitXTile, out int unitYTile);

        unit.SetTileRadius(unitXTile, unitYTile);
        // obtiene los tiles de movimiento y ataque de una unidad
        unit.SetMovementTiles(map.SetCollisionTiles(unit.GetMovementTiles(), unitXTile, unitYTile, unit.GetMovementRadius()));
        unit.SetAttackTiles(map.SetAttackTiles(unit.GetMovementTiles(), unitXTile, unitYTile));
        List<Vector2> movementTiles = unit.GetMovementTiles();
        List<Vector2> attackTiles = unit.GetAttackTiles();

        // elimina los tiles de ataque en los que hay unidades aliadas
        //foreach (Unidad otherUnit in army)
        //{
        //    Vector2 posOtraUnidad = new Vector2(otherUnit.GetPosition().x, otherUnit.GetPosition().y);
        //    if (attackTiles.Contains(posOtraUnidad))
        //    {
        //        attackTiles.Remove(otherUnit.GetPosition());
        //    }
        //}

        // comprueba que el click no haya sido sobre el tile de la unidad
        if (xTile != unitXTile || yTile != unitYTile)
        {
            if (unit.ClickOnMovementTile(xTile, yTile))
            {
                worldPos = map.GetWorldPos(xTile, yTile);
                // determina la direccion de movimiento segun la posicion del tile destino
                unit.SetMovementDirection(worldPos);
            }
            else if (unit.ClickOnAttackTile(xTile, yTile))
            {
                bool targetFound = false;
                foreach (UnitScript possibleTarget in enemyArmy)
                {
                    map.GetGridTile(possibleTarget.GetPosition(), out int targetXTile, out int targetYTile);
                    if (xTile == targetXTile && yTile == targetYTile)
                    {
                        targetUnit = possibleTarget;
                        unit.ToggleAttack(true);
                        targetFound = true;
                        break;
                    }
                }

                if (targetFound)
                {
                    unit.SetMovementDirection(map.GetWorldPos(xTile, yTile));

                    // asegura que al hacer click en un tile de ataque la unidad mueva al tile anterior
                    if (xTile > unitXTile)
                        xTile--;
                    else if (xTile < unitXTile)
                        xTile++;

                    if (yTile > unitYTile)
                        yTile--;
                    else if (yTile < unitYTile)
                        yTile++;

                    worldPos = map.GetWorldPos(xTile, yTile);
                }
                else { unit.ToggleSelected(false); }
            }
            else { unit.ToggleSelected(false); }

            map.DestroyTiles();
            movementTiles.Clear();
            attackTiles.Clear();
            selectingTile = false;
        }
        else if (!selectingTile)
        {           
            // crea los tiles de movimiento
            map.InstantiateMovementTiles(movementTiles);
            map.InstantiateAttackTiles(attackTiles);
            selectingTile = true;
        }
    }

    private bool ClickOnGrid(out int x, out int y)
    {
        x = 0;
        y = 0;
        if (Input.GetMouseButtonDown(0))
        {
            // obtiene la posicion del mouse dentro del juego
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return map.GetGridTile(mouseWorldPos, out x, out y);
        }

        return false;
    }

    #endregion

    #region Despliegue

    private int SetDeployUnitIndex()
    {
        // obtiene botones activos (visibles)
        List<GameObject> unitButtons = GameObject.FindGameObjectsWithTag("Boton").ToList();

        for (int i = 0; i < unitButtons.Count; i++)
        {
            try
            {
                if (unitButtons[i].GetComponent<UnitButtonScript>().selected)
                    return i;
            }
            catch (NullReferenceException)
            {
                return -1;
            }
        }

        return -1;
    }

    private void DeployUnit(UnitScript unit)
    {
        // obtiene botones activos (visibles)
        List<GameObject> unitButtons = GameObject.FindGameObjectsWithTag("Boton").ToList();

        if (ClickOnGrid(out int xTile, out int yTile))
        {
            // comprueba que el click no haya sido sobre otra unidad
            foreach (UnitScript otherUnit in army)
            {
                map.GetGridTile(otherUnit.GetPosition(), out int otherUnitXTile, out int otherUnitYTile);

                // comprueba si la posicion de despleigue conincide con la de otra unidad ya desplegada
                if (otherUnitXTile == xTile && otherUnitYTile == yTile)
                {
                    // 'deselecciona' el boton
                    unitButtons[deployUnitIndex].GetComponent<UnitButtonScript>().Deseleccionar();
                    deployUnitIndex = -1;

                    return;
                }
            }

            // oculta el boton
            unitButtons[deployUnitIndex].SetActive(false);

            Vector3 desplyPosition = map.GetWorldPos(xTile, yTile);
            unit.Deploy(desplyPosition);
            unit.gameObject.SetActive(true);
            deployUnitIndex = -1;
        }
    }

    private bool EveryoneDeployed()
    {
        if (endedDeployFaseCount == 2)
            return true;
        else
            return false;
    }

    #endregion
}