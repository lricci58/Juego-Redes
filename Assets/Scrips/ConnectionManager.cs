using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ConnectionManager : NetworkBehaviour
{
    public static ConnectionManager instance = null;
    public int mapScene = 1;

    void Start()
    {
        DontDestroyOnLoad(this);

        if (!isLocalPlayer) { return; }

        instance = this;
        SceneManager.LoadScene(mapScene);
    }

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
    public void CmdCountrySelected(string selectedCountryName, string[] borderingCountryName)
    {
        RpcUpdateSelectedCountry(selectedCountryName, borderingCountryName);
    }

    [ClientRpc]
    public void RpcUpdateSelectedCountry(string selectedCountryName, string[] borderingCountryName)
    {
        MapManager.instancia.ActualizarEstadoPaises(selectedCountryName, borderingCountryName);
    }
}
