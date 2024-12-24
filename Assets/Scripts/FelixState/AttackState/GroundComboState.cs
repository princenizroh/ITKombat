using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ITKombat{
public class GroundComboState : MeleeBaseState
{
    public override void OnEnter(FelixStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Attack
        attackIndex = 2;
        duration = 0.5f;
        animator.SetTrigger("Attack" + attackIndex);
        Debug.Log("Player Attack " + attackIndex + " Fired!");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (fixedtime >= duration)
        {
            if (shouldCombo)
            {
                stateMachine.SetNextState(new GroundFinisherState());
            }
            else
            {
                stateMachine.SetNextStateToMain();
            }
        }
    }
}

}
