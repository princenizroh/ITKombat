using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ITKombat{
    public class MeleeEntryState : FelixState
{
    public override void OnEnter(FelixStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        FelixState nextState = (FelixState)new GroundEntryState();
        stateMachine.SetNextState(nextState);
    }
}
}

