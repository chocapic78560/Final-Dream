using Mirror;
using UnityEngine;
public class TeleportTriggerSimple : NetworkBehaviour
{
    [Header("Teleport Settings")]
    public Vector3 teleportOffset = new Vector3(100f, 0f, 0f);
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        NetworkIdentity identity = other.GetComponent<NetworkIdentity>();
        
        // Seul le joueur local déclenche la téléportation
        if (identity != null && identity.isLocalPlayer)
        {
            CmdTeleportAllPlayers();
        }
    }
    
    [Command(requiresAuthority = false)]
    private void CmdTeleportAllPlayers()
    {
        // Utilise ClientRpc pour synchroniser tous les clients
        RpcTeleportAllPlayers();
    }
    
    [ClientRpc]
    private void RpcTeleportAllPlayers()
    {
        // Téléporte tous les joueurs locaux sur chaque client
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (GameObject player in players)
        {
            NetworkIdentity identity = player.GetComponent<NetworkIdentity>();
            if (identity != null)
            {
                Vector3 newPos = player.transform.position + teleportOffset;
                player.transform.position = newPos;
            }
        }
    }
}



