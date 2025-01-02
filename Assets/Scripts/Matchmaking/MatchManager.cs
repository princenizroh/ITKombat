using TMPro;
using UnityEngine;
using System.Collections;

namespace ITKombat
{
    public class MatchManager : MonoBehaviour
    {
        public static MatchManager Instance;
        public GameObject ReadyNotif, Round1Notif, Round2Notif, Round3Notif, Round4Notif, FinalRoundNotif, DrawRoundNotif, FightNotif, DefeatNotif, VictoryNotif, TimeoutNotif;
        public GameObject Reward;
        public MatchTimer matchTimer;
        public TMP_Text timerText, timeoutToTimer, vpPlayer, vpEnemy;
        public bool timeoutTriggered = false;
        public bool timeoutTimer = false;
        public bool finalRound = false;
        private bool isSoundFight = false;
        private PlayerMovement_2 playerMovement;
        private ServerCharacterMovement serverCharacterMovement;

        private PlayerIFAttack playerAttack;


        public GameObject initialPlayerPosition;
        public GameObject initialEnemyPosition;

        // Reference to PlayerState and EnemyState
        public PlayerState playerState;
        public EnemyState enemyState;
        public SoundManager soundManager;

        private int currentRound = 1;
        private bool isCountdownCoroutineStarted = false;
        
        void Start() 
        {
            ServerBattleRoomState.Instance.OnStateChanged += ServerBattleRoomState_OnStateChanged;
            ServerBattleRoomState.Instance.OnPlayerVictoryPointChanged += OnPlayerVictoryPointChanged;
            ServerBattleRoomState.Instance.OnEnemyVictoryPointChanged += OnEnemyVictoryPointChanged;

            // StartCoroutine(ShowRoundStartNotification(1));
            StartCoroutine(WaitForPlayer());
        }

        private void OnPlayerVictoryPointChanged(object sender, System.EventArgs e)
        {
            Debug.Log("Player Victory Point Changed");
            vpPlayer.text = ServerBattleRoomState.Instance.GetPlayerVictoryPoint().ToString();
        }

        private void OnEnemyVictoryPointChanged(object sender, System.EventArgs e)
        {
            Debug.Log("Enemy Victory Point Changed");
            vpEnemy.text = ServerBattleRoomState.Instance.GetEnemyVictoryPoint().ToString();
        }
        private void Awake()
        {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            } else {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            Instance = this;
            playerState = FindFirstObjectByType<PlayerState>();
            enemyState = FindFirstObjectByType<EnemyState>();
        }

        
        void Update()
        {
            if (ServerBattleRoomState.Instance != null)
            {
                UpdateTimerUI();
            }
            // timerText.text = matchTimer.GetStageTimeInSecond().ToString();
            

            // vpPlayer.text = playerVictoryPoint.ToString();
            // vpEnemy.text = enemyVictoryPoint.ToString();
            
            // if (matchTimer.GetStageTimeInSecond() == 0 && !timeoutTriggered) 
            if(ServerBattleRoomState.Instance.IsWaitingTime() && !timeoutTriggered)
            {
                Debug.Log("Timeout Triggered");
                HandleTimeout();
            } 
            else if (timeoutTimer == true) 
            {   
                HandleTimeoutTimer();
            }
            
        }

        private void UpdateTimerUI()
        {
            float gamePlayingTimer = ServerBattleRoomState.Instance.GetGamePlayingTimerNormalized();
            if (ServerBattleRoomState.Instance.state.Value == State.GamePlaying)
            {
                timerText.text = Mathf.CeilToInt(gamePlayingTimer).ToString();
            }
            vpPlayer.text = ServerBattleRoomState.Instance.GetPlayerVictoryPoint().ToString();
            vpEnemy.text = ServerBattleRoomState.Instance.GetEnemyVictoryPoint().ToString();
        }

        private void HandleTimeout()
        {            
            // ChangeState(new TimeOutState(this));
            // currentState.Execute();
            Debug.Log("Handle Timeout");
            timeoutTriggered = true;
            StartCoroutine(MatchTimeout());
        }

