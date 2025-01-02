using UnityEngine;


namespace ITKombat
{
    public class AIApproachState : AIBaseState
    {
        AI_Movement aiMovement;
        AI_Attack aiAttack;
        AI_Defense aiDefense;
        float steps;

        public override void EnterState(AIStateManager manager){
            aiMovement = manager.aiMovement;
            aiAttack = manager.aiAttack;
            aiDefense = manager.aiDefense;

            // Debug.Log("Approach State");
        }

        public override void UpdateState(AIStateManager manager, float distance){
            aiMovement.Approach();
            steps = aiMovement.movementStep;

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
            else if (steps >= aiMovement.timeMoving)
            {
                manager.SwitchState(manager.IdleState);
            }
        }
    }
}
