using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using VContainer;
using Unity.Services.Multiplay;
using UnityEngine;
using QFSW.QC.Actions;
using Unity.VisualScripting;

namespace ITKombat
{
    public class ServerBattleRoomState : NetworkBehaviour
    {
        public static ServerBattleRoomState Instance { get; private set; }
        
        // [SerializeField] PersistentGameState persistentGameState;
        [SerializeField] private MatchManager matchManager;
        
        [Tooltip("Tempat Spawn player berada")]
        [SerializeField] private Transform[] m_PlayerSpawnPoints;
        private List<Transform> m_PlayerSpawnPointsList = null;
        public event EventHandler OnStateChanged;
        public event EventHandler OnLocalGamePaused;
        public event EventHandler OnLocalGameUnpaused;
        public event EventHandler OnMultiplayerGamePaused;
        public event EventHandler OnMultiplayerGameUnpaused;
        public event EventHandler OnLocalPlayerReadyChanged;

        [SerializeField] private Transform playerPrefab;
        private bool isLocalPlayerReady;

        // Network Variable untuk menyimpan state game
        private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
        private NetworkVariable<RoundState> roundState = new NetworkVariable<RoundState>(RoundState.Round1);
        private NetworkVariable<WinState> winState = new NetworkVariable<WinState>(WinState.Invalid);
        
        private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
        private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);

        private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
        private float gamePlayingTimerMax = 100f;
        private bool isLocalGamePaused = false;
        
        private Dictionary<ulong, bool> playerReadyDictionary;
        private Dictionary<ulong, bool> playerPausedDictionary;
        private bool autoTestGamePausedState;

        [Inject] PersistentGameState m_PersistentGameState;
        [SerializeField ]private GameObject quantumConsole;

        private void Awake() {
            Instance = this;

            playerReadyDictionary = new Dictionary<ulong, bool>();
            playerPausedDictionary = new Dictionary<ulong, bool>();
            Debug.Log("ServerBattleRoomState Awake");
        }

        private void Start() {
            GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
            GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;

            Debug.Log("ServerBattleRoomState Start");

    #if DEDICATED_SERVER
            // await MultiplayService.Instance.UnreadyServerAsync();
            
            Camera.main.enabled = false;
    #endif
        }

        public override void OnNetworkSpawn() {
            state.OnValueChanged += State_OnValueChanged;
            isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

            if (IsServer) {
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
            }

            Debug.Log("ServerBattleRoomState OnNetworkSpawn");
        }

        private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
            Debug.Log("SceneManager_OnLoadEventCompleted: " + sceneName);
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
                Transform playerTransform = Instantiate(playerPrefab);
                playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
                Debug.Log("Player spawned for clientId: " + clientId);
            }
        }

        private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
            Debug.Log("Client disconnected: " + clientId);
            autoTestGamePausedState = true;
        }

        private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue) {
            Debug.Log("IsGamePaused_OnValueChanged: " + newValue);
            if (isGamePaused.Value) {
                Time.timeScale = 0f;

                OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
            } else {
                Time.timeScale = 1f;

                OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);
            }
        }

        private void State_OnValueChanged(State previousValue, State newValue) {
            Debug.Log("State_OnValueChanged: " + newValue);
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void GameInput_OnInteractAction(object sender, EventArgs e) {
            Debug.Log("GameInput_OnInteractAction");
            if (state.Value == State.WaitingToStart) {
                isLocalPlayerReady = true;
                OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);

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

            if (allClientsReady) {
                Debug.Log("All players are ready. Starting countdown.");
                state.Value = State.CountdownToStart;
            }
        }

        private void GameInput_OnPauseAction(object sender, EventArgs e) {
            Debug.Log("GameInput_OnPauseAction");
            TogglePauseGame();
        }

        private void Update() {
            if (!IsServer) {
                return;
            }

            switch (state.Value) {
                case State.WaitingToStart:
                    break;
                case State.CountdownToStart:
                    countdownToStartTimer.Value -= Time.deltaTime;
                    if (countdownToStartTimer.Value < 0f) {
                        state.Value = State.GamePlaying;
                        gamePlayingTimer.Value = gamePlayingTimerMax;
                        Debug.Log("Countdown finished. Game started.");
                    }
                    break;
                case State.GamePlaying:
                    gamePlayingTimer.Value -= Time.deltaTime;
                    if (gamePlayingTimer.Value <= 0f) {
                        DetermineRoundOutcome();
                        Debug.Log("Game over.");
                    }
                    break;
                case State.GameOver:
                    break;
            }
        }

        private void LateUpdate() {
            if (autoTestGamePausedState) {
                autoTestGamePausedState = false;
                TestGamePausedState();
            }
        }

        private void DetermineRoundOutcome()
        {
            if (matchManager.playerVictoryPoint > matchManager.enemyVictoryPoint)
            {
                winState.Value = WinState.Player1Win;
                matchManager.PlayerVictory();
            }
            else if (matchManager.playerVictoryPoint < matchManager.enemyVictoryPoint)
            {
                winState.Value = WinState.Player2Win;
                matchManager.EnemyVictory();
            }
            else
            {
                winState.Value = WinState.Draw;
                matchManager.DrawRound();
            }

            if (matchManager.playerVictoryPoint == 3 || matchManager.enemyVictoryPoint == 3)
            {
                state.Value = State.GameOver;
                StartCoroutine(HandleGameOver());
            }
            else
            {
                StartCoroutine(HandleRoundTransition());
            }
        }
        private IEnumerator HandleRoundTransition()
        {
            yield return new WaitForSeconds(3f);
            state.Value = State.CountdownToStart;
            countdownToStartTimer.Value = 5f;

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
            return state.Value == State.CountdownToStart;
        }

        public float GetCountdownToStartTimer() {
            return countdownToStartTimer.Value;
        }

        public bool IsGameOver() {
            return state.Value == State.GameOver;
        }

        public bool IsWaitingToStart() {
            return state.Value == State.WaitingToStart;
        }

        public bool IsLocalPlayerReady() {
            return isLocalPlayerReady;
        }

        public float GetGamePlayingTimerNormalized() {
            return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
        }

        public void TogglePauseGame() {
            isLocalGamePaused = !isLocalGamePaused;
            if (isLocalGamePaused) {
                PauseGameServerRpc();

                OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
            } else {
                UnpauseGameServerRpc();

                OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
            }
            Debug.Log("Game paused state toggled: " + isLocalGamePaused);
        }

        [ServerRpc(RequireOwnership = false)]
        private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default) {
            playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;
            Debug.Log($"Player {serverRpcParams.Receive.SenderClientId} paused the game.");

            TestGamePausedState();
        }

        [ServerRpc(RequireOwnership = false)]
        private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default) {
            playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;
            Debug.Log($"Player {serverRpcParams.Receive.SenderClientId} unpaused the game.");

            TestGamePausedState();
        }

        private void TestGamePausedState() {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
                if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId]) {
                    // This player is paused
                    isGamePaused.Value = true;
                    Debug.Log("Game is paused by player: " + clientId);
                    return;
                }
            }

            // All players are unpaused
            isGamePaused.Value = false;
            Debug.Log("All players are unpaused. Game is resumed.");
        }

    }
}
