using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ITKombat{
public class MediumReactionState : HitBaseState
{
    public override void OnEnter(HitFelixStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Attack
        hitIndex = 2;
        duration = 0.5f;
        animator.SetTrigger("TakeHit" + hitIndex);
        Debug.Log("Player Terserang " + hitIndex + " Kali!");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (fixedtime >= duration)
        {
            if (shouldHitCombo)
            {
                stateMachine.SetNextState(new HeavyReactionState());
            }
            else
            {
                stateMachine.SetNextStateToMain();
            }
        }
    }
}

}