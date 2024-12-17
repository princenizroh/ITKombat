using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
// using Unity.Services.Multiplay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameLobby : MonoBehaviour {


    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";


    public static KitchenGameLobby Instance { get; private set; }


    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler OnJoinFailed;
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs {
        public List<Lobby> lobbyList;
    }



    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float listLobbiesTimer;

#if DEDICATED_SERVER
    private float autoAllocateTimer = 9999999f;
    private bool alreadyAutoAllocated;
    private static IServerQueryHandler serverQueryHandler; // static so it doesn't get destroyed when this object is destroyed
    private string backfillTicketId;
    private float acceptBackfillTicketsTimer;
    private float acceptBackfillTicketsTimerMax = 1.1f;
    private PayloadAllocation payloadAllocation;
#endif


    private void Awake() {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentication();
    }

    private void Start() {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.OnInstanceCreated += CharacterSelectReady_OnInstanceCreated;
    }

    private void CharacterSelectReady_OnInstanceCreated(object sender, EventArgs e) {
        CharacterSelectReady.Instance.OnGameStarting += CharacterSelectReady_OnGameStarting;
    }

    private async void CharacterSelectReady_OnGameStarting(object sender, EventArgs e) {
#if DEDICATED_SERVER
        if (backfillTicketId != null) {
            await MatchmakerService.Instance.DeleteBackfillTicketAsync(backfillTicketId);
        }
#endif
    }

    private async void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e) {
#if DEDICATED_SERVER
        HandleUpdateBackfillTickets();

        if (KitchenGameMultiplayer.Instance.HasAvailablePlayerSlots()) {
            await MultiplayService.Instance.ReadyServerForPlayersAsync();
        } else {
            await MultiplayService.Instance.UnreadyServerAsync();
        }
#endif
    }

    private async void InitializeUnityAuthentication() {
        if (UnityServices.State != ServicesInitializationState.Initialized) {
            InitializationOptions initializationOptions = new InitializationOptions();
#if !DEDICATED_SERVER
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());
#endif

            await UnityServices.InitializeAsync(initializationOptions);

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
            IServerEvents serverEvents = await MultiplayService.Instance.SubscribeToServerEventsAsync(multiplayEventCallbacks);

            serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(4, "MyServerName", "KitchenChaos", "5.6", "Default");

            var serverConfig = MultiplayService.Instance.ServerConfig;
            if (serverConfig.AllocationId != "") {
                // Already Allocated
                MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));
            }
