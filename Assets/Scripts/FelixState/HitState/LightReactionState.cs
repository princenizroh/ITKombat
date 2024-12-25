using UnityEngine;

namespace ITKombat{
public class LightReactionState : HitBaseState
{
    public override void OnEnter(HitFelixStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Attack
        hitIndex = 1;
        duration = 0.40f;
        animator.SetTrigger("TakeHit2");
        Debug.Log("Player Terserang " + hitIndex + " Kali!");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (fixedtime >= duration)
        {
            if (shouldHitCombo)
            {
                stateMachine.SetNextState(new MediumReactionState());
            }
            else
            {
                stateMachine.SetNextStateToMain();
            }
        }
    }
}

}
