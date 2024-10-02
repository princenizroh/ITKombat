using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Movement : MonoBehaviour
{
    public float moveSpeed = 3f;
    // public float attackRange = 2f;
    // public float attackCooldown = 0.5f;
    // private float nextMoveTime = 0f; // Waktu berikutnya AI bisa bergerak
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
        if (Vector2.Distance(transform.position, player.position) <= aiAttack.attackRange)
        {
            is_Touching();
        }
        else if (Vector2.Distance(transform.position,player.position) > aiAttack.attackRange)
        {
            MoveTowardsPlayer();
            // nextMoveTime = Time.time + moveInterval; // Atur waktu berikutnya untuk bergerak
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        if ((direction.x > 0 && facingPlayer) || (direction.x < 0 && !facingPlayer))
        {
            Flip();
        }

        myRigidbody.velocity = new Vector2 (direction.x * moveSpeed, myRigidbody.velocity.y);
    }

    // void Attack()
    // {
    //     int randomAttack = Random.Range(1, 4); 
    //     Debug.Log("Enemy melakukan serangan: Attack" + randomAttack);
    // }

    void is_Touching()
    {
        Debug.Log("Enemy Menyentuh Player, Berhenti Bergerak");
        myRigidbody.velocity = Vector2.zero;
    }

    void Flip()
    {
        facingPlayer = !facingPlayer;
        Vector2 localScale = transform.localScale;
        localScale.x *= -1; 
        transform.localScale = localScale;
    }
}
