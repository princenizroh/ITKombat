using TMPro;
using UnityEngine;
using System.Collections;

namespace ITKombat
{
    public class MatchManager : MonoBehaviour
    {
        public static MatchManager Instance;
        public GameObject ReadyNotif, Round1Notif, Round2Notif, Round3Notif, Round4Notif, FinalRoundNotif, FightNotif, DefeatNotif, VictoryNotif, TimeoutNotif;
        public int playerVictoryPoint;
        public int enemyVictoryPoint;
        public MatchTimer matchTimer;
        public TMP_Text timerText, timeoutToTimer, vpPlayer, vpEnemy;
        public bool timeoutTriggered = false;
        public bool timeoutTimer = false;
        private bool finalRound = false;

        // Reference to PlayerState and EnemyState
        private PlayerState playerState;
        private EnemyState enemyState;

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
                HandleTimeout();
            } 
            else if (timeoutTimer == true) 
            {   
                HandleTimeoutTimer();
            }
        }

        private void HandleTimeout()
        {
            // Handle timeout logic
            if (playerVictoryPoint == 2 && enemyVictoryPoint == 2 && finalRound == true) 
            {
                TimeoutNotif.SetActive(true);
                timeoutToTimer.text = "Draw";
            } 
            else 
            {
                MatchTimeout();
                timeoutTriggered = true;
            }
        }

        private void HandleTimeoutTimer()
        {
            matchTimer.ChangeMatchStatus(false);
            timeoutToTimer.text = matchTimer.GetNormalTimeInSecond().ToString();

            if (matchTimer.normalTimerStart <= 1f) 
            {
                matchTimer.ChangeMatchStatus(true);
                timeoutToTimer.text = "Fight";
            }
        }

       public IEnumerator ShowRoundStartNotification(int roundNumber)
        {
            GameObject currentRoundNotif = null;

            switch (roundNumber)
            {
                case 1: currentRoundNotif = Round1Notif; break;
                case 2: currentRoundNotif = Round2Notif; break;
                case 3: currentRoundNotif = Round3Notif; break;
                case 4: currentRoundNotif = Round4Notif; break;
                case 5: currentRoundNotif = FinalRoundNotif; break;
            }

            if (currentRoundNotif != null)
            {
                currentRoundNotif.SetActive(true);
                yield return new WaitForSeconds(2f);
                currentRoundNotif.SetActive(false);
                matchTimer.ChangeMatchStatus(true);

                if (roundNumber == 1)
                {
                    FightNotif.SetActive(true);
                    yield return new WaitForSeconds(1.5f);
                    FightNotif.SetActive(false);
                }
            }
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

        void MatchTimeout() 
        {
            Debug.Log("Match Timeout");
            TimeoutNotif.SetActive(true);
            Debug.Log("TimeOutNotif match timeout");

            // Check health via PlayerState and EnemyState
            if (playerState != null && enemyState != null)
            {
                if (playerVictoryPoint == 2 && enemyVictoryPoint == 2 && finalRound == true) 
                {
                    // Logika untuk mengecek siapa yang memiliki health lebih banyak
                    if (playerState.currentHealth == enemyState.currentHealth) 
                    {
                        timeoutToTimer.text = "Draw";
                    } 
                    else if (playerState.currentHealth > enemyState.currentHealth) 
                    {
                        PlayerVictory();
                        timeoutToTimer.text = "Player Victory";
                    } 
                    else 
                    {
                        EnemyVictory();
                        timeoutToTimer.text = "Enemy Victory";
                    }
                }
                else 
                {
                    // Jika bukan final round, tetap cek pemenang berdasarkan health
                    if (playerState.currentHealth == enemyState.currentHealth) 
                    {
                        timeoutToTimer.text = "Draw";
                    }
                    else if (playerState.currentHealth > enemyState.currentHealth) 
                    {
                        PlayerVictory();
                    }
                    else 
                    {
                        EnemyVictory();
                    }
                }
            }
        }
        public void PlayerVictory() 
        {
            playerVictoryPoint += 1;
            StartCoroutine(HandleRoundTransition());
        }

        public void EnemyVictory() 
        {
            enemyVictoryPoint += 1;
            StartCoroutine(HandleRoundTransition());
        }

        private IEnumerator HandleRoundTransition()
        {
            TimeoutNotif.SetActive(true);
            timeoutToTimer.text = playerVictoryPoint > enemyVictoryPoint ? "Player Victory" : "Enemy Victory";

            yield return new WaitForSeconds(3f);

            TimeoutNotif.SetActive(false);

            if (playerVictoryPoint == 2 && enemyVictoryPoint == 2 && !finalRound)
            {
                finalRound = true;
                yield return StartCoroutine(ShowRoundStartNotification(5)); // Final round
            }
            else if (playerVictoryPoint < 3 && enemyVictoryPoint < 3)
            {
                int nextRound = playerVictoryPoint + enemyVictoryPoint + 1;
                
                yield return StartCoroutine(ShowRoundStartNotification(nextRound));
            }
            else
            {
                timeoutToTimer.text = playerVictoryPoint > enemyVictoryPoint ? "End Game - Player Wins" : "End Game - Enemy Wins";
            }

            StartNormalTimer();
        }
        // public void PlayerVictory() 
        // {
        //     matchTimer.ChangeMatchStatus(false);
        //     Debug.Log("Player Victory Detected");
        //      // Kode untuk kemenangan player...
        //     Debug.Log($"Updated Scores - Player: {playerVictoryPoint}, Enemy: {enemyVictoryPoint}");
        //     // Jika player menang dan skor belum 2
        //     if (playerVictoryPoint < 2) 
        //     {
        //         playerVictoryPoint += 1;
        //         StartCoroutine(StartRound("Player Victory"));
        //         int nextRound = playerVictoryPoint + enemyVictoryPoint + 1;
        //         StartCoroutine(ShowRoundStartNotification(nextRound));
        //         Debug.Log($" 1 Updated Scores - Player: {playerVictoryPoint}, Enemy: {enemyVictoryPoint}");
        //     } 
        //     else if (playerVictoryPoint < 2 && enemyVictoryPoint == 1) 
        //     {
        //         enemyVictoryPoint += 1;
        //         StartCoroutine(StartRound("Player Victory"));
        //         Debug.Log($"1 Updated Scores - Player: {playerVictoryPoint}, Enemy: {enemyVictoryPoint}");
        //     } 
        //     else if (playerVictoryPoint == 2 && enemyVictoryPoint == 2 && !finalRound) 
        //     {
        //         // Jika skor 2-2, tampilkan final round
        //         finalRound = true;
        //         Debug.Log("kedudukan sama");
        //         StartCoroutine(StartRound("Final Round"));
        //     } 
        //     else if (playerVictoryPoint == 3)
        //     {
        //         // Jika player sudah menang dengan skor 2
        //         TimeoutNotif.SetActive(true);
        //         timeoutToTimer.text = "End Game - Player Win";
        //     }
        // }

        // public void EnemyVictory() 
        // {
        //     matchTimer.ChangeMatchStatus(false);
        //     Debug.Log("Enemy Victory Detected");
        //     Debug.Log($"Updated Scores - Player: {playerVictoryPoint}, Enemy: {enemyVictoryPoint}");
        //     // Jika enemy menang dan skor belum 2
        //     if (enemyVictoryPoint < 2) 
        //     {
        //         enemyVictoryPoint += 1;
        //         StartCoroutine(StartRound("Enemy Victory"));
        //         int nextRound = playerVictoryPoint + enemyVictoryPoint + 1;
        //         StartCoroutine(ShowRoundStartNotification(nextRound));
        //         Debug.Log($"1 Updated Scores - Player: {playerVictoryPoint}, Enemy: {enemyVictoryPoint}");
        //     } 
        //     else if (enemyVictoryPoint < 2 && playerVictoryPoint == 1) 
        //     {
        //         enemyVictoryPoint += 1;
        //         StartCoroutine(StartRound("Enemy Victory"));
        //         Debug.Log($"1 Updated Scores - Player: {playerVictoryPoint}, Enemy: {enemyVictoryPoint}");
        //     } 
        //     else if (playerVictoryPoint == 2 && enemyVictoryPoint == 2 && !finalRound) 
        //     {
        //         // Jika skor 2-2, tampilkan final round
        //         finalRound = true;
        //         Debug.Log("kedudukan sama");
        //         StartCoroutine(StartRound("Final Round"));

        //     } 
        //     else if (enemyVictoryPoint == 3)
        //     {
        //         // Jika enemy sudah menang dengan skor 2
        //         TimeoutNotif.SetActive(true);
        //         timeoutToTimer.text = "End Game - Enemy Win";
        //     }
        // }

        IEnumerator StartRound(string victory_status) 
        {
            Debug.Log($"Starting Round: PlayerSkor = {playerVictoryPoint}, EnemySkor = {enemyVictoryPoint}");
            TimeoutNotif.SetActive(true);
            timeoutToTimer.text = victory_status;

            yield return new WaitForSeconds(3f);

            // Sembunyikan notifikasi kemenangan
            TimeoutNotif.SetActive(false);
            
            // if (victory_status == "Player Victory")
            // {
            //     yield return StartCoroutine(RoundPlayerVictory());
            // }
            // if (victory_status == "Enemy Victory")
            // {
            //     yield return StartCoroutine(RoundEnemyVictory());
            // }

            // if (victory_status == "Final Round")
            // {
            //     yield return StartCoroutine(DrawRound());
            // }
            
            StartNormalTimer();
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

        IEnumerator RoundPlayerVictory()
        {
            // Tampilkan notifikasi round baru sesuai dengan skor
            if (playerVictoryPoint == 1 && enemyVictoryPoint == 0)
            {
                StartCoroutine(ShowRoundStartNotification(2));  // Tampilkan Round 2
                yield return new WaitForSeconds(2);
                Debug.Log("Round 2");
                TimeoutNotif.SetActive(true);
            }
            else if (playerVictoryPoint == 2 && enemyVictoryPoint == 0)
            {
                StartCoroutine(ShowRoundStartNotification(3));  // Tampilkan Round 3
                yield return new WaitForSeconds(2);
                Debug.Log("Round 3");
                TimeoutNotif.SetActive(true);
            }

            else if (playerVictoryPoint == 2 && enemyVictoryPoint == 1)
            {
                StartCoroutine(ShowRoundStartNotification(4));  // Tampilkan Round 4
                yield return new WaitForSeconds(2);
                Debug.Log("Round 4");
                TimeoutNotif.SetActive(true);
            }

            // Tampilkan notifikasi round baru sesuai dengan skor
            else if (playerVictoryPoint == 1 && enemyVictoryPoint == 1)
            {
                StartCoroutine(ShowRoundStartNotification(3));  // Tampilkan Round 3
                yield return new WaitForSeconds(2);
                Debug.Log("Round 3");
                TimeoutNotif.SetActive(true);
            }
        }

        IEnumerator RoundEnemyVictory()
        {
            
            // Tampilkan notifikasi round baru sesuai dengan skor
            if (enemyVictoryPoint == 1 && playerVictoryPoint == 0)
            {
                StartCoroutine(ShowRoundStartNotification(2));  // Tampilkan Round 2
                yield return new WaitForSeconds(2);
                Debug.Log("Round 2");
                TimeoutNotif.SetActive(true);
            }
            else if (enemyVictoryPoint == 2 && playerVictoryPoint == 0)
            {
                StartCoroutine(ShowRoundStartNotification(3));  // Tampilkan Round 3
                yield return new WaitForSeconds(2);
                Debug.Log("Round 3");
                TimeoutNotif.SetActive(true);
            }

            else if (enemyVictoryPoint == 2 && playerVictoryPoint == 1)
            {
                StartCoroutine(ShowRoundStartNotification(4));  // Tampilkan Round 4
                yield return new WaitForSeconds(2);
                Debug.Log("Round 4");
                TimeoutNotif.SetActive(true);
            }

            // Tampilkan notifikasi round baru sesuai dengan skor
            else if (playerVictoryPoint == 1 && enemyVictoryPoint == 1)
            {
                StartCoroutine(ShowRoundStartNotification(3));  // Tampilkan Round 3
                yield return new WaitForSeconds(2);
                Debug.Log("Round 3");
                TimeoutNotif.SetActive(true);
            }
        }

        IEnumerator DrawRound()
        {
            if (playerVictoryPoint == 2 && enemyVictoryPoint == 2)
            {
                StartCoroutine(ShowRoundStartNotification(5));  // Tampilkan Final Round
                yield return new WaitForSeconds(2);
                Debug.Log("Final Round");
                TimeoutNotif.SetActive(true);
            }
        }
    }
}
