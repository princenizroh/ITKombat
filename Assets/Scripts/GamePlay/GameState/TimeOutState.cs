using System;
using System.Collections;
using UnityEngine;

namespace ITKombat
{
    public class TimeOutState : IState
    {
        private MatchManager matchManager;
        public TimeOutState(MatchManager matchManager)
        {
            this.matchManager = matchManager;
        }

        public void Enter()
        {
            Debug.Log("Entering Timeout State");
            
            matchManager.TimeoutNotif.SetActive(true);
            matchManager.timeoutToTimer.text = "TIME OUT";
            matchManager.matchTimer.ChangeMatchStatus(false);  
            matchManager.StartCoroutine(GetWaitAndExecute());
            
        }

        private IEnumerator GetWaitAndExecute()
        {
            yield return new WaitForSeconds(3f);
            Execute();
        }
        public void Execute()
        {
            Debug.Log("Executing Timeout State");
            if (matchManager.playerVictoryPoint == 2 && matchManager.enemyVictoryPoint == 2 && matchManager.finalRound == true)
            {
                if (matchManager.playerVictoryPoint > matchManager.enemyVictoryPoint)
                {
                    matchManager.PlayerVictory();
                    matchManager.timeoutToTimer.text = "PLAYER WON FINAL ROUND";
                    Debug.Log("Player Win the final round by health");
                }
                else
                {
                    matchManager.EnemyVictory();
                    matchManager.timeoutToTimer.text = "ENEMY WON FINAL ROUND ";
                    Debug.Log("Enemy Win the final round by health");
                }
            }
            else
            {
                if (matchManager.playerState.currentHealth.Value == matchManager.enemyState.currentHealth.Value)
                {
                    matchManager.timeoutToTimer.text = "DRAW";
                    Debug.Log("Draw");
                }
                else if (matchManager.playerState.currentHealth.Value > matchManager.enemyState.currentHealth.Value)
                {
                    matchManager.PlayerVictory();
                    Debug.Log("Player Win by health");
                }
                else
                {
                    matchManager.EnemyVictory();
                    Debug.Log("Enemy Win by health");
                }
            }            
        }

        public void Exit()
        {
            Debug.Log("Exiting Timeout State");
            matchManager.TimeoutNotif.SetActive(false);
            matchManager.matchTimer.ChangeMatchStatus(true);
        }
        
    }
}
