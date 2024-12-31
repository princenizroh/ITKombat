using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using VContainer;
// using Unity.Services.Multiplay;
using UnityEngine;
using QFSW.QC.Actions;
using Unity.VisualScripting;

namespace ITKombat
{
    public class ServerBattleRoomState : NetworkBehaviour
    {
        public static ServerBattleRoomState Instance { get; private set; }
        
        // [SerializeField] PersistentGameState persistentGameState;
        private MatchManager matchManager;
        
        [Tooltip("Tempat Spawn player berada")]
        public event EventHandler OnStateChanged;
        public event EventHandler OnLocalGamePaused;
        public event EventHandler OnLocalGameUnpaused;
        public event EventHandler OnMultiplayerGamePaused;
        public event EventHandler OnMultiplayerGameUnpaused;
        public event EventHandler OnLocalPlayerReadyChanged;

        [SerializeField] private Transform[] playerSpawnPoints;
        private List<Transform> playerSpawnPointsList = null;
        [SerializeField] private GameObject playerPrefab;

        // Network Variable untuk menyimpan state game
        public NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
        
        // private State state;
        [SerializeField] private NetworkVariable<RoundState> roundState = new NetworkVariable<RoundState>(RoundState.Round1);
        [SerializeField] private NetworkVariable<WinState> winState = new NetworkVariable<WinState>(WinState.Invalid);
        [SerializeField] private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(5f);
        private NetworkVariable<bool> isLocalPlayerReady = new NetworkVariable<bool>(false);

        private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(2f);

        private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
        [SerializeField] private const float gamePlayingTimerMax = 5f;
        private bool isLocalGamePaused = false;
        private bool isCountdownCoroutineStarted = false;
        private bool isRoundOutcomeDetermined = false;
        
        private Dictionary<ulong, bool> playerReadyDictionary;
        private Dictionary<ulong, bool> playerPausedDictionary;
        private bool autoTestGamePausedState;

        [Inject] PersistentGameState m_PersistentGameState;

        
        private void Awake() {
            Instance = this;
            playerReadyDictionary = new Dictionary<ulong, bool>();
            playerPausedDictionary = new Dictionary<ulong, bool>();
            Debug.Log("ServerBattleRoomState Awake");
            Debug.Log("Current State: " + state.Value);
        }

        private void Start() {
            Debug.Log("ServerBattleRoomState Start from ServerBattleRoomState");
            matchManager = MatchManager.Instance;
            if (matchManager == null)
            {
                Debug.LogError("MatchManager instance is null in ServerBattleRoomState.");
            }
            // ChangeState(State.CountdownToStart);
            // GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
            Debug.Log("ServerBattleRoomState Start");

    #if DEDICATED_SERVER
            // await MultiplayService.Instance.UnreadyServerAsync();
            
            Camera.main.enabled = false;
    #endif
        }
        public void ChangeState(State newState) {
            if (state.Value != newState)
            {
                state.Value = newState;
                Debug.Log("State changed to: " + newState);
                OnStateChanged?.Invoke(this, EventArgs.Empty);
            }
            
        }
        public override void OnNetworkSpawn() {
            state.OnValueChanged += State_OnValueChanged;
            state.CheckDirtyState();

            Debug.Log("IsServer: " + IsServer);
            if (IsServer) {
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
                Debug.Log("ServerBattleRoomState OnNetworkSpawn, ConnectedClientsIds: " + string.Join(", ", NetworkManager.Singleton.ConnectedClientsIds));                
                Debug.Log("ServerBattleRoomState OnNetworkSpawn, connectedHostIds: " + string.Join(", ", NetworkManager.Singleton.ConnectedHostname));
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
            
            }
        }

