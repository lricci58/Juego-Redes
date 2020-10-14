using Mirror;
using UnityEngine;

public class ConnectionManager : NetworkBehaviour
{
    public static ConnectionManager instance = null;
    public BattleManager battleManager;

    void Start()
    {
        if (!isLocalPlayer) return;

        instance = this;
        Instantiate(battleManager);
    }

    [Command]
    public void CmdSpawnObject(int index, Vector3 unitLocalPosition)
    {
        GameObject originalPrefab = BattleManager.instance.map.unitPrefabs[index];
        GameObject instance = Instantiate(originalPrefab, unitLocalPosition, Quaternion.identity);

        // @NOTE: al instanciar en server, host (a veces) recibe un objeto "de mas"

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
        BattleManager.instance.AddUnitToArmy(unitIdentity.GetComponent<Unit>());
    }

    [Command]
    public void CmdAttackUnit(GameObject unitObject, float damage)
    {
        Debug.Log(unitObject);

        Unit unit = unitObject.GetComponent<Unit>();
        if (unit.health > 0)
            unit.health -= damage;
    }
}
