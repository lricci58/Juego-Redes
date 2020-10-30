using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ConnectionManager : NetworkBehaviour
{
    public static ConnectionManager instance = null;

    void Start()
    {
        DontDestroyOnLoad(this);

        if (!isLocalPlayer) { return; }

        instance = this;
    }
    /* si es [TargetRpc] es del servidor a un solo cliente
     * si es [ClientRpc] es del servidor a todos los clientes y 
     * si es [Command]  es del cualquier cliente la servidor 
     */

    [Command]
    public void CmdSpawnObject(int index, Vector3 unitLocalPosition)
    {
        GameObject originalPrefab = BattleManager.instance.map.unitPrefabs[index];
        GameObject instance = Instantiate(originalPrefab, unitLocalPosition, Quaternion.identity);
        instance.transform.position = unitLocalPosition;

        // @NOTE: a veces el cliente ejecuta el comando de mas

        // spawnea la unidad y otorga la autoridad del objeto al cliente del parametro
        NetworkServer.Spawn(instance, connectionToClient);
    }

    [Command]
    public void CmdEndedDeployFase() => RpcUpdateDesployCount();

    [ClientRpc]
    public void RpcUpdateDesployCount() => BattleManager.instance.endedDeployFaseCount++;

    [Command]
    public void CmdCheckUnitOwner(NetworkIdentity unitIdentity)
    {
        if (connectionToClient.clientOwnedObjects.Contains(unitIdentity))
            TargetUpdateArmy(connectionToClient, unitIdentity);
    }

    [TargetRpc]
    public void TargetUpdateArmy(NetworkConnection conn, NetworkIdentity unitIdentity)
    {
        BattleManager.instance.AddUnitToArmy(unitIdentity.GetComponent<UnitScript>());
    }

    [Command]
    public void CmdAttackUnit(GameObject unitObject, float damage)
    {
        UnitScript unit = unitObject.GetComponent<UnitScript>();

        if (unit.currentHealth > 0)
            unit.currentHealth -= damage;
    }

    [Command]
    public void CmdCountryWasSelected(string selectedName, string[] borderingNames) => RpcUpdateSelectedCountryOnClients(selectedName, borderingNames);

    [ClientRpc]
    public void RpcUpdateSelectedCountryOnClients(string selectedName, string[] borderingNames)
    {
        MapManager.instancia.ActualizarEstadoPaises(selectedName, borderingNames);
    }

    [Command]
    public void CmdEndTurn(int playerBattleSide) => RpcPlayerEndedTurn(playerBattleSide, NetworkServer.connections.Count - 1);

    [ClientRpc]
    public void RpcPlayerEndedTurn(int playerBattleSide, int amountPlayers)
    {
        if (SceneManager.GetActiveScene().name == "CampaignMapScene")
        {
            // cambia de turno
            if (++MapManager.instancia.turnoActual > amountPlayers) { MapManager.instancia.turnoActual = 0; }

            // deselecciona cualquier pais que pueda estar seleccionado
            MapManager.instancia.HayPaisSeleccionado();

            // muestra el boton de terminar de turno en su turno
            if (MapManager.instancia.miTurno == MapManager.instancia.turnoActual)
                MapManager.instancia.canvas.ShowEndPhaseButton(true);
            else
                MapManager.instancia.canvas.ShowEndPhaseButton(false);
        }
        else if (SceneManager.GetActiveScene().name == "BattleScene")
        {
            // si se termino el turno del defensor (0), empieza el del atacante(0++)
            if (playerBattleSide == 0)
                BattleManager.instance.currentTurn++;
            else if (playerBattleSide == 1)
                BattleManager.instance.currentTurn--;

            // muestra el boton de terminar de turno en su turno
            if (BattleManager.instance.myTurnNumber == BattleManager.instance.currentTurn)
                BattleManager.instance.canvas.ShowEndTurnButton(true);
            else
                BattleManager.instance.canvas.ShowEndTurnButton(false);
        }
    }

    [Command]
    public void CmdPlayerAttacked(string paisAtacado, string paisAtacante)
    {
        // busca el jugador que esta siendo atacado
        RpcSearchCountryInPlayers(paisAtacado, paisAtacante);
    }

    [ClientRpc]
    public void RpcSearchCountryInPlayers(string paisAtacado, string paisAtacante)
    {
        // comprueba que el pais sea suyo
        if (GameManager.instance.misPaises.Contains(paisAtacado))
        {
            // deselecciona al pais seleccionado y sus limitrofes
            MapManager.instancia.HayPaisSeleccionado();

            // despliega el menu de ataque
            MapManager.instancia.DesplegarMenuAtaque(null, null, paisAtacado, paisAtacante, 0);
        }
    }

    [Command]
    public void CmdPlayerStoppedAttacking()
    {
        RpcCloseAttackMenues();
    }

    [ClientRpc]
    public void RpcCloseAttackMenues()
    {
        MapManager.instancia.OcultarMenuAtaque();
    }

    [ClientRpc]
    public void RpcChangeScene(string sceneName) => SceneManager.LoadScene(sceneName);

    [TargetRpc]
    public void TargetAddCountry(NetworkConnection conn, string nombrePais)
    {
        GameManager.instance.misPaises.Add(nombrePais);
    }

    [TargetRpc]
    public void TargetSetYourTurnNumber(NetworkConnection conn, int turnNumber)
    {
        MapManager.instancia.miTurno = turnNumber;

        // muestra el boton de terminar de turno en su turno
        if (MapManager.instancia.miTurno == MapManager.instancia.turnoActual)
            MapManager.instancia.canvas.ShowEndPhaseButton(true);
    }
}
