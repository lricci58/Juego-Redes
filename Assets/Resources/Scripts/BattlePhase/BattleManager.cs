using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleManager : NetworkBehaviour
{
    public static BattleManager instance;

    [NonSerialized] public List<UnitScript> army = new List<UnitScript>();
    [NonSerialized] public List<UnitScript> enemyArmy = new List<UnitScript>();
    private List<UnitScript> deployUnitList = new List<UnitScript>();
    private List<int> unitTypesList = new List<int>();
    private List<Vector3> unitPositionsList = new List<Vector3>();
    private Vector3 worldPos;
    private UnitScript selectedUnit = null;
    private UnitScript targetUnit = null;
    private bool selectingTile = false;

    [NonSerialized] public int deployUnitType = -1;
    [NonSerialized] public bool deployFase = true;
    private bool battleFase = false;
    public bool everyoneDeployed = false;
    [NonSerialized] [SyncVar] public int currentTurn = 0;
    [NonSerialized] public int myTurnNumber = -1;

    [Header("Other Scripts")]
    public MapLoader map;
    [SerializeField] private GameObject canvasObject = null;
    [NonSerialized] public BattleUI_Manager canvas;

    void Start()
    {
        instance = this;

        // sets map and units
        InitGame();
    }

    private void InitGame()
    {
        myTurnNumber = GameManager.instance.playerBattleSide;

        canvas = Instantiate(canvasObject).GetComponent<BattleUI_Manager>();

        canvas.ShowDeploymentPanel(true);
        canvas.ShowReadyButton(true);
        
        map.SetScene();
    }

    public void UnitInstantiated(UnitScript unit)
    {
        if (battleFase)
        {
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
        // si la unidad resultaba ser aliada se la remueve del ejercito enemigo
        army.Add(unit);
        // si la unidad es mia y soy atacante (lado derecho) hace flip
        if (GameManager.instance.playerBattleSide == 1) { unit.flipUnit(); }

        unit.transform.SetParent(map.allyUnitContainer);
    }

    public void AddUnitToEnemyArmy(UnitScript unit)
    {
        enemyArmy.Add(unit);
        // si la unidad es enemiga y soy defensor (lado izquierdo) hace flip
        if (GameManager.instance.playerBattleSide == 0) { unit.flipUnit(); }

        unit.transform.SetParent(map.enemyUnitContainer);
    }

    void Update()
    {
        if (!ConnectionManager.instance.isLocalPlayer) { return; }
        if (!ConnectionManager.instance.hasAuthority) { return; }

        // si es un espectador, no tiene ningun control sobre la batalla
        if (GameManager.instance.playerBattleSide == 2) { return; }

        BattlePhaseManager();

        DeployPhaseManager();

        DeployPhaseEnded(unitTypesList, unitPositionsList);
    }

    #region ControlJuego

    private void BattlePhaseManager()
    {
        // comprueba que este en la fase de batalla
        if (!battleFase) { return; }

        // comprueba que sea el turno del jugador
        if (myTurnNumber != currentTurn) { return; }

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
                SelectTileToMove(selectedUnit);

            // mueve la unidad si debe hacerlo
            selectedUnit.Move(worldPos);

            // ejecuta el ataque si existe un objetivo
            if (targetUnit != null)
            {
                // ejecuta el ataque sobre la unidad
                selectedUnit.Attack(targetUnit);

                targetUnit = null;
            }

            // comprueba si la unidad deja de estar seleccionada
            if (!selectedUnit.IsSelected() && !selectedUnit.IsMoving()) { selectedUnit = null; }
        }
    }

    public void PlayerWonBattle()
    {
        List<int> unitTypesList = new List<int>();

        foreach (UnitScript unit in army)
            unitTypesList.Add(unit.unitType);

        GameObject winnerCountry = GameObject.Find(GameManager.instance.countryInvolvedInBattle);

        // devuelve a los sobrevivientes
        winnerCountry.GetComponent<Pais>().countryGarrison.AddRange(unitTypesList);

        // devuelve al jugador a modo normal
        GameManager.instance.playerBattleSide = 2;
        GameManager.instance.countryInvolvedInBattle = "";

        ConnectionManager.instance.CmdPlayerWon(GameManager.instance.countryInvolvedInBattle);
    }

    public void PlayerRetreated() 
    {
        // @TODO: devolver los sobrevivientes a los paises
    }

    private void DeployPhaseManager()
    {
        // comprueba que estemos en la fase de despliegue
        if (!deployFase) { return; }

        // comprueba que se haya seleccionado una unidad para desplegar
        if (deployUnitType != -1)
        {
            // recorre la lista para ver que tipo de unidad se quiere desplegar
            foreach (UnitScript deployUnit in deployUnitList)
            {
                if (deployUnit.gameObject.activeSelf || deployUnit.unitType != deployUnitType) { continue; }
                
                DeployUnit(deployUnit);
                break;
            }

            deployFase = false;
            foreach (UnitScript unit in deployUnitList)
                if (!unit.gameObject.activeSelf)
                    deployFase = true;
        }
    }

    private void DeployPhaseEnded(List<int> unitTypesList, List<Vector3> unitPositionsList)
    {
        // comprueba que los 2 jugadores hayan desplegado
        if (!everyoneDeployed || battleFase) { return; }

        battleFase = true;

        // oculta la zona de despliegue
        map.deployZone.gameObject.SetActive(false);

        canvas.ShowWaitingText(false);
        // muestra el boton de terminar turno al primer jugador
        if (myTurnNumber == currentTurn)
            canvas.ShowEndTurnButton(true);

        // CameraManager.instance.SmoothZoomTo(.3f);
        // CameraManager.instance.SmoothMovementTo(new Vector3(0, 0, 0));

        // crea las unidades conectadas en todos los clientes
        ConnectionManager.instance.CmdSpawnObject(unitTypesList, unitPositionsList);

        // destruye las unidades reemplazadas
        foreach (UnitScript unit in deployUnitList)
            Destroy(unit.gameObject);
        deployUnitList.Clear();
    }

    #endregion

    #region Control Unidades

    private void SelectTileToMove(UnitScript unit)
    {
        Vector3Int gridClickPos;

        // obtiene el tile en el que se hizo click
        if (!ClickOnGrid(out gridClickPos)) { return; }

        // obtiene el tile en el que esta la unidad
        Vector3Int gridUnitPos = map.GetGridTile(unit.GetPosition());

        // setea el radio de movimiento y ataque de la unidad
        unit.SetTilesRadius(gridUnitPos);

        List<Vector2> movementTiles = unit.GetMovementTiles();
        List<Vector2> attackTiles = unit.GetAttackTiles();

        // comprueba que el click no haya sido sobre el tile de la unidad
        if (!selectingTile)
        {
            // crea los tiles de movimiento
            map.InstantiateMovementTiles(movementTiles);
            map.InstantiateAttackTiles(attackTiles);
            selectingTile = true;
        }
        else
        {
            if (unit.ClickOnMovementTile(gridClickPos.x, gridClickPos.y))
            {
                worldPos = map.GetWorldPos(gridClickPos);
                // determina la direccion de movimiento segun la posicion del tile destino
                unit.SetMovementDirection(worldPos);
            }
            else if (unit.ClickOnAttackTile(gridClickPos.x, gridClickPos.y))
            {
                bool targetFound = false;
                foreach (UnitScript possibleTarget in enemyArmy)
                {
                    Vector3Int gridTargetUnitPos = map.GetGridTile(possibleTarget.GetPosition());
                    if (gridClickPos.x == gridTargetUnitPos.x && gridClickPos.y == gridTargetUnitPos.y)
                    {
                        targetUnit = possibleTarget;
                        unit.ToggleAttack(true);
                        targetFound = true;
                        break;
                    }
                }

                if (targetFound)
                {
                    unit.SetMovementDirection(map.GetWorldPos(gridClickPos));

                    // asegura que al hacer click en un tile de ataque la unidad mueva al tile anterior
                    if (gridClickPos.x > gridUnitPos.x)
                        gridClickPos.x--;
                    else if (gridClickPos.x < gridUnitPos.x)
                        gridClickPos.x++;

                    if (gridClickPos.y > gridUnitPos.y)
                        gridClickPos.y--;
                    else if (gridClickPos.y < gridUnitPos.y)
                        gridClickPos.y++;

                    worldPos = map.GetWorldPos(gridClickPos);
                }
                else { unit.ToggleSelected(false); }
            }
            else { unit.ToggleSelected(false); }

            map.DestroyTiles();
            movementTiles.Clear();
            attackTiles.Clear();
            selectingTile = false;
        }
    }

    public bool CanMoveToPosition(Vector3Int gridPos)
    {
        // comprueba que sea un espacio disponible para mover
        if (!map.CanMoveToPos(map.GetWorldPos(gridPos)))
            return false;

        return true;
    }
    
    public bool IsUnitInPosition(Vector3Int tilePos)
    {
        // comprueba si hay una unidad enemiga en la posicion
        foreach (UnitScript unit in enemyArmy)
        {
            if (tilePos == map.GetGridTile(unit.GetPosition()))
                return true;
        }

        return false;
    }

    private bool ClickOnGrid(out Vector3Int gridClickPos)
    {
        gridClickPos = new Vector3Int(0, 0, 0);
        if (Input.GetMouseButtonDown(0))
        {
            // comprueba si el click fue sobre la UI
            if (EventSystem.current.IsPointerOverGameObject()) { return false; }

            // obtiene la posicion del mouse dentro del juego al hacer click
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gridClickPos = map.GetGridTile(mouseWorldPos);
            return true; 
        }
        return false;
    }

    #endregion

    #region Despliegue

    private void DeployUnit(UnitScript unit)
    {
        Vector3Int gridClickPos;

        // obtiene el tile en el que se hizo click
        if (!ClickOnGrid(out gridClickPos)) { return; }

        // comprueba si hizo click sobre la zona de despliegue
        if (!map.CanDeployInPos(gridClickPos)) { return; }

        // comprueba que el click de posicionamiento no haya sido sobre otra unidad
        foreach (UnitScript otherUnit in army)
        {
            Vector3Int gridUnitPos = map.GetGridTile(otherUnit.GetPosition());

            // comprueba si la posicion de despleigue conincide con la de otra unidad ya desplegada
            if (gridUnitPos.x == gridClickPos.x && gridUnitPos.y == gridClickPos.y)
            {
                // deselecciona el boton
                deployUnitType = -1;
                return;
            }
        }

        // setea la unidad en la posicion y direccion adecuada
        Vector3 desplyPosition = map.GetWorldPos(gridClickPos);
        unit.Deploy(desplyPosition);
        if (GameManager.instance.playerBattleSide == 1)
            unit.flipUnit();
        unit.gameObject.SetActive(true);

        // actualiza la lista para desplegar y el panel
        GameManager.instance.unitsToBattle.Remove(unit.unitType);
        canvas.UpdateDeploymentPanel();

        // deselecciona el boton
        deployUnitType = -1;

        unitTypesList.Add(unit.unitType);
        unitPositionsList.Add(unit.transform.position);
    }

    #endregion
}