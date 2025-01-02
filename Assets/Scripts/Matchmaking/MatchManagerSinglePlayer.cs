using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using NUnit.Framework.Internal;

namespace ITKombat
{
    public class MatchManagerSinglePlayer : MonoBehaviour
    {
        public static MatchManagerSinglePlayer Instance;
        public GameObject ReadyNotif, Round1Notif, Round2Notif, Round3Notif, Round4Notif, FinalRoundNotif, DrawRoundNotif, FightNotif, DefeatNotif, VictoryNotif, TimeoutNotif;
        public GameObject Reward;
        public int playerVictoryPoint;
        public int enemyVictoryPoint;
        public MatchTimer matchTimer;
        public TMP_Text timerText, timeoutToTimer, vpPlayer, vpEnemy;
        public bool timeoutTriggered = false;
        public bool timeoutTimer = false;
        public bool finalRound = false;
        //Audio Source Sound Manager
        private bool isSoundFight = false;


        private PlayerMovement_2 playerMovement;

        private PlayerIFAttack playerAttack;


        public GameObject initialPlayerPosition;
        public GameObject initialEnemyPosition;

        // Reference to PlayerState and EnemyState
        public PlayerState playerState;
        public EnemyState enemyState;
        public SoundManager soundManager;

  
        private IState currentState;
        
        [System.Obsolete]
        void Start() 
        {
            // ServerBattleRoomState.Instance.OnStateChanged += ServerBattleRoomState_OnStateChanged;
            Debug.Log("MatchManager Start");
            StartCoroutine(ShowRoundStartNotification(1));
            StartCoroutine(WaitForPlayer());
        }

        


        public void ChangeState(IState newState)
        {
            Debug.Log("Changing state to: " + newState.GetType().Name);
            currentState.Exit();
            currentState = newState; // Ubah state ke state baru
            currentState.Enter(); // Panggil Enter pada state baru
            Debug.Log("State changed. Current timeoutToTimer.text: " + timeoutToTimer.text);
        }

        

        private IEnumerator WaitForPlayer()
        {
            yield return new WaitUntil(() => FindObjectOfType<PlayerMovement_2>() != null);
            playerMovement = FindObjectOfType<PlayerMovement_2>();
            playerAttack = FindFirstObjectByType<PlayerIFAttack>();
            playerMovement.canMove = false; // Atur setelah player ditemukan
            playerAttack.canAttack = false;
        }
        private void Awake()
        {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            } else {
                Instance = this;
            }

            // Ensure PlayerState and EnemyState are correctly set up
            playerState = FindObjectOfType<PlayerState>();
            enemyState = FindObjectOfType<EnemyState>();
        }
        private void ServerBattleRoomState_OnStateChanged(object sender, System.EventArgs e)
        {
            Debug.Log("Checking IsCountdownToStartActive");
            if (ServerBattleRoomState.Instance.IsCountdownToStartActive())
            {
                Debug.Log("CountdownToStart is active");
                StartCoroutine(ShowRoundStartNotification(1));
            }
            else
            {
                Debug.Log("CountdownToStart is NOT active");
            }
        }

    // private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e) {
    //     if (KitchenGameManager.Instance.IsCountdownToStartActive()) {
    //         Show();
    //     } else {
    //         Hide();
    //     }
    // }
        void Update()
        {
            timerText.text = matchTimer.GetStageTimeInSecond().ToString();

            vpPlayer.text = playerVictoryPoint.ToString();
            vpEnemy.text = enemyVictoryPoint.ToString();
            
            if (matchTimer.GetStageTimeInSecond() == 0 && !timeoutTriggered) 
            {
                HandleTimeout();
            } 
            else if (timeoutTimer == true) 
            {   
                HandleTimeoutTimer();
            }
            
        }

        private void HandleTimeout()
        {            
            Debug.Log("Timeout triggered");
            // ChangeState(new TimeOutState(this));
            // currentState.Execute();
            StartCoroutine(MatchTimeout());
            timeoutTriggered = true;
        }

