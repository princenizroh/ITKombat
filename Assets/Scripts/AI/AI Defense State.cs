using Mono.CSharp;
using UnityEngine;
using UnityEngine.Rendering;

namespace ITKombat
{
    public class AIDefenseState : AIBaseState
    {
        AI_Defense aiDefense;
        AI_Movement aiMovement;
        float duration;
        float blockTimer;
        public AIDefenseState(float blockDuration){
            duration = blockDuration;
        }
        public override void EnterState(AIStateManager manager){
            aiDefense = manager.aiDefense;
            aiMovement = manager.aiMovement;

            blockTimer = duration;
            // Debug.Log("Defense State");
        }

        public override void UpdateState(AIStateManager manager, float distance){
            aiDefense.isBlocking = true;
            blockTimer -= Time.deltaTime;

            if (blockTimer <= 0)
            {
                aiDefense.isBlocking = false;
                aiMovement.movementStep = 0;
                manager.SwitchState(manager.RetreatState);
            }
        }
    }
}
