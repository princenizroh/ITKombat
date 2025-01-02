using System.Collections;
using UnityEngine;
using Unity.Netcode;

namespace ITKombat
{
    public class PlayerKelautan_Attack : NetworkBehaviour
    {
        public static PlayerKelautan_Attack Instance;
        public Transform attackPoint;
        public float attackRadius = 1.5f; // Jangkauan serangan Kelautan lebih luas
        public float attackCooldown = 1.0f; // Cooldown lebih lama untuk serangan berat
        public int maxCombo = 3; // Maksimal 3 combo
        public LayerMask enemyLayer;
        private int combo = 0;
        private float timeSinceLastAttack;

        private Animator animator;

        // VFX Right
        [SerializeField] private ParticleSystem Attack1_Right_Kelautan = null;
        [SerializeField] private ParticleSystem Attack2_Right_Kelautan = null;
        [SerializeField] private ParticleSystem Attack3_Right_Kelautan = null;
        [SerializeField] private ParticleSystem Attack4_Right_Kelautan = null;

        // VFX Left
        [SerializeField] private ParticleSystem Attack1_Left_Kelautan = null;
        [SerializeField] private ParticleSystem Attack2_Left_Kelautan = null;
        [SerializeField] private ParticleSystem Attack3_Left_Kelautan = null;
        [SerializeField] private ParticleSystem Attack4_Left_Kelautan = null;

        // Weapon state
        public bool isUsingWeapon; // Buat toggle manual di masing-masing prefab karakter menggunakan weapon atau tidak

        public CharacterStat characterStats;

        private void Awake()
        {
            animator = GetComponent<Animator>();

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }

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
            // Debug.Log("Performing attack...");
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
                // Debug.Log("Combo: " + combo);

                float attackPower = characterStats.characterBaseAtk;

                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);
                // Debug.Log("Hit " + hitEnemies.Length + " enemies.");
                bool isBlocked = false;

                foreach (Collider2D enemy in hitEnemies)
                {
                    Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                    AI_Defense enemyDefense = enemy.GetComponent<AI_Defense>();

                    if (enemyDefense != null && enemyDefense.isBlocking)
                    {
                        isBlocked = true; // Mark if at least one enemy is blocking
                    }

                    if (enemyRb != null && !enemyDefense.isBlocking)
                    {
                        GameObject enemyStateObject = GameObject.FindGameObjectWithTag("EnemyState");

                        if (enemyStateObject != null)
                        {
                            EnemyState enemyState = enemyStateObject.GetComponent<EnemyState>();
                            if (enemyState != null)
                            {
                                ApplyKnockback(enemy, combo);
                                // enemyState.TakeDamage(attackPower);
                            }
                        }
                        else
                        {
                            Debug.Log("EnemyState not found.");
                        }
                    }
                }
                // Pass the isBlocked state to AttackAnimation
                AttackAnimation(hitEnemies, isBlocked);
                // Debug.Log("Performed attack.");
            }
            else
            {
                animator.SetTrigger("Idle");
                // Debug.Log("Cooldown not exceeded, going to idle.");
            }
        }

        void ApplyKnockback(Collider2D enemyCollider, float currentCombo)
        {
            if (enemyCollider != null)
            {
                Rigidbody2D enemyRb = enemyCollider.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    if (currentCombo == 4) // knockback hanya untuk hit ke 4
                    {
                        float attackForce = enemyCollider.bounds.size.magnitude; // penyesuaian attack force sesuai size karakter
                        Vector2 direction = (enemyCollider.transform.position - attackPoint.position).normalized; // penyesuaian arah knockback

                        enemyRb.AddForce(direction * attackForce, ForceMode2D.Impulse);
                    }
                }
                else
                {
                    Debug.LogWarning("No Rigidbody2D found on the enemy.");
                }
            }
            else
            {
                Debug.LogWarning("No enemy detected within knockback radius.");
            }
        }

        private void AttackAnimation(Collider2D[] hitEnemies, bool isBlocked)
        {
            CharacterController2D1 character = GetComponent<CharacterController2D1>();
            if (character == null) return;

            switch (combo)
            {
                case 1:
                    if (character.IsFacingRight)
                    {
                        Attack1_Right_Kelautan.Play();
                    }
                    else
                    {
                        Attack1_Left_Kelautan.Play();
                    }
                    soundPlayerKelautan.Instance.PlayAttackSound(1, hitEnemies.Length > 0, isBlocked);
                    animator.SetTrigger("kelautan_attack1");
                    StartCoroutine(ResetToIdleAfterTime(1.2f));
                    break;
                case 2:
                    if (character.IsFacingRight)
                    {
                        Attack2_Right_Kelautan.Play();
                    }
                    else
                    {
                        Attack2_Left_Kelautan.Play();
                    }
                    soundPlayerKelautan.Instance.PlayAttackSound(2, hitEnemies.Length > 0, isBlocked);
                    animator.SetTrigger("kelautan_attack2");
                    StartCoroutine(ResetToIdleAfterTime(1.2f));
                    break;
                case 3:
                    if (character.IsFacingRight)
                    {
                        Attack3_Right_Kelautan.Play();
                    }
                    else
                    {
                        Attack3_Left_Kelautan.Play();
                    }
                    soundPlayerKelautan.Instance.PlayAttackSound(3, hitEnemies.Length > 0, isBlocked);
                    animator.SetTrigger("kelautan_attack3");
                    StartCoroutine(ResetToIdleAfterTime(1.2f));
                    break;
                case 4:
                    if (character.IsFacingRight)
                    {
                        Attack4_Right_Kelautan.Play();
                    }
                    else
                    {
                        Attack4_Left_Kelautan.Play();
                    }
                    soundPlayerKelautan.Instance.PlayAttackSound(4, hitEnemies.Length > 0, isBlocked);
                    animator.SetTrigger("mesin_attack4");
                    StartCoroutine(ResetToIdleAfterTime(1f));
                    break;
            }
        }

        private IEnumerator ResetToIdleAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            animator.SetTrigger("Idle");
        }

        private void OnDrawGizmosSelected()
        {
            if (attackPoint == null) return;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
