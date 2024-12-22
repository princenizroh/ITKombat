using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace ITKombat
{
    public class HostDisconnectUI : MonoBehaviour
    {
        [SerializeField] private GameObject disconnectCanvas;

        private NetworkVariable<bool> isHostAvailable = new NetworkVariable<bool>(true);

        private void Start()
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                // Listen to changes in isHostAvailable for clients
                ServerBattleRoomState.Instance.isHostAvailable.OnValueChanged += OnHostAvailabilityChanged;
            }
            
            Hide();
            Debug.Log("HostDisconnectUI initialized");
            UnityTransport transport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            Debug.Log($"Is server running: {NetworkManager.Singleton.IsListening}");

        }

        private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
        {
            Debug.Log($"Host DisconnectUI: Client disconnected: {clientId}");
            Debug.Log($"ServerClientId: {NetworkManager.ServerClientId}");

            if (clientId == NetworkManager.ServerClientId)
            {
                Debug.Log("Host disconnected");
                Show();
            }
        }

        private void OnHostAvailabilityChanged(bool previousValue, bool newValue)
        {
            if (!newValue)
            {
                Debug.Log("Detected that host is unavailable.");
                Show();
            }
        }

        private void Show()
        {
            if (!IsHost())
            {
                disconnectCanvas.SetActive(true);
                Debug.Log("Displaying host disconnect UI on client.");
            }
        }

        private void Hide()
        {
            disconnectCanvas.SetActive(false);
        }

        private bool IsHost()
        {
            return NetworkManager.Singleton.IsServer;
        }

        
    }
}
