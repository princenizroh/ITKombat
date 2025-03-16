using UnityEngine;

namespace ITKombat
{
    public class AICrouchAttackState : AIBaseState
    {
        private AI_Movement aiMovement;
        private bool isCrouchDown = false;
        public override void EnterState(AIStateManager manager)
        {
            aiMovement = manager.aiMovement;

            if (!isCrouchDown)
            {
                aiMovement.CrouchAttack();
                Debug.Log("Crouching State");
            }
            else
            {
                aiMovement.CrouchUp();
            }
        }


        public override void UpdateState(AIStateManager manager, float distance)
        {
            aiMovement = manager.aiMovement;

            if (!aiMovement.isCrouching)
            {
                manager.SwitchState(manager.IdleState);
            }
            else
            {
                aiMovement.CrouchAttack();
            }
        }
    }
}