#endif
        } else {
            // Already Initialized
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

        SetupBackfillTickets();

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

        KitchenGameMultiplayer.Instance.StartServer();
        Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
    }

    private async void SetupBackfillTickets() {
        Debug.Log("SetupBackfillTickets");
        payloadAllocation = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<PayloadAllocation>();

        backfillTicketId = payloadAllocation.BackfillTicketId;
        Debug.Log("backfillTicketId: " + backfillTicketId);

        acceptBackfillTicketsTimer = acceptBackfillTicketsTimerMax;
    }

    private async void HandleUpdateBackfillTickets() {
        if (backfillTicketId != null && payloadAllocation != null && KitchenGameMultiplayer.Instance.HasAvailablePlayerSlots()) {
            Debug.Log("HandleUpdateBackfillTickets");

            List<Unity.Services.Matchmaker.Models.Player> playerList = new List<Unity.Services.Matchmaker.Models.Player>();

            foreach (PlayerData playerData in KitchenGameMultiplayer.Instance.GetPlayerDataNetworkList()) {
                playerList.Add(new Unity.Services.Matchmaker.Models.Player(playerData.playerId.ToString()));
            }

            MatchProperties matchProperties = new MatchProperties(
                payloadAllocation.MatchProperties.Teams, 
                playerList, 
                payloadAllocation.MatchProperties.Region, 
                payloadAllocation.MatchProperties.BackfillTicketId
            );

            try {
                await MatchmakerService.Instance.UpdateBackfillTicketAsync(payloadAllocation.BackfillTicketId,
                    new BackfillTicket(backfillTicketId, properties: new BackfillTicketProperties(matchProperties))
                );
            } catch (MatchmakerServiceException e) {
                Debug.Log("Error: " + e);
            }
        }
    }

    private async void HandleBackfillTickets() {
        if (KitchenGameMultiplayer.Instance.HasAvailablePlayerSlots()) {
            BackfillTicket backfillTicket = await MatchmakerService.Instance.ApproveBackfillTicketAsync(backfillTicketId);
            backfillTicketId = backfillTicket.Id;
        }
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

    private void Update() {
        HandleHeartbeat();
        HandlePeriodicListLobbies();

#if DEDICATED_SERVER
        autoAllocateTimer -= Time.deltaTime;
        if (autoAllocateTimer <= 0f) {
            autoAllocateTimer = 999f;
            MultiplayEventCallbacks_Allocate(null);
        }

        if (serverQueryHandler != null) {
            if (NetworkManager.Singleton.IsServer) {
                serverQueryHandler.CurrentPlayers = (ushort)NetworkManager.Singleton.ConnectedClientsIds.Count;
            }
            serverQueryHandler.UpdateServerCheck();
        }

        if (backfillTicketId != null) {
            acceptBackfillTicketsTimer -= Time.deltaTime;
            if (acceptBackfillTicketsTimer <= 0f) {
                acceptBackfillTicketsTimer = acceptBackfillTicketsTimerMax;
                HandleBackfillTickets();
            }
        }
#endif
    }

    private void HandlePeriodicListLobbies() {
        if (joinedLobby == null &&
            UnityServices.State == ServicesInitializationState.Initialized &&
            AuthenticationService.Instance.IsSignedIn &&
            SceneManager.GetActiveScene().name == Loader.Scene.LobbyScene.ToString()) {

            listLobbiesTimer -= Time.deltaTime;
            if (listLobbiesTimer <= 0f) {
                float listLobbiesTimerMax = 3f;
                listLobbiesTimer = listLobbiesTimerMax;
                ListLobbies();
            }
        }
    }


    private void HandleHeartbeat() {
        if (IsLobbyHost()) {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer <= 0f) {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private bool IsLobbyHost() {
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


    private async Task<Allocation> AllocateRelay() {
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiplayer.MAX_PLAYER_AMOUNT - 1);

            return allocation;
        } catch (RelayServiceException e) {
            Debug.Log(e);

            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation) {
        try {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            return relayJoinCode;
        } catch (RelayServiceException e) {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode) {
        try {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        } catch (RelayServiceException e) {
            Debug.Log(e);
            return default;
        }
    }


    public async void CreateLobby(string lobbyName, bool isPrivate) {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
        try {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiplayer.MAX_PLAYER_AMOUNT, new CreateLobbyOptions {
                IsPrivate = isPrivate,
            });

            Allocation allocation = await AllocateRelay();

            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                     { KEY_RELAY_JOIN_CODE , new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                 }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            KitchenGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        } catch (LobbyServiceException e) {
            Debug.Log(e);
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void QuickJoin() {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            KitchenGameMultiplayer.Instance.StartClient();
        } catch (LobbyServiceException e) {
            Debug.Log(e);
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void JoinWithId(string lobbyId) {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            KitchenGameMultiplayer.Instance.StartClient();
        } catch (LobbyServiceException e) {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void JoinWithCode(string lobbyCode) {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            KitchenGameMultiplayer.Instance.StartClient();
        } catch (LobbyServiceException e) {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void DeleteLobby() {
        if (joinedLobby != null) {
            try {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

                joinedLobby = null;
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }

    public async void LeaveLobby() {
        if (joinedLobby != null) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                joinedLobby = null;
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }

    public async void KickPlayer(string playerId) {
        if (IsLobbyHost()) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }


    public Lobby GetLobby() {
        return joinedLobby;
    }


}