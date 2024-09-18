using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace ITKombat
{
    public class NetworkManagerUI : MonoBehaviour
    {
        [SerializeField] private Button serverBtn;
        [SerializeField] private Button hostBtn;
        [SerializeField] private Button clientBtn;

        private void Awake()
        {
            serverBtn.onClick.AddListener(() => 
                {
                    NetworkManager.Singleton.StartServer();
                });
            hostBtn.onClick.AddListener(() => 
                {
                    NetworkManager.Singleton.StartHost();
                    Hide();
                });
            clientBtn.onClick.AddListener(() => 
                {
                    NetworkManager.Singleton.StartClient();
                    Hide();
                });
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

    }
}
