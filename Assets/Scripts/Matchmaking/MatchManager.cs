using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor.SearchService;

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
        private bool finalRound = false;

        private PlayerMovement_2 playerMovement;

        // Reference to PlayerState and EnemyState
        private PlayerState playerState;
        private EnemyState enemyState;

        //Audio Source Sound Manager
        public AudioSource soundRound1;
        public AudioSource soundRound2;
        public AudioSource soundRound3;
        public AudioSource soundRound4;
        public AudioSource soundFinalRound;
        public AudioSource soundFight;
        public AudioSource soundDefead;
        public AudioSource soundVictory;
        public AudioSource soundTimeOut;


        void Start() 
        {
            StartCoroutine(ShowRoundStartNotification(1));
            playerMovement.canMove = false;
        }

        private void Awake()
        {
            Instance = this;

            // Ensure PlayerState and EnemyState are correctly set up
            playerState = FindObjectOfType<PlayerState>();
            enemyState = FindObjectOfType<EnemyState>();
            playerMovement = FindObjectOfType<PlayerMovement_2>();
        }

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
            StartCoroutine(MatchTimeout());
            timeoutTriggered = true;
        }

        private void HandleTimeoutTimer()
        {
            matchTimer.ChangeMatchStatus(false);
            timeoutToTimer.text = matchTimer.GetNormalTimeInSecond().ToString();

            if (matchTimer.normalTimerStart <= 1f) 
            {
                matchTimer.ChangeMatchStatus(true);
                soundFight.Play();
                timeoutToTimer.text = "FIGHT";
                
            }
        }

       public IEnumerator ShowRoundStartNotification(int roundNumber)
        {
            GameObject currentRoundNotif = null;

            switch (roundNumber)
            {
                case 1: currentRoundNotif = Round1Notif; soundRound1.Play(); break;
                case 2: currentRoundNotif = Round2Notif; soundRound2.Play(); break;
                case 3: currentRoundNotif = Round3Notif; soundRound3.Play(); break;
                case 4: currentRoundNotif = Round4Notif; soundRound4.Play(); break;
                case 5: currentRoundNotif = FinalRoundNotif; soundFinalRound.Play(); break;
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
                    soundFight.Play();
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
            soundTimeOut.Play();
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
            StartCoroutine(HandleDrawTransition());
        }
        public void PlayerVictory() 
        {
            playerVictoryPoint += 1;
            soundVictory.Play();
            StartCoroutine(HandleRoundTransition());
        }

        public void EnemyVictory() 
        {
            enemyVictoryPoint += 1;
            soundDefead.Play();
            StartCoroutine(HandleRoundTransition());
        }

        private IEnumerator HandleDrawTransition()
        {

            if (playerState.currentHealth == enemyState.currentHealth) 
            {   
                yield return StartCoroutine(ShowRoundStartNotification(6)); // Final round
            }

            StartNormalTimer();
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
                yield return new WaitForSeconds(2f);
                VictoryNotif.SetActive(false);
                yield return StartCoroutine(ShowEndGameButton());
            }
            
            else if (enemyVictoryPoint == 3) 
            {
                ShowVictoryNotif(false);
                yield return new WaitForSeconds(1f);
                DefeatNotif.SetActive(false);
                yield return StartCoroutine(ShowEndGameButton());
            }
            else if (playerVictoryPoint == 2 && enemyVictoryPoint == 2 && !finalRound)
            {
                finalRound = true;
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
                int nextRound = playerVictoryPoint + enemyVictoryPoint + 1;
                
                yield return StartCoroutine(ShowRoundStartNotification(nextRound));
        }

        void StartNormalTimer()
        {
            TimeoutNotif.SetActive(true);
            matchTimer.normalTimerStart = 3;
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
            matchTimer.timerStart = 120;
            timeoutTriggered = false;

            // Reset health
            playerState.ResetHealth();
            enemyState.ResetHealth();
        }

    }
}
