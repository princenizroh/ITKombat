using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Attack : MonoBehaviour
{
    public float attackRange = 2.5f;         // Range within which the enemy can attack
    // public float attackCooldown = 0.5f;    // Cooldown between attacks
    public float attackForce = 20f;
    // private float nextAttackTime = 0f;     // Time when the enemy can attack again
    public float currentCombo = 0;
    private Transform player;
    private Rigidbody2D playerRigidbody;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerRigidbody = player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            Attack();
            // nextAttackTime = Time.time + attackCooldown; 
        }
    }

    void Attack()
    {
        // Trigger a random attack
        // Debug.Log("Enemy performs attack: Attack" + currentCombo);
        currentCombo ++;
        Knockback();
    }

    void Knockback()
    {
        if (playerRigidbody != null)
        {
            Vector2 knockbackDirection = (player.position - transform.position).normalized;
            playerRigidbody.AddForce(knockbackDirection * attackForce, ForceMode2D.Impulse);
            Debug.Log("Player hit");
        }
    }
}
