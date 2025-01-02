using UnityEngine;

namespace ITKombat
{
    public class AIRetreatState : AIBaseState
    {
        AI_Movement aiMovement;
        AI_Attack aiAttack;
        AI_Defense aiDefense;
        float steps;
        public override void EnterState(AIStateManager manager){
            aiMovement = manager.aiMovement;
            aiAttack = manager.aiAttack;
            aiDefense = manager.aiDefense;

            aiMovement.movementStep = 0f;

            // Debug.Log("Retreat State");
        }

        public override void UpdateState(AIStateManager manager, float distance){
            aiMovement.Retreat();
            steps = aiMovement.movementStep;

            if (steps >= 1.3f){ 
                if (distance <= aiAttack.attackRange)
                {
                    aiMovement.StopMovement();
                    if (Random.value < 0.8f)
                        {
                            manager.SwitchState(manager.AttackState); // 70% menyerang
                        }
                        else
                        {
                            manager.SwitchState(manager.DefenseState); // 30% bertahan
                        }
                }
                else if (steps >= aiMovement.timeMoving || distance > aiMovement.maxDistance)
                {
                    manager.SwitchState(manager.IdleState);
                }
            }
            else if(steps > 1.2f) {
                if (Random.value < 0.15f && aiMovement.maxDistance > distance){
                    manager.SwitchState(manager.DashState);
                }
            }
        }
    }
}
