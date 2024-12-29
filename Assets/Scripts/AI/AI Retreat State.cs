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

            // Debug.Log("Retreat State");
        }

        public override void UpdateState(AIStateManager manager, float distance){
            aiMovement.Retreat();
            steps = aiMovement.movementStep;

            if (steps >= 0.5){ 
                if (distance <= aiAttack.attackRange)
                {
                    aiMovement.StopMovement();
                    if (Random.value < 0.7f)
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
        }
    }
}
