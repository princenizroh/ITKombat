using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

namespace ITKombat
{
    public class HostDisconnectUI : MonoBehaviour
    {
        public Button playAgainButton;

        private void Start()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

            Hide();
        }

        private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
        {
            if (clientId == NetworkManager.ServerClientId)
            {
                Show();
            }
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
