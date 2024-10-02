using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerAttackTestNope : NetworkBehaviour
{
    public Transform attackPoint;
    public float attackForce = 10f;
    public float attackRadius = 1f;
    public float attackCooldown = 0.5f; 
    public int maxCombo = 4;
    public LayerMask enemyLayer;

    private int combo = 0;
    public float cooldown = 0.5f; 
    private float timeSinceLastAttack;

    // Crouch state
    private bool isCrouching = false;
    private bool isCrouchInitiated = false;

    // Animator
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (Time.time - timeSinceLastAttack > cooldown)
        {
            combo = 0;
        }

        HandleMovementInput();
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }
    public void StartCrouch()
    {
        if (!isCrouching)
        {
            isCrouching = true;
            isCrouchInitiated = true;
            animator.SetTrigger("startCrouch"); 
            Debug.Log("Player started crouching.");
        }
    }

    public void StopCrouch()
    {
        if (isCrouching)
        {
            isCrouching = false;
            isCrouchInitiated = false;
            animator.SetBool("isCrouching", false); // Hentikan animasi crouch dan kembali ke idle
            animator.SetTrigger("standUp"); // Memicu animasi berdiri
            Debug.Log("Player stopped crouching.");
        }
    }

    public void ContinueCrouch()
    {
        if (isCrouchInitiated)
        {
            animator.SetBool("isCrouching", true); // Bertahan dalam posisi crouch
        }
    }

    public void PerformAttack()
    {
        if (!isCrouching) // debug
        {
            if (Time.time - timeSinceLastAttack <= cooldown && combo < maxCombo)
            {
                combo++;
            }
            else
            {
                combo = 1; 
            }

            timeSinceLastAttack = Time.time;

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);
            foreach (Collider2D enemy in hitEnemies)
            {
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyRb.AddForce(transform.right * attackForce, ForceMode2D.Impulse);
                }
            }

            Debug.Log("Player performed attack " + combo);
            TriggerAttackAnimation();
        }

    }

    public void PerformCrouchAttack()
    {
        if (isCrouching)
        {
            Debug.Log("Crouch Attack triggered!");
            animator.SetTrigger("crouchAttack");

            // Lakukan logika crouch attack
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);
            foreach (Collider2D enemy in hitEnemies)
            {
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyRb.AddForce(transform.right * attackForce, ForceMode2D.Impulse);
                }
            }
        }
        else
        {
            Debug.Log("Attack berdiri");
        }
    }

    private void HandleMovementInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        animator.SetFloat("moveSpeed", Mathf.Abs(moveHorizontal)); 

        Vector2 movement = new Vector2(moveHorizontal, 0);
        transform.Translate(movement * Time.deltaTime);
    }

    private void TriggerAttackAnimation()
    {
        switch (combo)
        {
            case 1:
                Debug.Log("Attack 1 triggered");
                animator.SetTrigger("attack1");
                break;
            case 2:
                Debug.Log("Attack 2 triggered");
                animator.SetTrigger("attack2");
                break;
            case 3:
                Debug.Log("Attack 3 triggered");
                animator.SetTrigger("attack3");
                break;
            case 4:
                Debug.Log("Attack 4 triggered");
                animator.SetTrigger("attack4");
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
