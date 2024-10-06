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

    // Audio sources for punch sounds
    public AudioSource punchSound1;
    public AudioSource punchSound2;
    public AudioSource punchSound3;
    public AudioSource punchSound4;

    // Audio sources for crouch and crouch attack
    public AudioSource crouchSound;
    public AudioSource crouchAttackSound;

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
            PlaySound(crouchSound);  // Play crouch sound
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
            animator.SetBool("isCrouching", false);
            animator.SetTrigger("standUp"); 
            Debug.Log("Player stopped crouching.");
        }
    }

    public void ContinueCrouch()
    {
        if (isCrouchInitiated)
        {
            animator.SetBool("isCrouching", true); 
        }
    }

    public void PerformAttack()
    {
        if (!isCrouching)
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
            PlaySound(crouchAttackSound);  // Play crouch attack sound
            animator.SetTrigger("crouchAttack");

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
                PlaySound(punchSound1);
                animator.SetTrigger("attack1");
                break;
            case 2:
                Debug.Log("Attack 2 triggered");
                PlaySound(punchSound2);
                animator.SetTrigger("attack2");
                break;
            case 3:
                Debug.Log("Attack 3 triggered");
                PlaySound(punchSound3);
                animator.SetTrigger("attack3");
                break;
            case 4:
                Debug.Log("Attack 4 triggered");
                PlaySound(punchSound4);
                animator.SetTrigger("attack4");
                break;
        }
    }

    private void PlaySound(AudioSource sound)
    {
        if (sound != null)
        {
            sound.Play();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
