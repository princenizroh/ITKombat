using UnityEngine;

namespace ITKombat
{
    public class AICrouchState : AIBaseState
    {
        private AI_Movement aiMovement;

        public override void EnterState(AIStateManager manager)
        {
            aiMovement = manager.aiMovement;

            aiMovement.CrouchDown();

            Debug.Log("Crouching State");
        }


        public override void UpdateState(AIStateManager manager, float distance)
        {
            manager.SwitchState(manager.IdleState);
        }
    }
}
