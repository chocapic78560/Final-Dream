using UnityEngine;
using Mirror;

public class MoveToMenuMultiplayer : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Vérifier si l'objet qui entre est un joueur local
        if (other.gameObject.CompareTag("Player"))
        {
            NetworkIdentity playerIdentity = other.GetComponent<NetworkIdentity>();
            
            if (playerIdentity != null && playerIdentity.isLocalPlayer)
            {
                // Directement charger le menu sans passer par le réseau
                LoadMainMenuDirectly();
            }
        }
    }

    private void LoadMainMenuDirectly()
    {
        // Arrêter toute activité réseau immédiatement
        if (NetworkManager.singleton != null)
        {
            if (NetworkManager.singleton.mode == NetworkManagerMode.Host)
            {
                NetworkManager.singleton.StopHost();
            }
            else if (NetworkManager.singleton.mode == NetworkManagerMode.ClientOnly)
            {
                NetworkManager.singleton.StopClient();
            }
            else if (NetworkManager.singleton.mode == NetworkManagerMode.ServerOnly)
            {
                NetworkManager.singleton.StopServer();
            }
        }
        
        // Charger immédiatement la scène du menu
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }
}