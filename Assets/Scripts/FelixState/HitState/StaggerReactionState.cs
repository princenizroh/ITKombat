using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ITKombat{
    public class StaggerReactionState : HitBaseState
{
    public override void OnEnter(HitFelixStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Attack
        hitIndex = 4;
        duration = 0.5f;
        animator.SetTrigger("Tumble");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (fixedtime >= duration)
        {
             stateMachine.SetNextState(new RiseState());
        }
    }
}

}
