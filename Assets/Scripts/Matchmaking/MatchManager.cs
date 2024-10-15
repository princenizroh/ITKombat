using TMPro;
using UnityEngine;
using System.Collections;

namespace ITKombat
{
    public class MatchManager : MonoBehaviour
    {
        public GameObject ReadyNotif, Round1Notif, Round2Notif, FinalRoundNotif, DefeatNotif, VictoryNotif, TimeoutNotif;
        public int playerVictoryPoint;
        public int enemyVictoryPoint;
        public MatchTimer matchTimer;
        public HealthBarTest healthBar;
        // public EnemyStats enemyStats;
        public TMP_Text timerText, timeoutToTimer, vpPlayer, vpEnemy;
        private bool timeoutTriggered = false;
        private bool timeoutTimer = false;
        private bool finalRound = false;

        void Start() {

        }

        void Update()
        {
            timerText.text = matchTimer.GetStageTimeInSecond().ToString();

            vpPlayer.text = playerVictoryPoint.ToString();
            vpEnemy.text = enemyVictoryPoint.ToString();
            
            if (matchTimer.GetStageTimeInSecond() == 0 && !timeoutTriggered) 
            {

                if (playerVictoryPoint == 2 && enemyVictoryPoint == 2 && finalRound == true) {
                    
                    TimeoutNotif.SetActive(true);
                    timeoutToTimer.text = "Draw";

                } else {

                    // StartCoroutine(MatchTimeout());
                    PlayerVictory();
                    timeoutTriggered = true;

                }

            } else if (timeoutTimer == true) {

                timeoutToTimer.text = matchTimer.GetNormalTimeInSecond().ToString();

                if (matchTimer.normalTimerStart <= 1f) {

                    timeoutToTimer.text = "Fight";

                }

            }
        }

        IEnumerator MatchTimeout() 
        {
            Debug.Log("Match is timeout");

            timeoutToTimer.text = "Timeout";
            
            // game timeout screen is activated
            TimeoutNotif.SetActive(true);

            // if (healthBar.health < 50) {

            //     enemyVictoryPoint += 1;

            //     timeoutToTimer.text = "You Won 1 Point";
                
            // } else {

            //     playerVictoryPoint += 1;

            //     timeoutToTimer.text = "Enemy Won";

            // }

            yield return new WaitForSeconds(5f);

            matchTimer.normalTimerStart = 6;

            timeoutTimer = true;

            yield return new WaitForSeconds(8f);

            TimeoutNotif.SetActive(false);

            timeoutTimer = false;

            // reset time
            matchTimer.timerStart = 120;
            timeoutTriggered = false;

            // spawn
            healthBar.health = 100;

        }

        public void PlayerVictory() {

            if (playerVictoryPoint <= 1) {

                playerVictoryPoint += 1;

                StartCoroutine(StartRound("Player Victory"));

            } else {

                if (enemyVictoryPoint == 2 && finalRound == false) {

                    finalRound = true;
                    StartCoroutine(StartRound("Final Round"));

                } else {

                    TimeoutNotif.SetActive(true);
                    timeoutToTimer.text = "End Game - Player Win";

                }     

            }

        }

        public void EnemyVictory() {

            if (enemyVictoryPoint <= 1) {

                enemyVictoryPoint += 1;

                StartCoroutine(StartRound("Enemy Victory"));

            } else {

                if (playerVictoryPoint == 2 && finalRound == false) {

                    finalRound = true;
                    StartCoroutine(StartRound("Final Round"));

                } else {

                    TimeoutNotif.SetActive(true);
                    timeoutToTimer.text = "End Game - Enemy Win";

                }

            }

        }

        IEnumerator StartRound(string victory_status) {

            TimeoutNotif.SetActive(true);
            timeoutToTimer.text = victory_status;

            yield return new WaitForSeconds(5f);

            matchTimer.normalTimerStart = 6;

            timeoutTimer = true;

            yield return new WaitForSeconds(8f);

            TimeoutNotif.SetActive(false);

            timeoutTimer = false;

            // reset time
            matchTimer.timerStart = 120;
            timeoutTriggered = false;

            // spawn
            healthBar.health = 100;


        }

    }
}
