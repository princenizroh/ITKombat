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
        public event EventHandler OnDataInitialized;
        
        [SerializeField] private List<GameObject> playerPrefabsList;
        private NetworkList<PlayerDataMultiplayer> playerDataNetworkList;
        public Dictionary<int, CharacterStat> characterStatDictionary;

        private string playerName;

        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
            playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "PlayerName" + UnityEngine.Random.Range(0, 1000));

            playerDataNetworkList = new NetworkList<PlayerDataMultiplayer>();
            playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
        }        

        public string GetPlayerName()
        {
            return playerName;
        }
        public void SetPlayerName(string playerName)
        {
            this.playerName = playerName;
            PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, playerName);
        }

        private void Start() {
            InitializeCharacterStats();
            if (!playMultiplayer) {
                // Singleplayer
                StartHost();
                Loader.LoadNetwork(Loader.Scene.SoundMultiplayer);
            }
        }
        private void InitializeCharacterStats()
        {
            characterStatDictionary = new Dictionary<int, CharacterStat>
            {
                { 0, Resources.Load<CharacterStat>("Characters/IFStats") },
                { 1, Resources.Load<CharacterStat>("Characters/MesinStats") },
                { 2, Resources.Load<CharacterStat>("Characters/FisikaStats") },
                { 3, Resources.Load<CharacterStat>("Characters/KimiaStats") },
                { 4, Resources.Load<CharacterStat>("Characters/KelautanStats") },
            };
            OnDataInitialized?.Invoke(this, EventArgs.Empty);
        }

        private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerDataMultiplayer> changeEvent) {
            OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
        }

        public void StartHost() {
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
            NetworkManager.Singleton.StartHost();
        }
        public void StartClient() {
            OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
            Debug.Log("StartClient Called");
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
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

            for (int i = 0; i < playerDataNetworkList.Count; i++) {
                PlayerDataMultiplayer playerDataMultiplayer = playerDataNetworkList[i];
                if (playerDataMultiplayer.clientId == clientId) {
                    // Disconnected!
                    playerDataNetworkList.RemoveAt(i);
                }
            }

    #if DEDICATED_SERVER
            Debug.Log("playerDataNetworkList.Count " + playerDataNetworkList.Count);
            if (SceneManager.GetActiveScene().name == Loader.Scene.GameScene.ToString()) {
                if (playerDataNetworkList.Count <= 0) {
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
            // OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
            // Loader.Load(Loader.Scene.Multiplayer);
            SetPlayerNameServerRpc(GetPlayerName());
            Debug.Log("SetPlayerNameServerRpc");
            SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
        }

        private void NetworkManager_OnClientConnectedCallback(ulong clientId) {
            Debug.Log("Client Connected " + " " + clientId);

            playerDataNetworkList.Add(new PlayerDataMultiplayer {
                clientId = clientId,
                prefabId = GetFirstUnusedPrefabId(),
            });
            SetPlayerNameServerRpc(GetPlayerName());
            SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);

        // #if !DEDICATED_SERVER
        //         SetPlayerNameServerRpc(GetPlayerName());
        //         SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
        // #endif
        }

        private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId) {
            Debug.Log("Client disconnected with ID: " + clientId);
            OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
        }

            
        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default) {
            int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

            PlayerDataMultiplayer playerData = playerDataNetworkList[playerDataIndex];

            playerData.playerName = playerName;

            playerDataNetworkList[playerDataIndex] = playerData;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default) {
            int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

            PlayerDataMultiplayer playerData = playerDataNetworkList[playerDataIndex];

            playerData.playerId = playerId;

            playerDataNetworkList[playerDataIndex] = playerData;
        }
        public NetworkList<PlayerDataMultiplayer> GetPlayerDataNetworkList() {
            return playerDataNetworkList;
        }

        public bool IsPlayerIndexConnected(int playerIndex) {
            return playerIndex < playerDataNetworkList.Count;
        }

        public int GetPlayerDataIndexFromClientId(ulong clientId) {
            for (int i=0; i< playerDataNetworkList.Count; i++) {
                if (playerDataNetworkList[i].clientId == clientId) {
                    return i;
                }
            }
            return -1;
        }

        public PlayerDataMultiplayer GetPlayerDataFromClientId(ulong clientId) {
            foreach (PlayerDataMultiplayer playerDataMultiplayer in playerDataNetworkList) {
                Debug.Log("GetPlayerDataFromClientId " + playerDataMultiplayer.clientId);
                Debug.Log("playerData" + playerDataMultiplayer);
                if (playerDataMultiplayer.clientId == clientId) {
                    return playerDataMultiplayer;
                }
            }
            return default;
        }

        public PlayerDataMultiplayer GetPlayerData() {
            return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
        }

        public PlayerDataMultiplayer GetPlayerDataFromPlayerIndex(int playerIndex) {
            return playerDataNetworkList[playerIndex];
        }

        public GameObject GetPlayerPrefab(int playerIndex) {
            Debug.Log("GetPlayerPrefab " + playerIndex);
            Debug.Log("GetPlayerPrefab Success " + playerPrefabsList[playerIndex]);
            return playerPrefabsList[playerIndex];
        }

        public void ChangePlayerPrefab(int prefabId)
        {
            ChangePlayerPrefabServerRpc(prefabId);
        }
        public CharacterStat GetUpdateCharacterInfo(int prefabId) {
            if (characterStatDictionary.TryGetValue(prefabId, out var characterStat))
            {
                return characterStat;
            }

            Debug.Log("Character Stat not found");
            return null;
        }


        [ServerRpc(RequireOwnership = false)]
        private void ChangePlayerPrefabServerRpc(int prefabId, ServerRpcParams serverRpcParams = default)
        {
            // Validasi apakah prefab tersedia
            if (!IsPrefabAvailable(prefabId))
            {
                return;
            }

            int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

            PlayerDataMultiplayer playerDataMultiplayer = playerDataNetworkList[playerDataIndex];

            playerDataMultiplayer.prefabId = prefabId;

            playerDataNetworkList[playerDataIndex] = playerDataMultiplayer;
        }

        private bool IsPrefabAvailable(int prefabId)
        {
            foreach (PlayerDataMultiplayer playerDataMultiplayer in playerDataNetworkList)
            {
                if (playerDataMultiplayer.prefabId == prefabId)
                {
                    return false; 
                }
            }
            return true;
        }

        private int GetFirstUnusedPrefabId()
        {
            for (int i = 0; i < playerPrefabsList.Count; i++)
            {
                if (IsPrefabAvailable(i))
                {
                    return i;
                }
            }
            return -1;
        }

        

    }
}
