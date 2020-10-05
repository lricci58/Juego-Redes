using Mirror;
using UnityEngine;

public class ControladorConexion : NetworkBehaviour
{
    public static ControladorConexion instancia = null;
    public ControladorBatalla batalla;

    void Start()
    {
        if (!hasAuthority) return;

        instancia = this;
        Instantiate(batalla);
    }

    [Command]
    public void CmdSpawnObjeto(int indice)
    {
        GameObject objetoAInstanciar = ControladorBatalla.instancia.mapa.unidades[indice];
        GameObject instancia = Instantiate(objetoAInstanciar, objetoAInstanciar.transform.position, Quaternion.identity);

        NetworkServer.Spawn(instancia);
    }

    [Command]
    public void CmdJugadorTerminoDespliegue()
    {
        ControladorBatalla.instancia.desplegados++;
    }
}
