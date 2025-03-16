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

        public AIIdleState(float IdleDuration)
        {
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

            //manager.SwitchState(manager.CrouchAttackState);

            if (IdleTimer <= 0)
            {
                if (distance <= aiAttack.attackRange)
                {
                    Debug.Log("IdleState: Player berada pada jarak serang AI");
                    manager.SwitchState(Random.value < 0.7f ? manager.AttackState : manager.DefenseState);
                }
                else if (distance > aiMovement.maxDistance)
                {
                    manager.SwitchState(Random.value < 0.7f ? manager.DashState : manager.ApproachState);
                }
                else
                {
                    Debug.Log("IdleState: kondisi tidak diketahui, Memilih opsi gerakan terakhir");
                    float rand = Random.value;
                    if (rand < 0.3f) 
                    {
                        manager.SwitchState(manager.RetreatState);
                    }
                    else if (rand < 0.6f)
                    {
                        manager.SwitchState(manager.DashState);
                    }
                    else
                    {
                        manager.SwitchState(manager.ApproachState);
                    }
                }
            }
        }
        
    }
}
