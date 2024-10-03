// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class AI_Attack : MonoBehaviour
// {
//     public float attackRange = 2.5f;         // Range within which the enemy can attack
//     // public float attackCooldown = 0.5f;    // Cooldown between attacks
//     public float attackForce = 20f;
//     // private float nextAttackTime = 0f;     // Time when the enemy can attack again
//     public float currentCombo = 0;
//     private Transform player;
//     private Rigidbody2D playerRigidbody;
    
//     void Start()
//     {
//         player = GameObject.FindGameObjectWithTag("Player").transform;
//         playerRigidbody = player.GetComponent<Rigidbody2D>();
//     }

//     void Update()
//     {
//         float distanceToPlayer = Vector2.Distance(transform.position, player.position);
//         if (distanceToPlayer <= attackRange)
//         {
//             Attack();
//             // nextAttackTime = Time.time + attackCooldown; 
//         }
//     }

//     void Attack()
//     {
//         // Trigger a random attack
//         Debug.Log("Enemy performs attack: Attack" + currentCombo);
//         currentCombo ++;
//         Knockback();
//     }

//     void Knockback()
//     {
//         if (playerRigidbody != null)
//         {
//             Vector2 knockbackDirection = (player.position - transform.position).normalized;
//             playerRigidbody.AddForce(knockbackDirection * attackForce, ForceMode2D.Impulse);
//             Debug.Log("Player hit");
//         }
//     }
// }


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Attack : MonoBehaviour
{
    public float attackRange = 2.5f;         // Range within which the enemy can attack
    public float attackForce = 30f;          // Knockback force
    public float attackCooldown = 2f;      // Cooldown between each attack
    public float comboResetTime = 4f;        // Cooldown after completing the combo
    public int maxCombo = 4;                 // Maximum combo count
    public bool canAttack = true;           // Can the AI attack

    private int currentCombo = 0;            // Tracks the current combo
    private float lastAttackTime = 0f;       // Time of the last attack

    private Transform player;
    private Rigidbody2D playerRigidbody;
    private AI_Movement aiMovement;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerRigidbody = player.GetComponent<Rigidbody2D>();
        aiMovement = GetComponent<AI_Movement>();  // Reference to AI_Movement
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Check if AI can attack and if within attack range
        if (canAttack && distanceToPlayer <= attackRange)
        {
            Attack();
        }

        // Stop movement if currently attacking
        if (!canAttack)
        {
            aiMovement.StopMovement();
        }
    }

    void Attack()
    {
        if (currentCombo < maxCombo)
        {
            // Attack logic
            Debug.Log("Enemy performs attack: Attack " + (currentCombo + 1));
            Knockback();
            
            // Increment combo count
            currentCombo++;
            lastAttackTime = Time.time;

            // Check if the combo is complete
            if (currentCombo == maxCombo)
            {
                StartCoroutine(ComboCooldown());
            }
            else
            {
                StartCoroutine(AttackCooldown());
            }
        }
    }

    void Knockback()
    {
        if (playerRigidbody != null)
        {
            Vector2 knockbackDirection = (player.position - transform.position).normalized;
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.AddForce(knockbackDirection * attackForce, ForceMode2D.Impulse);
            Debug.Log("Player hit by knockback");
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private IEnumerator ComboCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(comboResetTime);

        // Reset the combo counter after cooldown
        currentCombo = 0;
        canAttack = true;
        Debug.Log("Combo reset after completing full sequence");
    }
}
