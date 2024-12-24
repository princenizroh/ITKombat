using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ITKombat
{
    public class AIAttackState : AIBaseState
    {
        AI_Movement aiMovement;
        AI_Attack aiAttack;
        AI_Defense aiDefense;
        public override void EnterState(AIStateManager manager){
            aiMovement = manager.aiMovement;
            aiAttack = manager.aiAttack;
            aiDefense = manager.aiDefense;

            Debug.Log("Attack State");
        }

        public override void UpdateState(AIStateManager manager, float distance){
            if (aiAttack.canAttack){
                aiAttack.Attack();
            }
            if (aiAttack.currentCombo == 0 || distance > aiAttack.attackRange)
            {
                manager.SwitchState(manager.RetreatState);
            }
        }
    }
}
