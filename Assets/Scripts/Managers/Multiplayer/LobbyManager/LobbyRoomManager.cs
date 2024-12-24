
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Unity.Netcode; // Tambahkan ini
using Unity.Netcode.Transports.UTP; 
// using Unity.Services.Multiplay;
using TMPro;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

namespace ITKombat
{
    public class LobbyRoomManager : MonoBehaviour
    {
        public static LobbyRoomManager Instance { get; private set; }
        private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
        // public const string KEY_PLAYER_NAME = "PlayerName";
        // public const string KEY_PLAYER_CHARACTER = "Character";
        // public const string KEY_GAME_MODE = "GameMode";
        public event EventHandler OnLeftLobby;

        public event EventHandler<LobbyEventArgs> OnJoinedLobby;
        public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
        public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
        public event EventHandler<LobbyEventArgs> OnLobbyGameModeChanged;
        public event EventHandler<LobbyEventArgs> OnCreateLobbyStarted;
        public event EventHandler<LobbyEventArgs> OnCreateLobbyFailed;
        public event EventHandler<LobbyEventArgs> OnJoinStarted;
        public event EventHandler<LobbyEventArgs> OnJoinFailed;
        public event EventHandler<LobbyEventArgs> OnQuickJoinFailed;
        public class LobbyEventArgs : EventArgs {
            public Lobby lobby;
        }

        public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
        public class OnLobbyListChangedEventArgs : EventArgs {
            public List<Lobby> lobbyList;
        }


        public enum GameMode {
            Ranked,
            Unranked
        }

        public enum PlayerCharacter {
            Informatics,
            MachineEngineer,
        }
        
        private Lobby hostLobby;
        private Lobby joinedLobby;
        private float heartbeatTimer;
        private float listLobbiesTimer;
        private float lobbyPollTimer;
        private float refreshLobbyListTimer = 5f;
        private string playerId;
        private string playerEmail;
        private string playerUser;
        private FirebaseUser user;
        private FirebaseAuth auth;

    #if DEDICATED_SERVER
        private float autoAllocateTimer = 9999999f;
        private bool alreadyAutoAllocated;
        private static IServerQueryHandler serverQueryHandler; // static so it doesn't get destroyed when this object is destroyed
        private string backfillTicketId;
        private float acceptBackfillTicketsTimer;
        private float acceptBackfillTicketsTimerMax = 1.1f;
        private PayloadAllocation payloadAllocation;
    #endif
        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            // Uncomment ini jika sudah  tidak memerlukan BUTTON Authentication
            // Authenticate(GetFirebaseUser());
            DebugSignInAnonymously();
        }

        private void Start() 
        {
            
        }

        public string GetFirebaseUser()
        {
            return playerUser;
        }

        public async void Authenticate(string playerUser)
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                // Authentikasi
                InitializeFirebase(); // Inisialisasi Firebase Auth
                await WaitForFirebaseUser(); // Tunggu sampai Firebase User siap
                // Memastikan Firebase User sudah terisi
                if (user != null)
                {
                    playerId = user.UserId;  
                    playerEmail = user.Email;
                    playerUser = user.DisplayName;
                    Debug.Log("Firebase UserID already " + playerId);
                    Debug.Log("Firebase Email already " + playerEmail);
                    Debug.Log("Firebase Display Name already " + playerUser);
                   
                }
                else
                {
                    playerId = "Player_" + UnityEngine.Random.Range(10, 99);  // Default jika Firebase User belum siap
                    Debug.Log("Firebase User ID not ready yet. Using random player name: " + playerId);
                }

                
                Debug.Log("Player Name: " + playerId);

                // Inisialisasi Unity Services dengan profile playerUser
                InitializationOptions initializationOptions = new InitializationOptions();
                initializationOptions.SetProfile(playerUser ?? playerId);
                await UnityServices.InitializeAsync(initializationOptions);

                Debug.Log("Authentication Service " + AuthenticationService.Instance.PlayerId);
                Debug.Log("Player Name: " + playerId);
                Debug.Log("Player Email: " + playerEmail);
                Debug.Log("Player User: " + playerUser);