        private void HandleTimeoutTimer()
        {
            matchTimer.ChangeMatchStatus(false);
            timeoutToTimer.text = matchTimer.GetNormalTimeInSecond().ToString();

            Debug.Log(timeoutToTimer.text);
            if (matchTimer.GetNormalTimeInSecond() <= 0f) 
            {
                matchTimer.ChangeMatchStatus(true);
                if(!isSoundFight){
                    NewSoundManager.Instance.PlaySound2D("Fight");
                    isSoundFight = true;
                }
                timeoutToTimer.text = "FIGHT";
            }
            else{
                isSoundFight = false;
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
                currentRoundNotif.SetActive(true);
                yield return new WaitForSeconds(2f);
                currentRoundNotif.SetActive(false);
                
                matchTimer.ChangeMatchStatus(true);

                // if (roundNumber == 1)
                // {
                //     FightNotif.SetActive(true);
                //     NewSoundManager.Instance.PlaySound2D("Fight");
                //     yield return new WaitForSeconds(1.5f);
                //     FightNotif.SetActive(false);
                //     playerMovement.canMove = true;
                //     playerAttack.canAttack = true;
                // }
            }
            playerMovement.canMove = false;
            playerAttack.canAttack = false;
            
            StartNormalTimer(); // Mulai timer untuk ronde baru

            yield return new WaitForSeconds(1.5f);

            playerMovement.canMove = true;
            playerAttack.canAttack = true;

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

        public IEnumerator ShowEndGameButton()
        {
            matchTimer.ChangeMatchStatus(false);  // Stop the timer when match ends
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
            TimeoutNotif.SetActive(true);
            timeoutToTimer.text = "TIME OUT";
            yield return new WaitForSeconds(3f);

            TimeoutNotif.SetActive(false);

            // Check health via PlayerState and EnemyState
            if (playerState != null && enemyState != null)
            {
                if (playerVictoryPoint == 2 && enemyVictoryPoint == 2 && finalRound == true) 
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


        public void DrawRound()
        {
            playerVictoryPoint += 1;
            enemyVictoryPoint += 1;
            NewSoundManager.Instance.PlaySound2D("Draw");
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
            if (playerState.currentHealth == enemyState.currentHealth) 
            {   
                yield return StartCoroutine(ShowRoundStartNotification(6)); // Tampilkan Notifikasi Draw
            }

            // Cek apakah kedua pemain mendapatkan poin kemenangan
            if (playerVictoryPoint == 1 && enemyVictoryPoint == 1)
            {
                yield return StartCoroutine(ShowRoundStartNotification(playerVictoryPoint + enemyVictoryPoint)); // Tampilkan Notifikasi Ronde 2
            }
            // Jika hanya musuh yang mencapai 4 poin, lanjutkan ke ronde berikutnya
            else if (playerVictoryPoint == 2 && enemyVictoryPoint == 2)
            {
                yield return StartCoroutine(ShowRoundStartNotification(playerVictoryPoint + enemyVictoryPoint - 1)); // Tampilkan Notifikasi Ronde 2
            }

            StartNormalTimer(); // Mulai timer untuk ronde baru
        }



        private IEnumerator HandleRoundTransition()
        {
            
            TimeoutNotif.SetActive(true);
            // Kondisi untuk menentukan siapa yang memenangkan ronde terakhir
            if (playerState.currentHealth.Value > enemyState.currentHealth.Value)
            {
                playerMovement.canMove = false;
                playerAttack.canAttack = false;
                timeoutToTimer.text = "PLAYER WON";
                ResetPlayerAndEnemyPositions();
                
            }
            else if (playerState.currentHealth.Value < enemyState.currentHealth.Value)
            {
                playerMovement.canMove = false;
                playerAttack.canAttack = false;
                timeoutToTimer.text = "ENEMY WON";
                ResetPlayerAndEnemyPositions();
                
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
                yield return StartCoroutine(ShowEndGameButton());
            }
            
            else if (enemyVictoryPoint == 3) 
            {
                ShowVictoryNotif(false);
                NewSoundManager.Instance.PlaySound2D("Defeat");
                yield return new WaitForSeconds(1f);
                DefeatNotif.SetActive(false);
                yield return StartCoroutine(ShowEndGameButton());
            }
            else if (playerVictoryPoint == 2 && enemyVictoryPoint == 2 && !finalRound)
            {
                finalRound = true;
                isSoundFight = false;
                yield return StartCoroutine(ShowRoundStartNotification(5)); // Final round
            }

            else
            {
                yield return StartCoroutine(NextRound());
            }
            playerMovement.canMove = true;
            playerAttack.canAttack = true;
            StartNormalTimer();
        }

        public IEnumerator NextRound()
        {
            isSoundFight = false;

            int nextRound = playerVictoryPoint + enemyVictoryPoint + 1;
                
            yield return StartCoroutine(ShowRoundStartNotification(nextRound));
        }

        void StartNormalTimer()
        {
            TimeoutNotif.SetActive(true);
            matchTimer.GetNormalTimeInSecond();
            timeoutTimer = true;
            
            StartCoroutine(WaitAndResetTimeout());
        }

         IEnumerator WaitAndResetTimeout()
        {
            yield return new WaitForSeconds(2f);
            TimeoutNotif.SetActive(false);
            timeoutTimer = false;
            matchTimer.ChangeMatchStatus(true);

            // Reset timer
            matchTimer.GetResetTimerStart();
            timeoutTriggered = false;

            // Reset health
            playerState.ResetHealth();
            enemyState.ResetHealth();
        }

    }
}