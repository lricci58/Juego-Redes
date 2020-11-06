using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleManager : NetworkBehaviour
{
    public static BattleManager instance = null;

    [NonSerialized] public List<UnitScript> army = new List<UnitScript>();
    private List<UnitScript> deployUnitList = new List<UnitScript>();
    private List<UnitScript> enemyArmy = new List<UnitScript>();
    private Vector3 worldPos;
    private UnitScript selectedUnit;
    private UnitScript targetUnit;
    private bool selectingTile = false;

    [NonSerialized] public int deployUnitType = -1;
    [NonSerialized] public bool deployFase = true;
    private bool battleFase = false;
    [NonSerialized] [SyncVar] public int endedDeployFaseCount = 0;
    [NonSerialized] [SyncVar] public int currentTurn = 0;
    [NonSerialized] public int myTurnNumber;

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
            // agrega todas las unidades instanciadas al ejercito enemigo local
            enemyArmy.Add(unit);

            // setea el contenedor padre de las unidades
            unit.transform.SetParent(map.unitContainer);
            unit.transform.SetParent(map.enemyUnitContainer);

            // gira la unidad ya que es enemiga
            if (GameManager.instance.playerBattleSide == 1) { unit.flipUnit(); }

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
        enemyArmy.Remove(unit);
        army.Add(unit);

        unit.transform.SetParent(map.allyUnitContainer);
    }

    void Update()
    {
        if (!ConnectionManager.instance.isLocalPlayer) { return; }
        if (!ConnectionManager.instance.hasAuthority) { return; }

        // si es un espectador, no tiene ningun control sobre la batalla
        if (GameManager.instance.playerBattleSide == 2) { return; }

        BattlePhaseManager();

        DeployPhaseManager();

        DeployPhaseEnded();
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
                SelectTile(selectedUnit);

            // mueve la unidad si debe hacerlo
            selectedUnit.Move(worldPos);

            // ejecuta el ataque si existe un objetivo
            if (targetUnit != null)
            {
                selectedUnit.Attack(targetUnit);

                if (targetUnit.IsDead())
                {
                    enemyArmy.Remove(targetUnit);

                    // si al enemigo no le quedan mas unidades
                    if (enemyArmy.Count == 0)
                        PlayerWonBattle();
                }

                targetUnit = null;
            }

            // comprueba si la unidad deja de estar seleccionada
            if (!selectedUnit.IsSelected() && !selectedUnit.IsMoving()) { selectedUnit = null; }
        }
    }

    private void PlayerWonBattle()
    {
        GameObject playerCountry = GameObject.Find(GameManager.instance.countryInvolvedInBattle);

        foreach (UnitScript unit in army)
            playerCountry.GetComponent<Pais>().countryGarrison.Add(unit.unitType);

        ConnectionManager.instance.CmdPlayerWon(playerCountry.name);

        // devuelve al ganador a modo normal
        GameManager.instance.playerBattleSide = 2;
        GameManager.instance.countryInvolvedInBattle = "";
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
            DeployUnit(deployUnitList[deployUnitType]);

            deployFase = false;
            foreach (UnitScript unit in deployUnitList)
                if (!unit.gameObject.activeSelf)
                    deployFase = true;
        }
    }

    private void DeployPhaseEnded()
    {
        // comprueba que los 2 jugadores hayan desplegado
        if (!EveryoneDeployed() || battleFase) { return; }

        battleFase = true;

        // oculta la zona de despliegue
        map.deployZone.gameObject.SetActive(false);

        canvas.ShowWaitingText(false);
        // muestra el boton de terminar turno al primer jugador
        if (myTurnNumber == currentTurn)
            canvas.ShowEndTurnButton(true);

        // CameraManager.instance.SmoothZoomTo(.3f);
        // CameraManager.instance.SmoothMovementTo(new Vector3(0, 0, 0));

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

        // comprueba que el click no haya sido sobre otra unidad
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

        Vector3 desplyPosition = map.GetWorldPos(gridClickPos);
        unit.Deploy(desplyPosition);

        if (GameManager.instance.playerBattleSide == 1)
            unit.flipUnit();
        unit.gameObject.SetActive(true);

        GameManager.instance.unitsToBattle.Remove(deployUnitType);
        canvas.UpdateDeploymentPanel();

        // deselecciona el boton
        deployUnitType = -1;
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