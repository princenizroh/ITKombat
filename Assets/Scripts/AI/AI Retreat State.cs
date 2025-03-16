using UnityEngine;

namespace ITKombat
{
    public class AIRetreatState : AIBaseState
    {
        AI_Movement aiMovement;
        AI_Attack aiAttack;
        AI_Defense aiDefense;

        bool isApproaching = false;
        float steps;
        public override void EnterState(AIStateManager manager){
            aiMovement = manager.aiMovement;
            aiAttack = manager.aiAttack;
            aiDefense = manager.aiDefense;

            aiMovement.movementStep = 0f;

            Debug.Log("Retreat State");


        }

        public override void UpdateState(AIStateManager manager, float distance){
            steps = aiMovement.movementStep;
            aiMovement.Retreat();

            if (steps >= Random.Range(0.6f, 2f))
            {
                manager.SwitchState(manager.ApproachState);
            } else
            {
                if (Random.value < 0.05f)
                {
                    aiMovement.Dash();
                }
            }
            //if (steps >= 1.3f){ 
            //    if (distance <= aiAttack.attackRange)
            //    {
            //        aiMovement.StopMovement();
            //        if (Random.value < 0.9f)
            //            {
            //                manager.SwitchState(manager.AttackState); // 70% menyerang
            //            }
            //            else
            //            {
            //                manager.SwitchState(manager.DefenseState); // 30% bertahan
            //            }
            //    }
            //    else if (steps >= aiMovement.timeMoving || distance > aiMovement.maxDistance)
            //    {
            //        manager.SwitchState(manager.IdleState);
            //    }
            //}
            //else 
            //if (Random.value < 0.0f)
            //{
            //        manager.SwitchState(manager.DashState);
            //}
            //else
            //{
            //    if(Random.value < 1f)
            //    {
            //        aiMovement.Retreat();
            //    } else
            //    {
            //        aiMovement.Approach();
            //        manager.SwitchState(manager.IdleState);
            //    }
            //}

        }
    }
}
