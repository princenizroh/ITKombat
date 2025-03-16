using UnityEngine;

namespace ITKombat
{
    public class AIStateManager : MonoBehaviour
    {
        AIBaseState currentState;
        public Transform player;
        public AI_Movement aiMovement;
        public AI_Attack aiAttack;
        public AI_Defense aiDefense;

        public AIAttackState AttackState = new AIAttackState();
        public AIDefenseState DefenseState = new AIDefenseState(blockDuration : 0.4f);
        public AIApproachState ApproachState = new AIApproachState();
        public AIRetreatState RetreatState = new AIRetreatState();
        public AIDashState DashState = new AIDashState();
        public AIIdleState IdleState = new AIIdleState(IdleDuration : 0.1f);
        public AICrouchState CrouchState = new AICrouchState();
        public AICrouchAttackState CrouchAttackState = new AICrouchAttackState();
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            aiMovement = GetComponent<AI_Movement>();
            aiAttack = GetComponent<AI_Attack>();
            aiDefense = GetComponent<AI_Defense>();
            player = GameObject.FindGameObjectWithTag("Player").transform;

            currentState = IdleState;
            currentState.EnterState(this);
        }

        // Update is called once per frame
        void Update()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position); 
            currentState.UpdateState(this, distanceToPlayer);
        }

        public void SwitchState(AIBaseState state)
        {
            currentState = state;
            state.EnterState(this);
        }
    }
}
