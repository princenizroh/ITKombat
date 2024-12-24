using UnityEngine;

namespace ITKombat
{
    public class HitBaseState : HitFelixState
    {
        // How long this state should be active for before moving on
        public float duration;
        // Cached animator component
        protected Animator animator;
        // bool to check whether or not the next attack in the sequence should be played or not
        protected bool shouldHitCombo;
        // The attack index in the sequence of attacks
        protected int hitIndex;

        private float HitReactionTimer = 0;

        public override void OnEnter(HitFelixStateMachine _stateMachine)
        {
            base.OnEnter(_stateMachine);
            animator = GetComponent<Animator>();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            HitReactionTimer -= Time.deltaTime;

            if (EnemyState.Instance.CheckDamage()) //Ini apa yang benar
            {
                HitReactionTimer = 2;
            }

            if (animator.GetFloat("HitWindow.Open") > 0f && HitReactionTimer > 0)
            {
                shouldHitCombo = true;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
