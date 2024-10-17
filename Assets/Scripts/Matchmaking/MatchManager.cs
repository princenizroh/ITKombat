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
        public HealthBarTest healthBar;
        // public EnemyStats enemyStats;
        public TMP_Text timerText, timeoutToTimer, vpPlayer, vpEnemy;
        private bool timeoutTriggered = false;
        private bool timeoutTimer = false;
        private bool finalRound = false;

        void Start() 
        {
            StartCoroutine(ShowRoundStartNotification(1));
        }

        private void Awake()
        {
            Instance = this;
        }

        void Update()
        {
            timerText.text = matchTimer.GetStageTimeInSecond().ToString();

            vpPlayer.text = playerVictoryPoint.ToString();
            vpEnemy.text = enemyVictoryPoint.ToString();
            
            if (matchTimer.GetStageTimeInSecond() == 0 && !timeoutTriggered) 
            {

                if (playerVictoryPoint == 2 && enemyVictoryPoint == 2 && finalRound == true ) {
                    
                    TimeoutNotif.SetActive(true);
                    timeoutToTimer.text = "Draw";

                } else {

                    StartCoroutine(MatchTimeout());
                    PlayerVictory();
                    timeoutTriggered = true;
                    Debug.Log("health : "+ healthBar.health );

                }

            } else if (timeoutTimer == true) {

                timeoutToTimer.text = matchTimer.GetNormalTimeInSecond().ToString();

                if (matchTimer.normalTimerStart <= 1f) {

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

            FightNotif.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            FightNotif.SetActive(false);
        }
        
        public void ShowVictoryNotif(bool isPlayerVictory)
        {
            if(isPlayerVictory)
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
            Debug.Log("Match is timeout");

            timeoutToTimer.text = "Timeout";
            
            // game timeout screen is activated
            TimeoutNotif.SetActive(true);

            if (healthBar.health < 50) {

                enemyVictoryPoint += 1;

                timeoutToTimer.text = "You Won 1 Point";
                Debug.Log("You Won 1 Point");
                
            } else {

                playerVictoryPoint += 1;

                timeoutToTimer.text = "Enemy Won";
                Debug.Log("Enemy Won");

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
