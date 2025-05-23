using UnityEngine;
using Mirror;

public class CustomNetManager : NetworkManager
{
    public GameObject hostPrefab;
    public GameObject clientPrefab;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject playerPref = conn.identity == null ? hostPrefab : clientPrefab;
        GameObject playerInstance = Instantiate(playerPref);
        NetworkServer.AddPlayerForConnection(conn, playerInstance);
    }
}
