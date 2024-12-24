using UnityEngine;

namespace ITKombat
{
    public class AIDefenseState : AIBaseState
    {
        private float update;
        private float timer;

        public AIDefenseState(float updateTime){
            update = updateTime;
        }
        public override void EnterState(AIStateManager manager){
            Debug.Log("Defense State");
            timer = update;
        }

        public override void UpdateState(AIStateManager manager, float distance){
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                manager.SwitchState(manager.IdleState);
            }
        }

        public override void ExitState(AIStateManager manager){

        }
    }
}
