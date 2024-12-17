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
        //Audio Source Sound Manager
        private bool isSoundFight = false;


        private PlayerMovement_2 playerMovement;

        // Reference to PlayerState and EnemyState
        public PlayerState playerState;
        public EnemyState enemyState;
        public SoundManager soundManager;

  
        private IState currentState;
        

        void Start() 
        {
            ServerBattleRoomState.Instance.OnStateChanged += ServerBattleRoomState_OnStateChanged;
            Debug.Log("MatchManager Start");
            // StartCoroutine(ShowRoundStartNotification(1));
            // playerMovement.canMove = false;
        }

        public void ChangeState(IState newState)
        {
            Debug.Log("Changing state to: " + newState.GetType().Name);
            currentState.Exit();
            currentState = newState; // Ubah state ke state baru
            currentState.Enter(); // Panggil Enter pada state baru
            Debug.Log("State changed. Current timeoutToTimer.text: " + timeoutToTimer.text);
        }

        [System.Obsolete]
        private void Awake()
        {
            Instance = this;

            // Ensure PlayerState and EnemyState are correctly set up
            playerState = FindObjectOfType<PlayerState>();
            enemyState = FindObjectOfType<EnemyState>();
            playerMovement = FindObjectOfType<PlayerMovement_2>();
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
                playerMovement.canMove = false;
                matchTimer.ChangeMatchStatus(true);

                if (roundNumber == 1)
                {
                    FightNotif.SetActive(true);
                    NewSoundManager.Instance.PlaySound2D("Fight");
                    yield return new WaitForSeconds(1.5f);
                    FightNotif.SetActive(false);
                    playerMovement.canMove = true;
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

        public IEnumerator ShowEndGameButton()
        {
            matchTimer.ChangeMatchStatus(false);  // Stop the timer when match ends
            if (Reward != null)
            {
                Reward.SetActive(true);
                yield return new WaitForSeconds(1.5f);
                SceneManager.LoadScene("Asrama");
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
                    if (playerState.currentHealth > enemyState.currentHealth) 
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
                    if (playerState.currentHealth == enemyState.currentHealth) 
                    {
                        DrawRound();  // Tambahkan poin draw untuk kedua pihak
                    }
                    else if (playerState.currentHealth > enemyState.currentHealth) 
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
            playerMovement.canMove = false;
            TimeoutNotif.SetActive(true);
            // Kondisi untuk menentukan siapa yang memenangkan ronde terakhir
            if (playerState.currentHealth > enemyState.currentHealth)
            {
                timeoutToTimer.text = "PLAYER WON";
            }
            else if (playerState.currentHealth < enemyState.currentHealth)
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
            yield return new WaitForSeconds(5f);
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
