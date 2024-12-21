using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;

namespace ITKombat
{
    public class LobbyListUIManager : MonoBehaviour
    {
        public static LobbyListUIManager Instance;

        [SerializeField] private Transform lobbySingleTemplate;
        [SerializeField] private Transform container;
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button createLobbyButton;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            lobbySingleTemplate.gameObject.SetActive(false);
            refreshButton.onClick.AddListener(RefreshButtonClick);
            createLobbyButton.onClick.AddListener(CreateLobbyButtonClick);
        }

        private void Start()
        {
            // LobbyManager.Instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
            // LobbyManager.Instance.OnJoinedLobby += LobbyManager_OnJoinedLobby;
            // LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
            // LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;
        }

        private void RefreshButtonClick() {
            LobbyRoomManager.Instance.RefreshLobbyList();
        }

        private void CreateLobbyButtonClick() {
            // LobbyCreateUI.Instance.Show();
        }

    }
}
