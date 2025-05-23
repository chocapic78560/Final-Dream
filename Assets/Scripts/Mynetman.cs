using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    private int playerIndex = 0;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject prefabToSpawn = playerIndex == 0 ? player1Prefab : player2Prefab;

        // IMPORTANT : GetStartPosition() peut retourner null
        Vector3 spawnPosition = GetStartPosition() != null
            ? GetStartPosition().position
            : Vector3.zero;

        GameObject player = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);
        playerIndex++;
    }
}