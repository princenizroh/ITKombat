using UnityEngine;

namespace ITKombat
{
    public abstract class AIBaseState 
    {
        public abstract void EnterState(AIStateManager manager);
        public abstract void UpdateState(AIStateManager managerm, float distance);
        public abstract void ExitState(AIStateManager manager);
    }
}
