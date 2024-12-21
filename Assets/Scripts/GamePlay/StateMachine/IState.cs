using UnityEngine;

namespace ITKombat
{
    public interface IState
    {
        public void Enter();
        public void Exit();
        public void Execute();
    }
}
