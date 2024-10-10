using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Movement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public bool facingPlayer = true;
    private Transform player;
    private Rigidbody2D myRigidbody;
    private AI_Attack aiAttack;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        aiAttack = GetComponent<AI_Attack>();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if the AI is within attack range or currently attacking
        if (distanceToPlayer > aiAttack.attackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StopMovement();  // Ensure movement stops when attacking
        }
    }

    public void StopMovement()
    {
        // Stop the AI movement
        myRigidbody.linearVelocity = Vector2.zero;
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        if ((direction.x > 0 && facingPlayer) || (direction.x < 0 && !facingPlayer))
        {
            Flip();
        }

        myRigidbody.linearVelocity = new Vector2(direction.x * moveSpeed, myRigidbody.linearVelocity.y);
    }

    void Flip()
    {
        facingPlayer = !facingPlayer;
        Vector2 localScale = transform.localScale;
        localScale.x *= -1; 
        transform.localScale = localScale;
    }
}