        private void HandleTimeoutTimer()
        {
            float normalTime = ServerBattleRoomState.Instance.GetCountdownToStartTimer(); // 120f
            float limitNormalTime = ServerBattleRoomState.Instance.GetLimitCountdownToStartTimer(); //  0.5f
            timeoutToTimer.text = Mathf.CeilToInt(normalTime).ToString();
            Debug.Log("Normal Time: " + normalTime);
            TimeoutNotif.SetActive(true);
            if (normalTime <= limitNormalTime )
            {
                Debug.Log("Timeout Timer");
                TimeoutNotif.SetActive(false);
                if(!isSoundFight){
                    Debug.Log("Sound Fight");
                    FightNotif.SetActive(true);
                    NewSoundManager.Instance.PlaySound2D("Fight");
                    isSoundFight = true;
                }
                Debug.Log("State Handle Timeout Timer" + ServerBattleRoomState.Instance.state.Value);
            }
            else{
                isSoundFight = false;
            }
        }
        
        private IEnumerator WaitForPlayer()
        {

            yield return new WaitUntil(() => FindObjectOfType<PlayerMovement_2>() != null);
            playerMovement = FindObjectOfType<PlayerMovement_2>();
            playerAttack = FindFirstObjectByType<PlayerIFAttack>();
            playerMovement.canMove = false; // Atur setelah player ditemukan
            playerAttack.canAttack = false;
            if (playerMovement == null)
            {
                Debug.Log("playerMovement tidak ditemukan, menggunakan serverCharacterMovement sebagai gantinya.");
                serverCharacterMovement = FindFirstObjectByType<ServerCharacterMovement>();
                if (serverCharacterMovement != null)
                {
                    serverCharacterMovement.canMove = false;
                }
                else
                {
                    Debug.LogError("serverCharacterMovement juga tidak ditemukan.");
                }
            }
            else
            {
                playerMovement.canMove = false; // Atur setelah player ditemukan
            }
        }
        private void ServerBattleRoomState_OnStateChanged(object sender, System.EventArgs e)
        {
            // Debug.Log("Checking IsCountdownToStartActive");
        
            if (ServerBattleRoomState.Instance.IsCountdownToStartActive())
            {
                Debug.Log("CountdownToStart is active");
        
                if (!isCountdownCoroutineStarted)
                {
                    Debug.Log($"Couroutine started for Round {currentRound}");
                    StartCoroutine(ShowRoundStartNotification(currentRound));
                    Debug.Log($"CountdownToStart coroutine started for Round {currentRound}");
                    isCountdownCoroutineStarted = true;
                }
            }
            else
            {
                Debug.Log("CountdownToStart is NOT active");
                isCountdownCoroutineStarted = false;
            }
        }

