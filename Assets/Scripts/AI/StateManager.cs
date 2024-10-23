using System.Collections;
using UnityEngine;

namespace ITKombat{
    public class StateManager : MonoBehaviour
    {
        public AI_Movement aimovement;
        public AI_Attack aiAttack;
        public AI_Defense aiDefense;
        public Transform player;
        public float waitingTime = 1f; // Delay time before next state

        private enum AIState { Approach, Retreat, Attack, Idle, Blocking }
        private AIState currentState = AIState.Idle;

        [System.Obsolete]
        void Start()
        {
            aiAttack = GetComponent<AI_Attack>();
            aimovement = GetComponent<AI_Movement>();
            aiDefense = GetComponent<AI_Defense>();
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

                    if (distanceToPlayer <= aiDefense.criticalProximity && Random.value < aiDefense.blockChance)
                    {
                        currentState = AIState.Blocking;
                    }
                    else if (aiAttack.canAttack && distanceToPlayer <= aiAttack.attackRange)
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
                    if (aimovement.canMove && aimovement.movementStep < aimovement.maxStep)
                    {
                        aimovement.Retreat();
                    }
                    else
                    {
                        StartCoroutine(ChangeStateWithDelay(AIState.Idle)); // Delay before moving to Idle
                    }

                    if (distanceToPlayer <= aiDefense.criticalProximity && Random.value < aiDefense.blockChance)
                    {
                        currentState = AIState.Blocking;
                    }
                    else if (aiAttack.canAttack && distanceToPlayer <= aiAttack.attackRange)
                    {
                        currentState = AIState.Attack;
                    }
                    break;
                
                case AIState.Blocking :
                    StartCoroutine(ActivateBlock());
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

        IEnumerator ActivateBlock()
        {
            aiDefense.isBlocking = true;
            Debug.Log("AI is blocking");

            yield return new WaitForSeconds(aiDefense.blockDuration);

            aiDefense.isBlocking = false;
            Debug.Log("Ai stop blocking");

            NextDecision();
        }

        // Coroutine to handle state change with a delay
        IEnumerator ChangeStateWithDelay(AIState nextState)
        {
            aimovement.StopMovement();
            aimovement.canMove = false; // Set waiting flag to true
            yield return new WaitForSeconds(waitingTime); // Wait for the defined delay
            currentState = nextState; // Change to the next state
            aimovement.canMove = true; // Reset waiting flag
        }
    }
}