
using UnityEngine;

namespace ITKombat
{
    public class AIDashState : AIBaseState
    {
        AI_Movement aiMovement;
        public override void EnterState(AIStateManager manager){
            aiMovement = manager.aiMovement;

            aiMovement.Dash();

            Debug.Log("Dash State");
        }

        public override void UpdateState(AIStateManager manager, float distance){
            manager.SwitchState(manager.IdleState);
        }
    }
}