       public IEnumerator ShowRoundStartNotification(int roundNumber)
        {
            GameObject currentRoundNotif = null;

            switch (roundNumber)
            {
                case 1: currentRoundNotif = Round1Notif; NewSoundManager.Instance.PlaySound2D("Round1") ; break;
                case 2: currentRoundNotif = Round2Notif; NewSoundManager.Instance.PlaySound2D("Round2") ; break;
                case 3: currentRoundNotif = Round3Notif; NewSoundManager.Instance.PlaySound2D("Round3") ; break;
                case 4: currentRoundNotif = Round4Notif; NewSoundManager.Instance.PlaySound2D("Round4") ; break;
                case 5: currentRoundNotif = FinalRoundNotif; NewSoundManager.Instance.PlaySound2D("Final_Round"); break;
                case 6: currentRoundNotif = DrawRoundNotif; break;
            }

            if (currentRoundNotif != null)
            {
                Debug.Log("Before Round Start Notification");
                currentRoundNotif.SetActive(true);
                Debug.Log("Round Start Notification");
                yield return new WaitForSeconds(2f);
                Debug.Log("After WaitForsecond");
                currentRoundNotif.SetActive(false);
                // if (currentRound > 1 && currentRound < 6)
                // {
                //     timeoutTimer = true;
                // }
                while (ServerBattleRoomState.Instance.IsCountdownToStartActive())
                {
                    HandleTimeoutTimer();
                    yield return null;
                }

                yield return new WaitForSeconds(0.5f);
                FightNotif.SetActive(false);
                Debug.Log("After FightNotif");

                if (playerMovement != null)
                {
                    playerMovement.canMove = true;
                    playerAttack.canAttack = true;
                }
                else if (serverCharacterMovement != null)
                {
                    serverCharacterMovement.canMove = true;
                }
                

            }
        }
        private void ResetPlayerAndEnemyPositions()
        {
            if (initialPlayerPosition != null && playerMovement != null)
            {
                playerMovement.transform.position = initialPlayerPosition.transform.position; // Reset posisi pemain
            }
            else
            {
                Debug.LogWarning("Initial Player Position atau Player Movement tidak diatur!");
            }

            if (initialEnemyPosition != null && enemyState != null)
            {
                enemyState.transform.position = initialEnemyPosition.transform.position; // Reset posisi musuh
            }
            else
            {
                Debug.LogWarning("Initial Enemy Position atau Enemy State tidak diatur!");
            }
        }


        public void PlayerDied()
        {
            // Logika untuk ketika player mati
            if (ServerBattleRoomState.Instance.GetPlayerVictoryPoint() == 2 && ServerBattleRoomState.Instance.GetEnemyVictoryPoint() == 2 && finalRound)
            {
                // Kondisi final round
                EnemyVictory();
            }
            else
            {
                EnemyVictory();
            }
        }

        public void EnemyDied()
        {
            // Logika untuk ketika enemy mati
            if (ServerBattleRoomState.Instance.GetPlayerVictoryPoint() == 2 && ServerBattleRoomState.Instance.GetEnemyVictoryPoint() == 2 && finalRound)
            {
                // Kondisi final round
                PlayerVictory();
            }
            else
            {
                PlayerVictory();
            }
        }
        
        public void ShowVictoryNotif(bool isPlayerVictory)
        {
            if (isPlayerVictory)
            {
                VictoryNotif.SetActive(true);
                TimeoutNotif.SetActive(false);
                timeoutToTimer.text = "";
            }
            else
            {
                DefeatNotif.SetActive(true);
            }
        }

        public IEnumerator ShowEndGame()
        {
            // matchTimer.ChangeMatchStatus(false);  // Stop the timer when match ends
            if (Reward != null)
            {
                Reward.SetActive(true);
                yield return new WaitForSeconds(1.5f);
                Loader.Load(Loader.Scene.Lingkungan);
            }
        }

        private IEnumerator MatchTimeout() 
        {
            Debug.Log("Match Timeout");
            NewSoundManager.Instance.PlaySound2D("Time_Out");
            Debug.Log("2 Match Timeout");
            TimeoutNotif.SetActive(true);
            Debug.Log("3 Match Timeout");
            timeoutToTimer.text = "TIME OUT";
            Debug.Log("State Macth Timeout" + ServerBattleRoomState.Instance.state.Value);
            ServerBattleRoomState.Instance.ChangeState(State.WaitingTime);
            Debug.Log("State Macth Timeout 2" + ServerBattleRoomState.Instance.state.Value);
            Debug.Log("4 Match Timeout");
            
            yield return new WaitForSeconds(3f);
            Debug.Log("5 Match Timeout");

            TimeoutNotif.SetActive(false);
            Debug.Log("6 Match Timeout");

            // Check health via PlayerState and EnemyState
            if (playerState != null && enemyState != null)
            {
                if (ServerBattleRoomState.Instance.GetPlayerVictoryPoint() == 2 && ServerBattleRoomState.Instance.GetEnemyVictoryPoint() == 2 && finalRound == true) 
                {
                    if (playerState.GetCurrentHealth() > enemyState.GetCurrentHealth()) 
                    {
                        PlayerVictory();  // Player menang di final round
                        timeoutToTimer.text = "PLAYER WON FINAL ROUND";
                        Debug.Log("Player won the final round by health.");
                    } 
                    else 
                    {
                        EnemyVictory();  // Enemy menang di final round
                        timeoutToTimer.text = "ENEMY WON FINAL ROUND";
                        Debug.Log("Enemy won the final round by health.");
                    }
                }
                else 
                {
                    // Jika bukan final round, cek pemenang biasa berdasarkan health
                    if (playerState.GetCurrentHealth() == enemyState.GetCurrentHealth()) 
                    {
                        DrawRound();  // Tambahkan poin draw untuk kedua pihak
                    }
                    else if (playerState.GetCurrentHealth() > enemyState.GetCurrentHealth()) 
                    {
                        PlayerVictory();  // Player menang jika health lebih besar
                    }
                    else 
                    {
                        EnemyVictory();  // Enemy menang jika health lebih besar
                    }
                }
            }
        }

