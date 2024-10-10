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
        private float timeSinceLastAttack;
        private bool isCrouching = false;

        // Animator
        private Animator animator;

        // Audio sources for normal attacks
        public AudioSource punchSound1;
        public AudioSource punchSound2;
        public AudioSource punchSound3;
        public AudioSource punchSound4;

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

        public void StartCrouch()
        {
            if (animator != null && !isCrouching)
            {
                animator.SetTrigger("Crouch");
                isCrouching = true;
                Debug.Log("Player started crouching.");
            }
        }

        public void StopCrouch()
        {
            if (animator != null && isCrouching)
            {
                isCrouching = false;
                animator.SetTrigger("Idle");
                Debug.Log("Player stopped crouching.");
            }
        }

        public bool IsCrouching()
        {
            return isCrouching;
        }

        public void PerformAttack()
        {
            // Check if cooldown is exceeded
            if (Time.time - timeSinceLastAttack > attackCooldown)
            {
                // Reset combo if cooldown has passed
                combo++;
                if (combo > maxCombo)
                {
                    combo = 1; // Reset to the first attack if it exceeds maxCombo
                }

                timeSinceLastAttack = Time.time; // Update the time of the last attack

                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);
                foreach (Collider2D enemy in hitEnemies)
                {
                    Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                    if (enemyRb != null)
                    {
                        enemyRb.AddForce(transform.right * attackForce, ForceMode2D.Impulse);
                    }
                }

                // Call the attack animation based on the current combo
                AttackAnimation();

                Debug.Log("Performed attack.");
            }
            else
            {
                // If cooldown hasn't passed, go idle
                animator.SetTrigger("Idle");
                Debug.Log("Cooldown not exceeded, going to idle.");
            }
        }

        private void AttackAnimation()
        {
            switch (combo)
            {
                case 1:
                    PlaySound(punchSound1);
                    animator.SetTrigger("attack1");
                    StartCoroutine(ResetToIdleAfterTime(1f)); // Durasi 1 detik untuk animasi attack 1
                    Debug.Log("Attack 1 triggered");
                    break;
                case 2:
                    PlaySound(punchSound2);
                    animator.SetTrigger("attack2");
                    StartCoroutine(ResetToIdleAfterTime(1f)); // Durasi 1 detik untuk animasi attack 2
                    Debug.Log("Attack 2 triggered");
                    break;
                case 3:
                    PlaySound(punchSound3);
                    animator.SetTrigger("attack3");
                    StartCoroutine(ResetToIdleAfterTime(1f)); // Durasi 1 detik untuk animasi attack 3
                    Debug.Log("Attack 3 triggered");
                    break;
                case 4:
                    PlaySound(punchSound4);
                    animator.SetTrigger("attack4");
                    StartCoroutine(ResetToIdleAfterTime(1f)); // Durasi 1 detik untuk animasi attack 4
                    Debug.Log("Attack 4 triggered");
                    break;
            }
        }

        private IEnumerator ResetToIdleAfterTime(float time)
        {
            yield return new WaitForSeconds(time); // Tunggu selama waktu yang ditentukan
            animator.SetTrigger("Idle"); // Kembali ke idle
            Debug.Log("Reset to Idle after " + time + " seconds.");
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
}
