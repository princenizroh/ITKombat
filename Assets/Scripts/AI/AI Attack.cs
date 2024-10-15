using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Attack : MonoBehaviour
{
    public float attackRange = 3f;         // Range within which the enemy can attack
    public float attackForce = 50f;          // Knockback force
    public float attackCooldown = 0.5f;      // Cooldown between each attack
    public float comboResetTime = 1f;        // Cooldown after completing the combo
    public int maxCombo = 4;                 // Maximum combo count
    public bool canAttack = true;           // Can the AI attack

    public int currentCombo = 0;            // Tracks the current combo
    public float lastAttackTime = 0f;       // Time of the last attack

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
        
    }

    public void Attack()
    {
        if (canAttack && Time.time - lastAttackTime > attackCooldown)
        {
            if (currentCombo == 0 || Time.time - lastAttackTime > attackCooldown*2)
            {
                currentCombo = 1;
            }
            else
            {
                currentCombo ++;
            }

            lastAttackTime = Time.time;
            
            Debug.Log("Enemy performs attack : Attack" + (currentCombo));
            StartCoroutine(AttackCooldown());

            if (currentCombo == maxCombo)
                {
                    Knockback();
                    currentCombo = 0;
                    StartCoroutine(ComboCooldown());
                }
        }
    }

    void Knockback()
    {
        if (playerRigidbody != null)
        {
            Vector2 knockbackDirection = (player.position - transform.position).normalized;
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.AddForce(knockbackDirection * (50*attackForce), ForceMode2D.Force);
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
        Debug.Log("Combo Reset");
        // Reset the combo counter after cooldown
        // currentCombo = 0;
        canAttack = true;
    }
}