        public void PlayerVictory()
        {
            if (ServerBattleRoomState.Instance.IsServer)
            {
                ServerBattleRoomState.Instance.IncrementPlayerVictoryPointServerRpc();
            }
            NewSoundManager.Instance.PlaySound2D("Player_Won");
            StartCoroutine(HandleRoundTransition());
        }

        public void EnemyVictory()
        {
            if (ServerBattleRoomState.Instance.IsServer)
            {
                ServerBattleRoomState.Instance.IncrementEnemyVictoryPointServerRpc();
            }
            NewSoundManager.Instance.PlaySound2D("Enemy_Won");
            StartCoroutine(HandleRoundTransition());
        }

        public void DrawRound()
        {
            Debug.Log("Draw Round Is Called");
            if (ServerBattleRoomState.Instance.IsServer)
            {
                ServerBattleRoomState.Instance.IncrementDrawVictoryPointServerRpc();
                Debug.Log("Draw Round Is Server");

            }
            NewSoundManager.Instance.PlaySound2D("Draw");
            StartCoroutine(HandleDrawTransition());
        }


        private IEnumerator HandleDrawTransition()
        {
            // Cek apakah ada draw
            if (playerState.GetCurrentHealth() == enemyState.GetCurrentHealth()) 
            {   
                yield return StartCoroutine(ShowRoundStartNotification(6)); 
            }

            // Cek apakah kedua pemain mendapatkan poin kemenangan
            if (ServerBattleRoomState.Instance.GetPlayerVictoryPoint() == 1 && ServerBattleRoomState.Instance.GetEnemyVictoryPoint() == 1)
            {
                Debug.Log("Moving to next round: 2");
                currentRound = ServerBattleRoomState.Instance.GetPlayerVictoryPoint() + ServerBattleRoomState.Instance.GetEnemyVictoryPoint();
                Debug.Log("Current Round in Handle draw: " + currentRound);
                // yield return StartCoroutine(ShowRoundStartNotification(playerVictoryPoint + enemyVictoryPoint)); // Tampilkan Notifikasi Ronde 2
            }
            // Jika hanya musuh yang mencapai 4 poin, lanjutkan ke ronde berikutnya
            else if (ServerBattleRoomState.Instance.GetPlayerVictoryPoint() == 2 && ServerBattleRoomState.Instance.GetEnemyVictoryPoint() == 2)
            {
                Debug.Log("Moving to next round: 3");
                currentRound = ServerBattleRoomState.Instance.GetPlayerVictoryPoint() + ServerBattleRoomState.Instance.GetEnemyVictoryPoint() - 1;
                Debug.Log("Current Round in Handle draw: " + currentRound);
                // yield return StartCoroutine(ShowRoundStartNotification(playerVictoryPoint + enemyVictoryPoint - 1)); // Tampilkan Notifikasi Ronde 2
            }
            Debug.Log("Moving to next round: 2 iffiifififis");
            StartNormalTimer(); // Mulai timer untuk ronde baru
            Debug.Log($"Moving to next round: {currentRound}");
        }

