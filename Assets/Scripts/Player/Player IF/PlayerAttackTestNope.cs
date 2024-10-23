using System.Collections;
using UnityEngine;
using Unity.Netcode;

namespace ITKombat
{
    public class PlayerAttackTestNope : NetworkBehaviour
    {
        public Transform attackPoint;
        public float attackForce = 5f;
        public float attackRadius = 1f;
        public float attackCooldown = 0.5f;
        public float attackPower = 25f;
        public int maxCombo = 4;
        public LayerMask enemyLayer;
        private int combo = 0;
        private float timeSinceLastAttack;

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

        public void PerformAttack()
        {
            if (Time.time - timeSinceLastAttack > attackCooldown)
            {
                // Jika cooldown terlampaui sebelum serangan berikutnya, reset combo ke 1
                if (combo == 0 || Time.time - timeSinceLastAttack > attackCooldown * 2)
                {
                    combo = 1; // Reset ke serangan pertama jika waktu terlalu lama
                }
                else
                {
                    combo++; // Lanjutkan ke serangan berikutnya jika waktu masih dalam cooldown
                    if (combo > maxCombo)
                    {
                        combo = 1; // Kembali ke serangan pertama jika melebihi maxCombo
                    }
                }

                timeSinceLastAttack = Time.time; // Simpan waktu serangan terakhir

                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);
                foreach (Collider2D enemy in hitEnemies)
                {
                    Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                    AI_Defense enemyDefense = enemy.GetComponent<AI_Defense>();
                    if (enemyRb != null && !enemyDefense.isBlocking)
                    {
                        enemyRb.AddForce(transform.right * attackForce, ForceMode2D.Impulse);
                        HealthBarTest enemyHealth = enemy.GetComponent<HealthBarTest>();
                        if (enemyHealth != null)
                        {
                            enemyHealth.TakeDamage(attackPower);
                        }
                    }
                }
                AttackAnimation();

                Debug.Log("Performed attack.");
            }
            else
            {
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
                    StartCoroutine(ResetToIdleAfterTime(1f)); 
                    Debug.Log("Attack 1 triggered");
                    break;
                case 2:
                    PlaySound(punchSound2);
                    animator.SetTrigger("attack2");
                    StartCoroutine(ResetToIdleAfterTime(1f));
                    Debug.Log("Attack 2 triggered");
                    break;
                case 3:
                    PlaySound(punchSound3);
                    animator.SetTrigger("attack3");
                    StartCoroutine(ResetToIdleAfterTime(1f)); 
                    Debug.Log("Attack 3 triggered");
                    break;
                case 4:
                    PlaySound(punchSound4);
                    animator.SetTrigger("attack4");
                    StartCoroutine(ResetToIdleAfterTime(1f));
                    Debug.Log("Attack 4 triggered");
                    break;
            }
        }

        private IEnumerator ResetToIdleAfterTime(float time)
        {
            yield return new WaitForSeconds(time); 
            animator.SetTrigger("Idle"); 
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
