using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public AI_Movement aimovement;
    public AI_Attack aiAttack;
    public Transform player;
    public float retreatRange = 2f;
    
    private enum AIState { Approach, Retreat, Jump, Attack, Attacked, Idle }
    private AIState currentState = AIState.Approach;

    void Start()
    {
        aiAttack = GetComponent<AI_Attack>();
        aimovement = GetComponent<AI_Movement>();
    }

    void Update()
    {
        StateHandle();
    }

    void StateHandle()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case (AIState.Approach) :
                if (aimovement.movementStep < aimovement.maxStep)
                {
                    aimovement.Approach();
                }else{
                    NextDecision();
                }

                if (aiAttack.canAttack && distanceToPlayer <= aiAttack.attackRange)
                {
                    currentState = AIState.Attack;
                }
                break;
            
            
            case (AIState.Attack):
                aiAttack.Attack();
                if (aiAttack.currentCombo >= aiAttack.maxCombo || distanceToPlayer > aiAttack.attackRange)
                {
                    NextDecision();
                }
                break;
            
            case (AIState.Retreat):
                aimovement.Retreat();
                if(aiAttack.canAttack && distanceToPlayer <= aiAttack.attackRange)
                {
                    currentState = AIState.Attack;
                }
                else if (aimovement.movementStep > aimovement.maxStep)
                {
                    NextDecision();
                }
                break;
            
            case (AIState.Idle):
                aimovement.StopMovement();
                break;
        }
    }

    void NextDecision()
    {
        float decision = Random.value;

        if (decision < 0.6f)
        {
            currentState = AIState.Approach;
            aimovement.movementStep = 0f;
            aiAttack.canAttack = true;
            aiAttack.currentCombo = 0;
        }
        else{
            currentState = AIState.Retreat;
            aimovement.movementStep = 0f;
        }
    }
}
