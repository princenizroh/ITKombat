using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
namespace ITKombat
{
    public class ConnectingResponseMessageUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text responseMessage;
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            closeButton.onClick.AddListener(() =>
            {
                Hide();
                NewSoundManager.Instance.PlaySound2D("Button_Click");
            });
        }

        private void Start()
        {
            GameMultiplayerManager.Instance.OnFailedToJoinGame += GameMultiplayerManager_OnFailedToJoinGame;
            LobbyRoomManager.Instance.OnCreateLobbyStarted += LobbyRoomManager_OnCreateLobbyStarted;
            LobbyRoomManager.Instance.OnCreateLobbyFailed += LobbyRoomManager_OnCreateLobbyFailed;
            LobbyRoomManager.Instance.OnJoinStarted += LobbyRoomManager_OnJoinStarted;
            LobbyRoomManager.Instance.OnJoinFailed += LobbyRoomManager_OnJoinFailed;
            LobbyRoomManager.Instance.OnQuickJoinFailed += LobbyRoomManager_OnQuickJoinFailed;
            Debug.Log("ConnectingResponseMessageUI Start");
            Hide();
        }

        private void LobbyRoomManager_OnQuickJoinFailed(object sender, System.EventArgs e) {
            ShowMessage("Could not find a Lobby to Quick Join!");
        }

        private void LobbyRoomManager_OnJoinFailed(object sender, System.EventArgs e) {
            ShowMessage("Failed to join Lobby!");
        }

        private void LobbyRoomManager_OnJoinStarted(object sender, System.EventArgs e) {
            ShowMessage("Joining Lobby...");
        }

        private void LobbyRoomManager_OnCreateLobbyFailed(object sender, System.EventArgs e) {
            ShowMessage("Failed to create Lobby!");
        }

        private void LobbyRoomManager_OnCreateLobbyStarted(object sender, System.EventArgs e) {
            ShowMessage("Creating Lobby...");
        }

        private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e) {
            if (NetworkManager.Singleton.DisconnectReason == "") {
                ShowMessage("Failed to connect");
            } else {
                ShowMessage(NetworkManager.Singleton.DisconnectReason);
            }
        }

        private void GameMultiplayerManager_OnFailedToJoinGame(object sender, System.EventArgs e)
        {
            Show();
            string disconnectReason = NetworkManager.Singleton.DisconnectReason;
            responseMessage.text = "Failed to connect";
            Debug.Log("Disconnect Reason: " + disconnectReason);
            Debug.Log("Response Message Text: " + responseMessage.text);
            
        }

        private void ShowMessage(string message) {
            Show();
            responseMessage.text = message;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            GameMultiplayerManager.Instance.OnFailedToJoinGame -= GameMultiplayerManager_OnFailedToJoinGame;
            LobbyRoomManager.Instance.OnCreateLobbyStarted -= LobbyRoomManager_OnCreateLobbyStarted;
            LobbyRoomManager.Instance.OnCreateLobbyFailed -= LobbyRoomManager_OnCreateLobbyFailed;
            LobbyRoomManager.Instance.OnJoinStarted -= LobbyRoomManager_OnJoinStarted;
            LobbyRoomManager.Instance.OnJoinFailed -= LobbyRoomManager_OnJoinFailed;
            LobbyRoomManager.Instance.OnQuickJoinFailed -= LobbyRoomManager_OnQuickJoinFailed;
        }
    }
}
