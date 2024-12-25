using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ITKombat{
    public class RiseState : HitBaseState
{
    public override void OnEnter(HitFelixStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Attack
        duration = 1f;
        animator.SetTrigger("Wake");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (fixedtime >= duration)
        {
             stateMachine.SetNextStateToMain();
        }
    }
}

}
