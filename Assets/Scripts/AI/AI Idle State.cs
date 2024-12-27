using UnityEngine;

namespace ITKombat
{
    public class AIIdleState : AIBaseState
    {
        private AI_Movement aiMovement;
        private AI_Attack aiAttack;
        private AI_Defense aiDefense;
        private float IdleTime;
        private float IdleTimer;

        public AIIdleState(float IdleDuration){
            IdleTime = IdleDuration;
        }

        public override void EnterState(AIStateManager manager)
        {   
            aiMovement = manager.aiMovement;
            aiAttack = manager.aiAttack;
            aiDefense = manager.aiDefense;

            aiMovement.movementStep = 0;
            Debug.Log("Enter Idle");
            IdleTimer = IdleTime;

        }

        public override void UpdateState(AIStateManager manager, float distance)
        {
            aiMovement.StopMovement();
            IdleTimer -= Time.deltaTime;

            if (IdleTimer <= 0)
            {
                if (distance <= aiAttack.attackRange)
                {
                    if (Random.value < 0.7f)
                    {
                        manager.SwitchState(manager.AttackState); // 70% menyerang
                    }
                    else
                    {
                        manager.SwitchState(manager.DefenseState); // 30% bertahan
                    }
                }
                else if (distance > aiMovement.maxDistance)
                {
                    manager.SwitchState(manager.ApproachState);
                }
                else
                {
                    if (Random.value < 0.80)
                    {
                        manager.SwitchState(manager.ApproachState); // 75% maju
                    }                    
                    else 
                    {
                        manager.SwitchState(manager.RetreatState); // 25% mundur
                    }
                }
            }
        }
    }
}
