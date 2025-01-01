using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using NUnit.Framework.Internal;

namespace ITKombat
{
    public class MatchManager : MonoBehaviour
    {
        public static MatchManager Instance;
        public GameObject ReadyNotif, Round1Notif, Round2Notif, Round3Notif, Round4Notif, FinalRoundNotif, DrawRoundNotif, FightNotif, DefeatNotif, VictoryNotif, TimeoutNotif;
        public GameObject Reward;
        public int playerVictoryPoint;
        public int enemyVictoryPoint;
        public MatchTimer matchTimer;
        public TMP_Text timerText, timeoutToTimer, vpPlayer, vpEnemy;
        public bool timeoutTriggered = false;
        public bool timeoutTimer = false;
        public bool finalRound = false;
        private bool isSoundFight = false;
        private PlayerMovement_2 playerMovement;
        private ServerCharacterMovement serverCharacterMovement;
        public PlayerState playerState;
        public EnemyState enemyState;
        public SoundManager soundManager;

        private int currentRound = 1;
        private bool isCountdownCoroutineStarted = false;
        
        void Start() 
        {
            ServerBattleRoomState.Instance.OnStateChanged += ServerBattleRoomState_OnStateChanged;
            // StartCoroutine(ShowRoundStartNotification(1));
            StartCoroutine(WaitForPlayer());
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
            vpPlayer.text = playerVictoryPoint.ToString();
            vpEnemy.text = enemyVictoryPoint.ToString();
         
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
            float normalTime = ServerBattleRoomState.Instance.GetCountdownToStartTimer();
            timeoutToTimer.text = Mathf.CeilToInt(normalTime).ToString();
            Debug.Log("Normal Time: " + normalTime);
            // matchTimer.ChangeMatchStatus(false);
            // timeoutToTimer.text = matchTimer.GetNormalTimeInSecond().ToString();
            TimeoutNotif.SetActive(true);
            // if (matchTimer.GetNormalTimeInSecond() <= 0f) 
            if (normalTime <= 0.1f)
            {
                Debug.Log("Timeout Timer");
                TimeoutNotif.SetActive(false);
                // matchTimer.ChangeMatchStatus(true);
                if(!isSoundFight){
                    Debug.Log("Sound Fight");
                    FightNotif.SetActive(true);
                    NewSoundManager.Instance.PlaySound2D("Fight");
                    isSoundFight = true;
                }
                Debug.Log("State Handle Timeout Timer" + ServerBattleRoomState.Instance.state.Value);
                ServerBattleRoomState.Instance.ChangeState(State.GamePlaying);
            }
            else{
                isSoundFight = false;
            }
        }
        
        private IEnumerator WaitForPlayer()
        {
            yield return new WaitUntil(() => FindFirstObjectByType<PlayerMovement_2>() != null);
            playerMovement = FindFirstObjectByType<PlayerMovement_2>();
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
                // Debug.Log("CountdownToStart is active");
        
                if (!isCountdownCoroutineStarted)
                {
                    // Debug.Log($"Couroutine started for Round {currentRound}");
                    StartCoroutine(ShowRoundStartNotification(currentRound));
                    // Debug.Log($"CountdownToStart coroutine started for Round {currentRound}");
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

                if (playerMovement != null)
                {
                    playerMovement.canMove = true;
                }
                else if (serverCharacterMovement != null)
                {
                    serverCharacterMovement.canMove = true;
                }
                

            }
        }

