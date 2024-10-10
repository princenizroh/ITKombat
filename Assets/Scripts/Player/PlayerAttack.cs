using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackForce = 10f;     // Adding force in between attacks
    public float attackRadius = 1f;     // Radius for the area of effects
    public float attackCooldown = 0.5f; // Adjust attack cooldown
    public int maxCombo = 4;            // Max Combo
    public float comboResetTime = 1f; // Adjust combo cooldown
    public int requiredComboChain = 1;

    private int currentCombo = 0;
    private bool canAttack = true;
    private float lastAttackTime = 0f;

    [Header("Input Action")]
    private InputActionAsset inputActionAsset;
    private InputAction attackAction;

    PlayerControls controls;

    [Header("Damage Settings")]
    public int damageAmount = 25; // Damage to deal
    public LayerMask enemyLayer;    // Tag/Layernya kasih Player biar ke detect damagenya ke Player
    private GameObject parentPlayer; // Parent player GameObject

    private void Start()
    {
        // Get parent from GameObject of the player
        parentPlayer = transform.root.gameObject; // Assume the player object is a child under "Player"

        // Ensure the attackPoint has a collider and is set to trigger
        var collider = attackPoint.gameObject.GetComponent<Collider2D>();
        {
            collider = attackPoint.gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
        }
    }

    private void Awake()
    {
        // Finding Input Action for the Attack
        InputActionMap playerActionMap = inputActionAsset.FindActionMap("Player");
        playerActionMap.Enable();
        attackAction = playerActionMap.FindAction("Attack");

        // Input Action scritps
        controls = new PlayerControls();
        controls.Enable();
        attackAction = controls.Player.Attack;

        attackAction.performed += ctx => Attack();
        
    }

    public void OnAttackInput()
    {
        if (canAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (currentCombo < maxCombo && canAttack)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyRb.AddForce(transform.right * attackForce, ForceMode2D.Impulse);
                }
            }

            currentCombo++;
            lastAttackTime = Time.time;

            Debug.Log("Attack! Combo hit: " + currentCombo);


            // Combo Cooldown

            if (currentCombo == maxCombo)
            {
                StartCoroutine(ComboCooldown());
            }

            else
            {
                // If Combo already Max then start the timed AttackCooldown timer

                StartCoroutine(AttackCooldown());
            }

        }
    }
    private IEnumerator AttackCooldown()
    {
        // Pauses Attack
        canAttack = false;
        yield return new WaitForSecondsRealtime(attackCooldown);
        
        // Resetting Combo per Time
        if (currentCombo >= requiredComboChain && Time.time - lastAttackTime > comboResetTime)
        {
            currentCombo = 0;
            Debug.Log("Combo reset due to inactivity.");
        }
        canAttack = true;
    }

    private IEnumerator ComboCooldown()
    {
        // Pauses Cooldown
        canAttack = false;
        yield return new WaitForSeconds(comboResetTime);

        // Reset Combo After Max
        currentCombo = 0;
        canAttack = true;

        Debug.Log("Combo reset after completing full combo.");
    }

    // Making Attack Area Visible
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get root object of the collided object
        GameObject otherPlayer = collision.transform.root.gameObject;

        // Ensure the collided object is a different player and not the player themselves
        // Tagnya harus "Player" buat ngasih damage ke player lain
        if (otherPlayer == parentPlayer.transform.parent && otherPlayer.CompareTag("Player"))
        {
            HealthBar otherHealth = otherPlayer.GetComponentInChildren<HealthBar>();
            
            if (otherHealth != null)
            {
                // Deal damage to the other player
                otherHealth.TakeDamage(damageAmount);
                Debug.Log($"{gameObject.name} has been hit!");
            }
        }

    }


}
