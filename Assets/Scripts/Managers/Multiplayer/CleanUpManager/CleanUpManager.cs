using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace ITKombat
{
    public class CleanUpManager : MonoBehaviour
    {
        private void Awake()
        {
            if (NetworkManager.Singleton != null)
            {
                Destroy(NetworkManager.Singleton.gameObject);
            }
            if (LobbyManager.Instance != null)
            {
                Destroy(LobbyManager.Instance.gameObject);
            }            
        }
    }
}