        private IEnumerator HandleRoundTransition()
        {
            currentRound++;
            if (serverCharacterMovement != null)
            {
                serverCharacterMovement.canMove = true;
            }
            TimeoutNotif.SetActive(true);
            // Kondisi untuk menentukan siapa yang memenangkan ronde terakhir
            if (playerState.currentHealth.Value > enemyState.currentHealth.Value)
            {
                // playerMovement.canMove = false;
                // playerAttack.canAttack = false;
                timeoutToTimer.text = "PLAYER WON";
                ResetPlayerAndEnemyPositions();
                
            }
            else if (playerState.currentHealth.Value < enemyState.currentHealth.Value)
            {
                // playerMovement.canMove = false;
                // playerAttack.canAttack = false;
                timeoutToTimer.text = "ENEMY WON";
                ResetPlayerAndEnemyPositions();
                
            }

            yield return new WaitForSeconds(3f);

            TimeoutNotif.SetActive(false);

            // Cek apakah salah satu sudah mencapai 3 poin (kondisi kemenangan)
            if (ServerBattleRoomState.Instance.GetPlayerVictoryPoint() == 3) 
            {
                ShowVictoryNotif(true);
                NewSoundManager.Instance.PlaySound2D("Victory");
                yield return new WaitForSeconds(2f);
                VictoryNotif.SetActive(false);
                yield return StartCoroutine(ShowEndGame());
            }
            
            else if (ServerBattleRoomState.Instance.GetEnemyVictoryPoint() == 3) 
            {
                ShowVictoryNotif(false);
                NewSoundManager.Instance.PlaySound2D("Defeat");
                yield return new WaitForSeconds(1f);
                DefeatNotif.SetActive(false);
                yield return StartCoroutine(ShowEndGame());
            }
            else if (ServerBattleRoomState.Instance.GetPlayerVictoryPoint() == 2 && ServerBattleRoomState.Instance.GetEnemyVictoryPoint() == 2 && !finalRound)
            {
                finalRound = true;
                isSoundFight = false;
                yield return StartCoroutine(ShowRoundStartNotification(5)); // Final round
            }

            else
            {     
                NextRound();
            }
            // if (playerMovement != null)
            // {
            //     playerMovement.canMove = true;
            //     playerAttack.canAttack = true;
            // }
            if (serverCharacterMovement != null)
            {
                serverCharacterMovement.canMove = true;
            }
            
            StartNormalTimer();
        }

        public void NextRound()
        {
            isSoundFight = false;

            int nextRound = ServerBattleRoomState.Instance.GetPlayerVictoryPoint() + ServerBattleRoomState.Instance.GetEnemyVictoryPoint() + 1;
            
            currentRound = nextRound;
            // yield return StartCoroutine(ShowRoundStartNotification(nextRound));
        }

        void StartNormalTimer()
        {
            Debug.Log("Current Round in Start Normal Timer: " + currentRound);
            Debug.Log("State Start Normal Timer" + ServerBattleRoomState.Instance.state.Value);
            Debug.Log("Starting normal timer 2");


            ServerBattleRoomState.Instance.ChangeStateServerRpc(State.CountdownToStart);
            // ServerBattleRoomState.Instance.ResetCountdownToStartTimerServerRpc();
            StartCoroutine(WaitAndResetTimeout());
        }

        private IEnumerator WaitAndResetTimeout()
        {
            yield return new WaitForSeconds(0.5f);
            
            // TimeoutNotif.SetActive(false);
            Debug.Log("Resetting timer");
            timeoutTimer = false;

            Debug.Log("Resetting timer 2");
            // ServerBattleRoomState.Instance.ResetGamePlayingTimerNormalizedServerRpc();
            timeoutTriggered = false;
            ServerBattleRoomState.Instance.RoundOutcomeDeterminedServerRpc();

            playerState.ResetHealthServerRpc();
            enemyState.ResetHealthServerRpc();
        }
    }
}