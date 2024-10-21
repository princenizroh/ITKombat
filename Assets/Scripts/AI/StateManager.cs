using System.Collections;
using UnityEngine;

namespace ITKombat{
    public class StateManager : MonoBehaviour
    {
        public AI_Movement aimovement;
        public AI_Attack aiAttack;
        public Transform player;
        public float waitingTime = 0.5f; // Delay time before next state

        private enum AIState { Approach, Retreat, Jump, Attack, Attacked, Idle }
        private AIState currentState = AIState.Idle;

        [System.Obsolete]
        void Start()
        {
            aiAttack = GetComponent<AI_Attack>();
            aimovement = GetComponent<AI_Movement>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        void Update()
        {
            if (aimovement.canMove) // Ensure the state is not updated while waiting
            {
                StateHandle();
            }
        }

        void StateHandle()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            switch (currentState)
            {
                case AIState.Approach:
                    if (aimovement.canMove && aimovement.movementStep < aimovement.maxStep)
                    {
                        aimovement.Approach();
                    }
                    else
                    {
                        StartCoroutine(ChangeStateWithDelay(AIState.Idle)); // Delay before moving to Idle
                    }

                    if (aiAttack.canAttack && distanceToPlayer <= aiAttack.attackRange)
                    {
                        currentState = AIState.Attack;
                    }
                    break;

                case AIState.Attack:
                    if (aiAttack.canAttack) aiAttack.Attack();
                    if (aiAttack.currentCombo >= aiAttack.maxCombo || distanceToPlayer > aiAttack.attackRange)
                    {
                        NextAttackDecision();
                    }
                    break;

                case AIState.Retreat:
                    aimovement.Retreat();
                    if (aiAttack.canAttack && distanceToPlayer <= aiAttack.attackRange)
                    {
                        currentState = AIState.Attack;
                    }
                    else if (aimovement.movementStep > aimovement.maxStep)
                    {
                        StartCoroutine(ChangeStateWithDelay(AIState.Idle)); // Delay before Idle
                    }
                    break;
                case AIState.Idle :
                    NextDecision();
                    break;
            }
        }

        // Handle the next state decision
        void NextDecision()
        {
            float decision = Random.value;

            if (decision < 0.8f)
            {
                aimovement.movementStep = 0f;
                StartCoroutine(ChangeStateWithDelay(AIState.Approach)); // Delay before Approaching
                aiAttack.canAttack = true;
            }
            else
            {
                aimovement.movementStep = 0f;
                StartCoroutine(ChangeStateWithDelay(AIState.Retreat)); // Delay before Retreating
            }
        }

        // Handle the next attack decision
        void NextAttackDecision()
        {
            float decision = Random.value;

            if (decision < 0.3f)
            {
                aimovement.movementStep = 0f;
                StartCoroutine(ChangeStateWithDelay(AIState.Approach)); // Delay before Approaching
                aiAttack.canAttack = true;
            }
            else
            {
                aimovement.movementStep = 0f;
                StartCoroutine(ChangeStateWithDelay(AIState.Retreat)); // Delay before Retreating
            }
        }

        // Coroutine to handle state change with a delay
        IEnumerator ChangeStateWithDelay(AIState nextState)
        {
            aimovement.canMove = false; // Set waiting flag to true
            aimovement.StopMovement();
            yield return new WaitForSeconds(waitingTime); // Wait for the defined delay
            currentState = nextState; // Change to the next state
            aimovement.canMove = true; // Reset waiting flag
        }
    }
}