        private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
            SpawnPlayers();
        }
        private void SpawnPlayers() {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList) {
                var spawnPoint = GetSpawnPoint();
                if (spawnPoint == null) {
                    Debug.LogError("No spawn point available.");
                    return;
                }

                PlayerDataMultiplayer playerData = GameMultiplayerManager.Instance.GetPlayerDataFromClientId(client.ClientId);
                if (playerData.Equals(default(PlayerDataMultiplayer))) {
                    Debug.LogError("Player data not found for clientId: " + client.ClientId);
                    continue;
                }

                var playerPrefab = GameMultiplayerManager.Instance.GetPlayerPrefab(playerData.prefabId);
                if (playerPrefab == null) {
                    Debug.LogError("Player prefab not found for prefabId: " + playerData.prefabId);
                    continue;
                }

                var playerInstance = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
                var networkObject = playerInstance.GetComponent<NetworkObject>();
                if (networkObject == null) {
                    Debug.LogError("Player prefab does not have a NetworkObject component.");
                    return;
                }
                // Change Tag
                if (playerData.clientId == NetworkManager.Singleton.LocalClientId) {
                    playerInstance.tag = "Player";
                } else {
                    playerInstance.tag = "Enemy";
                }

                networkObject.SpawnAsPlayerObject(client.ClientId);
            }
        }

        private Transform GetSpawnPoint() {
            if (playerSpawnPointsList == null || playerSpawnPointsList.Count == 0) {
                playerSpawnPointsList = new List<Transform>(playerSpawnPoints);
            }

            if (playerSpawnPointsList.Count == 0) {
                Debug.LogError("No spawn points available.");
                return null;
            }

            var spawnPoint = playerSpawnPointsList[0];
            playerSpawnPointsList.RemoveAt(0);
            return spawnPoint;
        }
        

        private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
            Debug.Log("Client disconnected: " + clientId);
            autoTestGamePausedState = true;
        }

        // private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue) {
        //     Debug.Log("IsGamePaused_OnValueChanged: " + newValue);
        //     if (isGamePaused.Value) {
        //         Time.timeScale = 0f;

        //         OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
        //     } else {
        //         Time.timeScale = 1f;

        //         OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);
        //     }
        // }

        private void State_OnValueChanged(State previousValue, State newValue) {
            Debug.Log("State_OnValueChanged: " + previousValue);
            Debug.Log("State_OnValueChanged: " + newValue);
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void OnInteractAction(object sender, EventArgs e) {
            Debug.Log("OnInteractAction");
            if (state.Value == State.WaitingToStart) {
                Debug.Log("State from GameInputOnInteractAction: " + state.Value);
                Debug.Log("Local player is ready." + isLocalPlayerReady);
                isLocalPlayerReady.Value = true;
                OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);

                Debug.Log("After Local player is ready." + isLocalPlayerReady);
                SetPlayerReadyServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
            playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
            Debug.Log($"Player {serverRpcParams.Receive.SenderClientId} is ready.");

            bool allClientsReady = true;
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
                if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId]) {
                    // This player is NOT ready
                    allClientsReady = false;
                    break;
                }
            }

            Debug.Log("All clients ready: " + allClientsReady);
            
            if (allClientsReady) {
                Debug.Log("All players are ready. Starting countdown.");
                state.Value = State.CountdownToStart;
            }
        }

        // private void GameInput_OnPauseAction(object sender, EventArgs e) {
        //     Debug.Log("GameInput_OnPauseAction");
        //     TogglePauseGame();
        // }

        private void Update() {
            if (!IsServer) {
                return;
            }

            switch (state.Value) {
                case State.WaitingToStart:
                    break;
                case State.CountdownToStart:
                    countdownToStartTimer.Value -= Time.deltaTime;
                    Debug.Log("Countdown to start: " + countdownToStartTimer.Value);
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                    if (countdownToStartTimer.Value <= 0f) {
                        Debug.Log("Countdown to start finished." + countdownToStartTimer.Value);
                        state.Value = State.GamePlaying;
                        gamePlayingTimer.Value = gamePlayingTimerMax;
                    }
                    break;
                case State.GamePlaying:
                    gamePlayingTimer.Value -= Time.deltaTime;
                    if (gamePlayingTimer.Value <= 0f && !isRoundOutcomeDetermined) {
                        OnStateChanged?.Invoke(this, EventArgs.Empty);
                        isRoundOutcomeDetermined = true;
                        state.Value = State.WaitingToStart;
                        DetermineRoundOutcome();

                        Debug.Log("State winState: " + state.Value);
                        Debug.Log("Next round.");
                        if (winState.Value == WinState.Victory || winState.Value == WinState.Defeat) {
                            Debug.Log("Game over.");
                        }
                    }
                    break;
                case State.GameOver:
                    break;
            }
        }

        // private void LateUpdate() {
        //     if (autoTestGamePausedState) {
        //         autoTestGamePausedState = false;
        //         TestGamePausedState();
        //     }
        // }

        private void DetermineRoundOutcome()
        {
            if (matchManager.playerState.currentHealth.Value == matchManager.enemyState.currentHealth.Value)
            {
                Debug.Log("Draw");
                winState.Value = WinState.Draw;
                Debug.Log("State winState: " + winState.Value);
                Debug.Log("State: " + state.Value);
            }
            else if (matchManager.playerState.currentHealth.Value > matchManager.enemyState.currentHealth.Value)
            {
                Debug.Log("Player 1 win");
                winState.Value = WinState.Player1Win;
            }
            else if (matchManager.playerState.currentHealth.Value < matchManager.enemyState.currentHealth.Value)
            {
                Debug.Log("Player 2 win");
                winState.Value = WinState.Player2Win;
            }

            if (matchManager.playerVictoryPoint == 3 || matchManager.enemyVictoryPoint == 3)
            {
                Debug.Log("Game over");
                state.Value = State.GameOver;
                StartCoroutine(HandleGameOver());
            }
            // else
            // {
            //     StartCoroutine(HandleRoundTransition());
            // }
        }
        private IEnumerator HandleRoundTransition()
        {
            yield return new WaitForSeconds(3f);
            state.Value = State.CountdownToStart;
            countdownToStartTimer.Value = 3f;

            // Update round state
            switch (roundState.Value)
            {
                case RoundState.Round1:
                    roundState.Value = RoundState.Round2;
                    break;
                case RoundState.Round2:
                    roundState.Value = RoundState.Round3;
                    break;
                case RoundState.Round3:
                    roundState.Value = RoundState.Round4;
                    break;
                case RoundState.Round4:
                    roundState.Value = RoundState.FinalRound;
                    break;
                case RoundState.FinalRound:
                    state.Value = State.GameOver;
                    break;
            }

            gamePlayingTimer.Value = gamePlayingTimerMax;
        }

        private IEnumerator HandleGameOver()
        {
            yield return new WaitForSeconds(3f);
            if (winState.Value == WinState.Player1Win || winState.Value == WinState.Player2Win)
            {
                matchManager.ShowVictoryNotif(winState.Value == WinState.Player1Win);
            }
            else
            {
                matchManager.ShowVictoryNotif(false);
            }
            yield return new WaitForSeconds(2f);
            Loader.Load(Loader.Scene.LobbyScene);
        }

        public bool IsGamePlaying() {
            return state.Value == State.GamePlaying;
        }

        public bool IsCountdownToStartActive() {
            return state.Value == State.CountdownToStart && countdownToStartTimer.Value > 0f;
        }

        public bool IsCountdownToStartFinished() {
            return state.Value == State.CountdownToStart && countdownToStartTimer.Value < 0f;
        }

        public float GetCountdownToStartTimer() {
            return countdownToStartTimer.Value;
        }

        public float GetResetCountdownToStartTimer() {
            countdownToStartTimer.Value = 5f;
            return countdownToStartTimer.Value;
        }
        public bool IsGameOver() {
            return state.Value == State.GameOver;
        }

        public bool IsWaitingToStart() {
            Debug.Log("State: " + state.Value);
            return state.Value == State.WaitingToStart;
        }

        public bool IsLocalPlayerReady() {
            Debug.Log("IsLocalPlayerReady: " + isLocalPlayerReady);
            return isLocalPlayerReady.Value;
        }

        public int GetGamePlayingTimerNormalized() {
            float normalizedTime = Mathf.Max(0, gamePlayingTimer.Value);
            return Mathf.FloorToInt(normalizedTime);
            // return 3 - (gamePlayingTimer.Value / gamePlayingTimerMax);
        }

        public int GetResetGamePlayingTimerNormalized() {
            return Mathf.FloorToInt(gamePlayingTimerMax);
        }

        // public void TogglePauseGame() {
        //     isLocalGamePaused = !isLocalGamePaused;
        //     if (isLocalGamePaused) {
        //         PauseGameServerRpc();

        //         OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        //     } else {
        //         UnpauseGameServerRpc();

        //         OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
        //     }
        //     Debug.Log("Game paused state toggled: " + isLocalGamePaused);
        // }

        // [ServerRpc(RequireOwnership = false)]
        // private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default) {
        //     playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;
        //     Debug.Log($"Player {serverRpcParams.Receive.SenderClientId} paused the game.");

        //     TestGamePausedState();
        // }

        // [ServerRpc(RequireOwnership = false)]
        // private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default) {
        //     playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;
        //     Debug.Log($"Player {serverRpcParams.Receive.SenderClientId} unpaused the game.");

        //     TestGamePausedState();
        // }

        // private void TestGamePausedState() {
        //     foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
        //         if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId]) {
        //             // This player is paused
        //             isGamePaused.Value = true;
        //             Debug.Log("Game is paused by player: " + clientId);
        //             return;
        //         }
        //     }

        //     // All players are unpaused
        //     isGamePaused.Value = false;
        //     Debug.Log("All players are unpaused. Game is resumed.");
        // }

    }
}
