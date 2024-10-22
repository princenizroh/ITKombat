using TMPro;
using UnityEngine;
using System.Collections;

namespace ITKombat
{
    public class MatchManager : MonoBehaviour
    {
        public static MatchManager Instance;
        public GameObject ReadyNotif, Round1Notif, Round2Notif, FinalRoundNotif, FightNotif, DefeatNotif, VictoryNotif, TimeoutNotif;
        public int playerVictoryPoint;
        public int enemyVictoryPoint;
        public MatchTimer matchTimer;
        public TMP_Text timerText, timeoutToTimer, vpPlayer, vpEnemy;
        private bool timeoutTriggered = false;
        private bool timeoutTimer = false;
        private bool finalRound = false;

        // Reference to PlayerState and EnemyState
        public PlayerState playerState;
        public EnemyState enemyState;

        void Start() 
        {
            StartCoroutine(ShowRoundStartNotification(1));
        }

        private void Awake()
        {
            Instance = this;

            // Ensure PlayerState and EnemyState are correctly set up
            playerState = FindObjectOfType<PlayerState>();
            enemyState = FindObjectOfType<EnemyState>();
        }

        void Update()
        {
            timerText.text = matchTimer.GetStageTimeInSecond().ToString();

            vpPlayer.text = playerVictoryPoint.ToString();
            vpEnemy.text = enemyVictoryPoint.ToString();
            
            if (matchTimer.GetStageTimeInSecond() == 0 && !timeoutTriggered) 
            {
                // Handle timeout logic
                if (playerVictoryPoint == 2 && enemyVictoryPoint == 2 && finalRound == true) 
                {
                    TimeoutNotif.SetActive(true);
                    timeoutToTimer.text = "Draw";
                } 
                else 
                {
                    StartCoroutine(MatchTimeout());
                    timeoutTriggered = true;
                }
            } 
            else if (timeoutTimer == true) 
            {   
                matchTimer.ChangeMatchStatus(false);
                timeoutToTimer.text = matchTimer.GetNormalTimeInSecond().ToString();

                if (matchTimer.normalTimerStart <= 1f) 
                {
                    matchTimer.ChangeMatchStatus(true);
                    timeoutToTimer.text = "Fight";
                }
            }
        }

        public IEnumerator ShowRoundStartNotification(int roundNumber)
        {
            switch (roundNumber)
            {
                case 1:
                    Round1Notif.SetActive(true);
                    break;
                case 2:
                    Round2Notif.SetActive(true);
                    break;
                case 3:
                    FinalRoundNotif.SetActive(true);
                    break;
            }

            yield return new WaitForSeconds(2f);

            Round1Notif.SetActive(false);
            Round2Notif.SetActive(false);
            FinalRoundNotif.SetActive(false);

            matchTimer.ChangeMatchStatus(true);
            FightNotif.SetActive(true); 
            yield return new WaitForSeconds(1.5f);
            FightNotif.SetActive(false);
        }
        
        public void ShowVictoryNotif(bool isPlayerVictory)
        {
            if (isPlayerVictory)
            {
                VictoryNotif.SetActive(true);
            }
            else
            {
                DefeatNotif.SetActive(true);
            }
        }

        public void ShowEndGameButton()
        {
            GameObject endGameButton = GameObject.Find("EndGameButton");
            if (endGameButton != null)
            {
                endGameButton.SetActive(true);
            }
        }

        IEnumerator MatchTimeout() 
        {
            Debug.Log("Match Timeout");
            TimeoutNotif.SetActive(true);

            // Check health via PlayerState and EnemyState
            if (playerState != null && enemyState != null)
            {
                if (playerState.currentHealth < enemyState.currentHealth) 
                {
                    EnemyVictory();
                } 
                else 
                {
                    PlayerVictory();
                }
            }

            yield return new WaitForSeconds(5f);

            matchTimer.normalTimerStart = 6;
            timeoutTimer = true;

            yield return new WaitForSeconds(8f);

            TimeoutNotif.SetActive(false);
            timeoutTimer = false;

            // reset time
            matchTimer.timerStart = 120;
            timeoutTriggered = false;

            // Reset health for next round
            // HealthBar.Instance.SetMaxHealth(100);
            HealthBar.Instance.UpdateHealth(100f, 100f);
            playerState.ResetHealth();
            enemyState.ResetHealth();
        }

        public void PlayerVictory() 
        {
            matchTimer.ChangeMatchStatus(false);
            if (playerVictoryPoint <= 2) // Change to ensure gradual round victory
            {
                playerVictoryPoint += 1;
                StartCoroutine(StartRound("Player Victory"));
            } 
            else 
            {
                if (enemyVictoryPoint == 2 && !finalRound) 
                {
                    finalRound = true;
                    StartCoroutine(StartRound("Final Round"));
                } 
                else 
                {
                    TimeoutNotif.SetActive(true);
                    timeoutToTimer.text = "End Game - Player Win";
                }
            }
        }

        public void EnemyVictory() 
        {
            matchTimer.ChangeMatchStatus(false);
            if (enemyVictoryPoint <= 2) // Change to ensure gradual round victory
            {
                enemyVictoryPoint += 1;
                StartCoroutine(StartRound("Enemy Victory"));
            } 
            else 
            {
                if (playerVictoryPoint == 2 && !finalRound) 
                {
                    finalRound = true;
                    StartCoroutine(StartRound("Final Round"));
                } 
                else 
                {
                    TimeoutNotif.SetActive(true);
                    timeoutToTimer.text = "End Game - Enemy Win";
                }
            }
        }

        IEnumerator StartRound(string victory_status) 
        {
            TimeoutNotif.SetActive(true);
            timeoutToTimer.text = victory_status;

            yield return new WaitForSeconds(5f);

            matchTimer.normalTimerStart = 6;
            timeoutTimer = true;

            yield return new WaitForSeconds(8f);

            TimeoutNotif.SetActive(false);
            timeoutTimer = false;
            matchTimer.ChangeMatchStatus(true);
            
            // reset time
            matchTimer.timerStart = 120;
            timeoutTriggered = false;

            // Reset health for next round
            // HealthBar.Instance.SetMaxHealth(100);
            HealthBar.Instance.UpdateHealth(100f, 100f);
            playerState.ResetHealth();
            enemyState.ResetHealth();
        }
    }
}