                // Dedicated Server
                #if !DEDICATED_SERVER
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                #endif

                #if DEDICATED_SERVER
                    Debug.Log("DEDICATED_SERVER LOBBY");

                    MultiplayEventCallbacks multiplayEventCallbacks = new MultiplayEventCallbacks();
                    multiplayEventCallbacks.Allocate += MultiplayEventCallbacks_Allocate;
                    multiplayEventCallbacks.Deallocate += MultiplayEventCallbacks_Deallocate;
                    multiplayEventCallbacks.Error += MultiplayEventCallbacks_Error;
                    multiplayEventCallbacks.SubscriptionStateChanged += MultiplayEventCallbacks_SubscriptionStateChanged;
                    Debug.Log("Berlangganan ke server event...");
                    IServerEvents serverEvents = await MultiplayService.Instance.SubscribeToServerEventsAsync(multiplayEventCallbacks);
                    Debug.Log("Langganan server event berhasil.");
                    // serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(2, "MyServerName", "ITKombat", "1.0", "Default");

                    // var serverConfig = MultiplayService.Instance.ServerConfig;
                    // Debug.Log($"Server ID[{serverConfig.ServerId}]");
                    // Debug.Log($"AllocationID[{serverConfig.AllocationId}]");
                    // Debug.Log($"Port[{serverConfig.Port}]");
                    // Debug.Log($"QueryPort[{serverConfig.QueryPort}]");
                    // Debug.Log($"LogDirectory[{serverConfig.ServerLogDirectory}]");
                    // if (serverConfig.AllocationId != "") {
                    //     // Already Allocated
                    //     MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));
                    // }
                #endif
            }  else {
        #if DEDICATED_SERVER
                Debug.Log("DEDICATED_SERVER LOBBY - ALREADY INIT");

                var serverConfig = MultiplayService.Instance.ServerConfig;
                if (serverConfig.AllocationId != "") {
                    // Already Allocated
                    MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));
                }
        #endif
            }
            
        }
    #if DEDICATED_SERVER
        private void MultiplayEventCallbacks_SubscriptionStateChanged(MultiplayServerSubscriptionState obj) {
            Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_SubscriptionStateChanged");
            Debug.Log(obj);
        }

        private void MultiplayEventCallbacks_Error(MultiplayError obj) {
            Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Error");
            Debug.Log(obj.Reason);
        }

        private void MultiplayEventCallbacks_Deallocate(MultiplayDeallocation obj) {
            Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Deallocate");
        }

        private void MultiplayEventCallbacks_Allocate(MultiplayAllocation obj) {
            Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Allocate");

            if (alreadyAutoAllocated) {
                Debug.Log("Already auto allocated!");
                return;
            }

            alreadyAutoAllocated = true;

            var serverConfig = MultiplayService.Instance.ServerConfig;
            Debug.Log($"Server ID[{serverConfig.ServerId}]");
            Debug.Log($"AllocationID[{serverConfig.AllocationId}]");
            Debug.Log($"Port[{serverConfig.Port}]");
            Debug.Log($"QueryPort[{serverConfig.QueryPort}]");
            Debug.Log($"LogDirectory[{serverConfig.ServerLogDirectory}]");

            string ipv4Address = "0.0.0.0";
            ushort port = serverConfig.Port;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port, "0.0.0.0");

            MultiplayerManager.Instance.StartServer();
            Loader.LoadNetwork(Loader.Scene.Multiplayer);
        }
        private async void SetupBackfillTickets() {
            Debug.Log("SetupBackfillTickets");
            payloadAllocation = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<PayloadAllocation>();

            backfillTicketId = payloadAllocation.BackfillTicketId;
            Debug.Log("backfillTicketId: " + backfillTicketId);

            acceptBackfillTicketsTimer = acceptBackfillTicketsTimerMax;
        }
         [Serializable]
        public class PayloadAllocation {
            public Unity.Services.Matchmaker.Models.MatchProperties MatchProperties;
            public string GeneratorName;
            public string QueueName;
            public string PoolName;
            public string EnvironmentId;
            public string BackfillTicketId;
            public string MatchId;
            public string PoolId;
        }
    #endif
        private async Task WaitForFirebaseUser()
        {
            while (auth.CurrentUser == null)
            {
                Debug.Log("Waiting for Firebase User...");
                await Task.Delay(500);  // Tunggu 500ms sebelum mengecek lagi
            }
            user = auth.CurrentUser;
            Debug.Log("Firebase User found: " + user.UserId);
        }
        private void Update()
        {
            HandleLobbyHeartbeat();
            HandlePeriodicListLobbies();
            // HandleLobbyPolling();
            ButtonLobby();
        }

        private void InitializeFirebase()
        {
            auth = FirebaseAuth.DefaultInstance;
            auth.StateChanged += AuthStateChanged;
        }



        private void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            if (auth.CurrentUser != user)
            {
                user = auth.CurrentUser;
                if (user != null)
                {
                    Debug.Log("Firebase User ID: " + user.UserId);
                    playerId = "Player_" + user.UserId;  // Gunakan Firebase User ID saat login
                    Debug.Log("Firebase User Email: " + user.Email);
                    playerEmail = "Email_" + user.Email;
                    Debug.Log("Firebase User Display Name: " + user.DisplayName);
                    playerUser = "User_" + user.DisplayName;
                }
            }
        }

        private async void HandleLobbyPolling() {
            if (joinedLobby != null) {
                lobbyPollTimer -= Time.deltaTime;
                if (lobbyPollTimer < 0f) {
                    float lobbyPollTimerMax = 1.1f;
                    lobbyPollTimer = lobbyPollTimerMax;

                    joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                    OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                    if (!IsPlayerInLobby()) {
                        // Player was kicked out of this lobby
                        Debug.Log("Kicked from Lobby!");

                        OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                        joinedLobby = null;
                    }
                }
            }
        }
        private async void HandleLobbyHeartbeat() {
            if (IsLobbyHost()) {
                heartbeatTimer -= Time.deltaTime;
                if (heartbeatTimer < 0f) {
                    float heartbeatTimerMax = 15f;
                    heartbeatTimer = heartbeatTimerMax;

                    Debug.Log("Heartbeat");
                    await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
                }
            }
        }

        private void HandlePeriodicListLobbies() {
            if (joinedLobby == null && AuthenticationService.Instance.IsSignedIn && 
                SceneManager.GetActiveScene().name == Loader.Scene.LobbyScene.ToString()) {
            // &&
            //     UnityServices.State == ServicesInitializationState.Initialized &&
            //     AuthenticationService.Instance.IsSignedIn &&
            //     SceneManager.GetActiveScene().name == Loader.Scene.LobbyScene.ToString()) {

                listLobbiesTimer -= Time.deltaTime;
                if (listLobbiesTimer <= 0f) {
                    float listLobbiesTimerMax = 3f;
                    listLobbiesTimer = listLobbiesTimerMax;
                    ListLobbies();
                }
            }
        }
        public bool IsLobbyHost() {
            // return joinedLobby != null && joinedLobby.HostId == user.UserId;
            return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
        }
        
        private async void ListLobbies() {
            try {
                QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions {
                    Filters = new List<QueryFilter> {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
                };
                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

                OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs {
                    lobbyList = queryResponse.Results
                });
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
        // private async Task<Allocation> AllocateRelay() {
        //     try {
        //         Allocation allocation = await RelayService.Instance.CreateAllocationAsync(GameMultiplayerManager.MAX_PLAYER_AMOUNT - 1);

        //         return allocation;
        //     } catch (RelayServiceException e) {
        //         Debug.Log(e);

        //         return default;
        //     }
        // }
        private bool IsPlayerInLobby() {
            if (joinedLobby != null && joinedLobby.Players != null) {
              Debug.Log("Player in Lobby");
                // foreach (Player player in joinedLobby.Players) {
                //     if (player.Id == user.UserId) {
                //         // This player is in this lobby
                //         return true;
                //     }
                // }
            }
            return false;
        }
        public async void CreateLobby(string lobbyName, bool isPrivate)
        {
            OnCreateLobbyStarted?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            try {
                joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, GameMultiplayerManager.MAX_PLAYER_AMOUNT, new CreateLobbyOptions {
                    IsPrivate = isPrivate,
                });

                Debug.Log("Create Lobby!" + joinedLobby.Name + "" + joinedLobby.MaxPlayers + " " + joinedLobby.Id + " " + joinedLobby.LobbyCode);

                GameMultiplayerManager.Instance.StartHost();
                Loader.LoadNetwork(Loader.Scene.SelectCharacterMultiplayer);
            } catch (LobbyServiceException e) {
                Debug.Log("Failed to create lobby: " + e.Message);
                OnCreateLobbyFailed?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
          
        }
        // private async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate, GameMode gamemode)
        // {
        //     try
        //     {
        //         // Player player = GetPlayer();
        //         CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
        //         {
        //             // Player = GetPlayer(),
        //             IsPrivate = isPrivate,
        //             Data = new Dictionary<string, DataObject>
        //             {
        //                 {
        //                     KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gamemode.ToString())
        //                 },
        //                 // {
        //                 //     "Map", new DataObject(DataObject.VisibilityOptions.Public, "Laboratorium")
        //                 // }
        //             }
        //         };
        //         // Create a new lobby
        //         Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
        //         joinedLobby = lobby;

        //         Debug.Log("Create Lobby!" + lobby.Name + "" + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
        //     }
        //     catch (LobbyServiceException e)
        //     {
        //         Debug.Log("Failed to create lobby: " + e.Message);
        //     }
        // }

        public async void RefreshLobbyList() {
            try {
                QueryLobbiesOptions options = new QueryLobbiesOptions();
                options.Count = 25;

                // Filter for open lobbies only
                options.Filters = new List<QueryFilter> {
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0")
                };

                // Order by newest lobbies first
                options.Order = new List<QueryOrder> {
                    new QueryOrder(
                        asc: false,
                        field: QueryOrder.FieldOptions.Created)
                };

                QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();

                Debug.Log("Lobbies found: " + lobbyListQueryResponse.Results.Count);

                OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }


        private void ButtonLobby()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                QuickJoinLobby();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                // CreateLobby();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                RefreshLobbyList();
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                UpdateLobbyGameMode("Unranked");
            }
        }

        private async void JoinLobbyByCode(string lobbyCode)
        {
            try
            {
                JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
                {
                    // Player = GetPlayer()
                };
                Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
                joinedLobby = lobby;

                Debug.Log("Joined lobby: " + lobbyCode);

                PrintPlayers(joinedLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log("Failed to join lobby: " + e.Message);
            }
        }

        public async void QuickJoinLobby()
        {
            OnJoinStarted?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            try
            {
                joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                GameMultiplayerManager.Instance.StartClient();
                Debug.Log("Quick join lobby");
            } 
            catch (LobbyServiceException e)
            {
                Debug.Log("Failed to quick join lobby: " + e.Message);
                OnQuickJoinFailed?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
        }

        // private Player GetPlayer()
        // {
        //     return new Player(user.UserId, null, new Dictionary<string, PlayerDataObject>
        //     {
        //         { 
        //             KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerUser) 
        //         },
        //         { 
        //             KEY_PLAYER_CHARACTER, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerCharacter.Informatics.ToString())  // Menyimpan Firebase User ID di sini
        //         }
        //     });
        // }

        private void PrintPlayers()
        {
            PrintPlayers(joinedLobby);
        }

        private void PrintPlayers(Lobby lobby)
        {
            // Debug.Log("Players in lobby: " + lobby.Name + " " + lobby.Data["GameMode"].Value + "" + lobby.Data["Map"].Value);
            // foreach (Player player in lobby.Players)
            // {
            //     Debug.Log("Player: " + player.Id + " " + player.Data["PlayerName"].Value);
            // }
        }

        private async void UpdateLobbyGameMode(string gameMode)
        {
            try
            {
                hostLobby =await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {
                            "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode)
                        }
                    }
                });
                joinedLobby = hostLobby;
                PrintPlayers(hostLobby);
            } 
            catch (LobbyServiceException e)
            {
                Debug.Log("Failed to update lobby: " + e.Message);
            }
        }

        private async void UpdatePlayerName(string newPlayerName)
        {
            try
            {
                playerId = newPlayerName;
                await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {
                            "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerId)
                        }
                    }
                });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log("Failed to update player: " + e.Message);
            }
        }

        private async void LeaveLobbyExists()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                Debug.Log("Left lobby");
            }
            catch (LobbyServiceException e)
            {
                Debug.Log("Failed to leave lobby: " + e.Message);
            }
        }

        public async void LeaveLobby()
        {
            if (joinedLobby != null)
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                    joinedLobby = null;
                    Debug.Log("Left lobby");
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log("Failed to leave lobby: " + e.Message);
                }
            }
        }

        private async void KickPlayer()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log("Failed to kick player: " + e.Message);
            }
        }

        private async void MigrateLobbyHost()
        {
            try
              {
                  hostLobby = await LobbyService.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
                  {
                      HostId = joinedLobby.Players[1].Id
                  });
                  joinedLobby = hostLobby;

              }
              catch (LobbyServiceException e)
              {
                  Debug.Log("Failed to migrate lobby host: " + e.Message);
              }
        }

        private async void DeleteLobbyExist()
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
            } 
            catch (LobbyServiceException e)
            {
                Debug.Log("Failed to delete lobby: " + e.Message);
            }
        }

        public async void DeleteLobby()
        {
            if (joinedLobby == null)
            {
                try
                {
                    await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                    joinedLobby = null;
                } 
                catch (LobbyServiceException e)
                {
                    Debug.Log("Failed to delete lobby: " + e.Message);
                }
            }
 
        }
        

                // ini untuk debugging
        private async void DebugSignInAnonymously()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized) 
            {
                InitializationOptions initializationOptions = new InitializationOptions();

                initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

                await UnityServices.InitializeAsync(initializationOptions);

                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
        private async void DebugAuth()
        {
            
            InitializeFirebase();

            await UnityServices.InitializeAsync();
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in to Unity Services" + AuthenticationService.Instance.PlayerId);
                Debug.Log("Firebase ID: " + user.UserId);
                Debug.Log("Firebase Email: " + user.Email);
                Debug.Log("Firebase Display Name" + user.DisplayName);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
            // Gunakan Firebase user ID sebagai bagian dari player name atau ID
            if (user != null)
            {
                // Debug.Log("Firebase UserID already   " + playerId);
                playerId = user.UserId;  // Gunakan Firebase User ID untuk player name
                playerEmail = user.Email;
                playerUser = user.DisplayName;
                Debug.Log("Firebase UserID already " + playerId);
                Debug.Log("Firebase Email already " + playerEmail);
                Debug.Log("Firebase Display Name already " + playerUser);
            }
            else
            {
                playerId = "Player_" + UnityEngine.Random.Range(10, 99);  // Default jika Firebase User belum siap
                Debug.Log("Firebase User ID not ready yet. Using random player name: " + playerId);
            }

            Debug.Log("Player Name: " + playerId);
            Debug.Log("Player Email: " + playerEmail);
        }
        //
        void LogConnectionEvent(NetworkManager manager, ConnectionEventData data)
        {
            switch (data.EventType)
            {
                case ConnectionEvent.ClientConnected:
                    FindFirstObjectByType<TMP_Text>().SetText("Client Connected" + data.ClientId + "Count: " + NetworkManager.Singleton.ConnectedClientsIds.Count + " Port:" + (manager.NetworkConfig.NetworkTransport as UnityTransport)?.ConnectionData.Port);
                    break;
            }
            
        }
        public async void JoinWithId(string lobbyId) {
            OnJoinStarted?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            try {
                joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

                // string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

                // JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

                // NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

                GameMultiplayerManager.Instance.StartClient();
            } catch (LobbyServiceException e) {
                Debug.Log(e);
                OnJoinFailed?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
        }
        public async void JoinWithCode(string lobbyCode)
        {
            OnJoinStarted?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            try {
                joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            } catch (LobbyServiceException e) {
                Debug.Log("Failed to join lobby by code: " + e.Message);
                OnJoinFailed?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
        }
        public Lobby GetLobby()
        {
            return joinedLobby;
        }
        
    }
}
