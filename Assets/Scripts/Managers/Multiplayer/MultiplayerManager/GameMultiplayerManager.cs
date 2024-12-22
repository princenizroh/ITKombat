using System;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Collections.Generic;
using Unity.Services.Authentication;
using UnityEngine.SceneManagement;

namespace ITKombat
{
    public class GameMultiplayerManager : NetworkBehaviour
    {
        public const int MAX_PLAYER_AMOUNT = 2;

        private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";
        public static GameMultiplayerManager Instance { get; private set;}

        public static bool playMultiplayer = true;

        public event EventHandler OnTryingToJoinGame;
        public event EventHandler OnFailedToJoinGame;
        public event EventHandler OnPlayerDataNetworkListChanged;

        private NetworkList<PlayerDataMultiplayer> playerDataNetworkList;
        private string playerName;

        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
            playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "PlayerName" + UnityEngine.Random.Range(0, 1000));

            playerDataNetworkList = new NetworkList<PlayerDataMultiplayer>();
        }        

        private void Start() {
        if (!playMultiplayer) {
            // Singleplayer
            StartHost();
            Loader.LoadNetwork(Loader.Scene.Multiplayer);
        }
    }

        

        public void StartHost() {
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            // NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            // NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
            NetworkManager.Singleton.StartHost();
        }
        public void StartClient() {
            OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
            Debug.Log("StartClient Called");
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
            // NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
            NetworkManager.Singleton.StartClient();
            Debug.Log("StartClient");
            Debug.Log("NetworkManager.Singleton.ConnectedClientsIds.Count " + NetworkManager.Singleton.ConnectedClientsIds.Count);
            // Loader.Load(Loader.Scene.ClientTest);
        }
        private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse) {
            Debug.Log("ConnectionApprovalCallback");
            Debug.Log("Current Scene: " + SceneManager.GetActiveScene().name);
            Debug.Log("Expected Scene: " + Loader.Scene.SelectCharacterMultiplayer.ToString());
            if (SceneManager.GetActiveScene().name != Loader.Scene.SelectCharacterMultiplayer.ToString()) {
                connectionApprovalResponse.Approved = false;
                connectionApprovalResponse.Reason = "Game has already started";
                Debug.Log("Game has already started");
                return;
            }
            Debug.Log("NetworkManager.Singleton.ConnectedClientsIds.Count " + NetworkManager.Singleton.ConnectedClientsIds.Count);
            if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT) {
                connectionApprovalResponse.Approved = false;
                connectionApprovalResponse.Reason = "Game is full";
                Debug.Log("Game is full");
                return;
            }
            Debug.Log("Connection Approved");
            connectionApprovalResponse.Approved = true;
        }

        private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId) {
            Debug.Log("Client Disconnected " + clientId);
            OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);

            // for (int i = 0; i < playerDataNetworkList.Count; i++) {
            //     PlayerDataMultiplayer playerDataMultiplayer = playerDataNetworkList[i];
            //     if (playerDataMultiplayer.clientId == clientId) {
            //         // Disconnected!
            //         playerDataNetworkList.RemoveAt(i);
            //     }
            // }

    #if DEDICATED_SERVER
            Debug.Log("playerDataNetworkList.Count " + playerDataNetworkList.Count);
            if (SceneManager.GetActiveScene().name == Loader.Scene.GameScene.ToString()) {
                // Player leaving during GameScene
                if (playerDataNetworkList.Count <= 0) {
                    // All players left the game
                    Debug.Log("All players left the game");
                    Debug.Log("Shutting Down Network Manager");
                    NetworkManager.Singleton.Shutdown();
                    Application.Quit();
                    //Debug.Log("Going Back to Main Menu");
                    //Loader.Load(Loader.Scene.MainMenuScene);
                }
            }
    #endif
        }
        private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId) {
            Debug.Log("Client connected with ID: " + clientId);
            OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
            // Loader.Load(Loader.Scene.Multiplayer);
            // SetPlayerNameServerRpc(GetPlayerName());
            // SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
        }

        private void NetworkManager_OnClientConnectedCallback(ulong clientId) {
            Debug.Log("Client Connected " + " " + clientId);

        //     playerDataNetworkList.Add(new PlayerData {
        //         clientId = clientId,
        //         colorId = GetFirstUnusedColorId(),
        //     });

        // #if !DEDICATED_SERVER
        //         SetPlayerNameServerRpc(GetPlayerName());
        //         SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
        // #endif
        }

        private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId) {
            Debug.Log("Client disconnected with ID: " + clientId);
            OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
        }


    }
}
