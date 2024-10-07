using System.Collections;
using UnityEngine;
using Unity.Netcode;

namespace ITKombat
{
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
        private bool canCrouch = true; // Flag to control crouch ability

        // Animator
        private Animator animator;

        public AudioSource punchSound1;
        public AudioSource punchSound2;
        public AudioSource punchSound3;
        public AudioSource punchSound4;

        // Audio source for crouch attack
        public AudioSource crouchAttackSound;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void OnAttackButtonPressed()
        {
            if (IsOwner)
            {
                PerformAttack();
            }
        }

        public void OnCrouchButtonPressed()
        {
            if (IsOwner && canCrouch) // Only allow crouching if permitted
            {
                ToggleCrouch();
            }
        }

        public void ToggleCrouch()
        {
            if (!isCrouching)
            {
                StartCrouch(); // Start crouch
            }
            else
            {
                StopCrouch(); // Stop crouch
            }
        }

        private void StartCrouch()
        {
            isCrouching = true;
            animator.SetTrigger("Crouch");
            Debug.Log("Player started crouching.");
        }

        public void StopCrouch()
        {
            isCrouching = false;
            canCrouch = false;
            animator.SetTrigger("Idle"); 
            Debug.Log("Player stopped crouching and transitioned to Idle.");

            StartCoroutine(WaitForCrouchButtonPress());
        }

        private IEnumerator WaitForCrouchButtonPress()
        {
            while (!canCrouch)
            {
                yield return null; // Wait for next frame
            }
        }

        public void PerformAttack()
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
            TriggerAttackAnimation();
        }

        public void PerformCrouchAttack()
        {
            if (isCrouching)
            {
                Debug.Log("Crouch Attack triggered!");
                PlaySound(crouchAttackSound);
                animator.SetTrigger("CrouchAttack");

                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);
                foreach (Collider2D enemy in hitEnemies)
                {
                    Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                    if (enemyRb != null)
                    {
                        enemyRb.AddForce(transform.right * attackForce, ForceMode2D.Impulse);
                    }
                }
                StartCoroutine(ReturnToCrouch());
            }
        }

        private IEnumerator ReturnToCrouch()
        {
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            StartCrouch();
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

        public bool IsCrouching()
        {
            return isCrouching;
        }
    }
}
