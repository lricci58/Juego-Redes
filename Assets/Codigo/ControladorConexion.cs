using Mirror;
using UnityEngine;

public class ControladorConexion : NetworkBehaviour
{
    public static ControladorConexion instancia = null;
    public GameObject objetoBatalla;

    void Start()
    {
        if (!isLocalPlayer) return;

        instancia = this;
        Instantiate(objetoBatalla);
    }

    [Command]
    public void CmdSpawnObjeto(int indice, Vector3 posUnidadLocal)
    {
        GameObject aInstanciar = ControladorBatalla.instancia.mapa.unidades[indice];
        GameObject unidadInstanciada =  Instantiate(aInstanciar, posUnidadLocal, Quaternion.identity);

        // @TODO: figure out how to delete unidadInstanciada after spawning

        // spawnea la unidad y otorga la autoridad del objeto al cliente del parametro
        NetworkServer.Spawn(unidadInstanciada, connectionToClient);
    }

    [Command]
    public void CmdJugadorTerminoDespliegue() => RpcTest();

    [ClientRpc]
    public void RpcTest() => ControladorBatalla.instancia.desplegados++;

    [Command]
    public void CmdUnidadValida(NetworkIdentity identidadUnidad)
    {
        if (connectionToClient.clientOwnedObjects.Contains(identidadUnidad))
            TargetActualizarValidez(connectionToClient, identidadUnidad);
    }

    [TargetRpc]
    public void TargetActualizarValidez(NetworkConnection conn, NetworkIdentity identidadUnidad)
    {
        ControladorBatalla.instancia.AgregarUnidadAEjercito(identidadUnidad.GetComponent<Unidad>());
    }
}
