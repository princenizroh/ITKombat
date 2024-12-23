using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace ITKombat
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private Button leaveGameButton;
        [SerializeField] private Button quickJoinGameButton;
        [SerializeField] private Button joinCodeGameButton;
        // [SerializeField] private Button listLobbiesButton;
        [SerializeField] private TMP_InputField joinCodeInputField;
        [SerializeField] private TMP_InputField playerNameInputField;
        [SerializeField] private Transform lobbyContainer;
        [SerializeField] private Transform lobbyTemplate;


        private void Awake()
        {
            leaveGameButton.onClick.AddListener(() => {
                LobbyRoomManager.Instance.LeaveLobby();
                Loader.Load(Loader.Scene.BattleMode);
            });
            quickJoinGameButton.onClick.AddListener(() => {
                LobbyRoomManager.Instance.QuickJoinLobby();
            });
            joinCodeGameButton.onClick.AddListener(() => {
                LobbyRoomManager.Instance.JoinWithCode(joinCodeInputField.text);
            });
            // listLobbiesButton.onClick.AddListener(() => {
                
            // });
            lobbyTemplate.gameObject.SetActive(false);
        }

        private void Start()
        {
            playerNameInputField.text = GameMultiplayerManager.Instance.GetPlayerName();
            playerNameInputField.onEndEdit.AddListener((string text) => {
                GameMultiplayerManager.Instance.SetPlayerName(text);
            });

            LobbyRoomManager.Instance.OnLobbyListChanged += LobbyRoomManager_OnLobbyListChanged;
        }

        private void LobbyRoomManager_OnLobbyListChanged(object sender, LobbyRoomManager.OnLobbyListChangedEventArgs e) {
            UpdateLobbyList(e.lobbyList);
        }


        private void UpdateLobbyList(List<Lobby> lobbyList) {
            foreach (Transform child in lobbyContainer) {
                if (child == lobbyTemplate) continue;
                Destroy(child.gameObject);
            }

            foreach (Lobby lobby in lobbyList) {
                Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
                lobbyTransform.gameObject.SetActive(true);
                lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
            }
        }
    }
}

