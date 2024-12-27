using Unity.VisualScripting;
using UnityEngine;

namespace ITKombat
{
    public class PlayerDamageChecker : MonoBehaviour
    {

        public static PlayerDamageChecker Instance;
        private HitFelixStateMachine meleeStateMachine;
        private Animator enemyAnimator;



        private void Awake()
        {
            Instance = this;
            enemyAnimator = GetComponent<Animator>();
            meleeStateMachine = GetComponent<HitFelixStateMachine>();
        }

        public void OnEnemyDamaged()
        {
            // Debug.Log("Enemy terkena damage!");

            if (meleeStateMachine == null)
            {
                Debug.LogError("FelixStateMachine is null! Ensure it's added to the GameObject.");
                return;
            }

            if (meleeStateMachine.CurrentState == null)
            {
                Debug.LogError("CurrentState is null! Check the state initialization in FelixStateMachine.");
                return;
            }

            if (meleeStateMachine.CurrentState.GetType() == typeof(HitIdleCombatState))
            {
                meleeStateMachine.SetNextState(new LightReactionState());
            }
        }

    }
}