        public void PlayerDied()
        {
            // Logika untuk ketika player mati
            if (playerVictoryPoint == 2 && enemyVictoryPoint == 2 && finalRound)
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
            if (playerVictoryPoint == 2 && enemyVictoryPoint == 2 && finalRound)
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
                if (playerVictoryPoint == 2 && enemyVictoryPoint == 2 && finalRound == true) 
                {
                    if (playerState.currentHealth.Value > enemyState.currentHealth.Value) 
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
                    if (playerState.currentHealth.Value == enemyState.currentHealth.Value) 
                    {
                        DrawRound();  // Tambahkan poin draw untuk kedua pihak
                    }
                    else if (playerState.currentHealth.Value > enemyState.currentHealth.Value) 
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


        public void DrawRound()
        {
            playerVictoryPoint += 1;
            enemyVictoryPoint += 1;
            NewSoundManager.Instance.PlaySound2D("Draw");
            Debug.Log("Draw round");
            StartCoroutine(HandleDrawTransition());
        }
        public void PlayerVictory() 
        {
            playerVictoryPoint += 1;
            NewSoundManager.Instance.PlaySound2D("Player_Won");
            StartCoroutine(HandleRoundTransition());
        }

        public void EnemyVictory() 
        {
            enemyVictoryPoint += 1;
            NewSoundManager.Instance.PlaySound2D("Enemy_Won");
            StartCoroutine(HandleRoundTransition());
        }

        private IEnumerator HandleDrawTransition()
        {
            // Cek apakah ada draw
            if (playerState.currentHealth.Value == enemyState.currentHealth.Value) 
            {   
                yield return StartCoroutine(ShowRoundStartNotification(6)); // Tampilkan Notifikasi Draw
            }

            // Cek apakah kedua pemain mendapatkan poin kemenangan
            if (playerVictoryPoint == 1 && enemyVictoryPoint == 1)
            {
                Debug.Log("Moving to next round: 2");
                currentRound = playerVictoryPoint + enemyVictoryPoint;
                Debug.Log("Current Round in Handle draw: " + currentRound);
                // yield return StartCoroutine(ShowRoundStartNotification(playerVictoryPoint + enemyVictoryPoint)); // Tampilkan Notifikasi Ronde 2
            }
            // Jika hanya musuh yang mencapai 4 poin, lanjutkan ke ronde berikutnya
            else if (playerVictoryPoint == 2 && enemyVictoryPoint == 2)
            {
                Debug.Log("Moving to next round: 3");
                currentRound = playerVictoryPoint + enemyVictoryPoint - 1;
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
            Debug.Log($"Moving to next round: {currentRound}");
            if (playerMovement != null)
            {
                playerMovement.canMove = true;
            }
            else if (serverCharacterMovement != null)
            {
                serverCharacterMovement.canMove = true;
            }
            TimeoutNotif.SetActive(true);
            // Kondisi untuk menentukan siapa yang memenangkan ronde terakhir
            if (playerState.currentHealth.Value > enemyState.currentHealth.Value)
            {
                timeoutToTimer.text = "PLAYER WON";
            }
            else if (playerState.currentHealth.Value < enemyState.currentHealth.Value)
            {
                timeoutToTimer.text = "ENEMY WON";
            }

            yield return new WaitForSeconds(3f);

            TimeoutNotif.SetActive(false);

            // Cek apakah salah satu sudah mencapai 3 poin (kondisi kemenangan)
            if (playerVictoryPoint == 3) 
            {
                ShowVictoryNotif(true);
                NewSoundManager.Instance.PlaySound2D("Victory");
                yield return new WaitForSeconds(2f);
                VictoryNotif.SetActive(false);
                yield return StartCoroutine(ShowEndGame());
            }
            
            else if (enemyVictoryPoint == 3) 
            {
                ShowVictoryNotif(false);
                NewSoundManager.Instance.PlaySound2D("Defeat");
                yield return new WaitForSeconds(1f);
                DefeatNotif.SetActive(false);
                yield return StartCoroutine(ShowEndGame());
            }
            else if (playerVictoryPoint == 2 && enemyVictoryPoint == 2 && !finalRound)
            {
                finalRound = true;
                isSoundFight = false;
                yield return StartCoroutine(ShowRoundStartNotification(5)); // Final round
            }

            else
            {     
                NextRound();
            }
            if (playerMovement != null)
            {
                playerMovement.canMove = true;
            }
            else if (serverCharacterMovement != null)
            {
                serverCharacterMovement.canMove = true;
            }
            StartNormalTimer();
        }

        public void NextRound()
        {
            isSoundFight = false;

            int nextRound = playerVictoryPoint + enemyVictoryPoint + 1;
            
            currentRound = nextRound;
            // yield return StartCoroutine(ShowRoundStartNotification(nextRound));
        }

        void StartNormalTimer()
        {
            Debug.Log("Current Round in Start Normal Timer: " + currentRound);
            Debug.Log("Starting normal timer");
            // TimeoutNotif.SetActive(true);
            Debug.Log("State Start Normal Timer" + ServerBattleRoomState.Instance.state.Value);
            Debug.Log("State Start Normal Timer 2" + ServerBattleRoomState.Instance.state.Value);
            Debug.Log("Starting normal timer 2");
            // matchTimer.GetResetNormalTimerStart();
            // matchTimer.GetNormalTimeInSecond();
    
            // Debug.Log("Starting normal timer 3 " + matchTimer.GetNormalTimeInSecond());

            ServerBattleRoomState.Instance.ChangeState(State.CountdownToStart);
            ServerBattleRoomState.Instance.GetGamePlayingTimerNormalized();
            ServerBattleRoomState.Instance.GetResetCountdownToStartTimer();
            Debug.Log("State Start Normal Timer 3" + ServerBattleRoomState.Instance.state.Value);  
            WaitAndResetTimeout();
            Debug.Log("Starting normal timer 4");
        }

        private void WaitAndResetTimeout()
        {
            // yield return new WaitForSeconds(1f);
            
            TimeoutNotif.SetActive(false);
            Debug.Log("Resetting timer");
            timeoutTimer = false;
            // matchTimer.ChangeMatchStatus(true);

            Debug.Log("Resetting timer 2");
            // Reset timer
            // matchTimer.GetResetTimerStart();
            // Debug.Log("Resetting timer 3 " + matchTimer.GetResetTimerStart());
            ServerBattleRoomState.Instance.GetResetGamePlayingTimerNormalized();
            // Debug.Log("Resetting timer 3" + ServerBattleRoomState.Instance.GetGamePlayingTimerNormalized());
            timeoutTriggered = false;
            ServerBattleRoomState.Instance.IsRoundOutcomeDetermined();

            // Reset health
            playerState.ResetHealth();
            enemyState.ResetHealth();
        }
    }
}