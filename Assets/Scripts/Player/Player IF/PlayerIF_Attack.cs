using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.VFX;
using System.Runtime.CompilerServices;
using UnityEngine.Audio;

namespace ITKombat
{
    public class PlayerIFAttack : NetworkBehaviour
    {
         public static PlayerIFAttack Instance;
        public Transform attackPoint;
        public float attackRadius = 1f;
        public float attackCooldown = 0.5f;
        public float attackPower = 5f;
        public int maxCombo = 4;
        public LayerMask enemyLayer;
        private int combo = 0;
        private float timeSinceLastAttack;
        // Animator
        private Animator animator;

        // VFX Right
        [SerializeField] private ParticleSystem Attack1_Right = null;
        [SerializeField] private ParticleSystem Attack2_Right = null;
        [SerializeField] private ParticleSystem Attack3_Right = null;
        [SerializeField] private ParticleSystem Attack4_Right = null;

        // VFX Left
        [SerializeField] private ParticleSystem Attack1_Left = null;
        [SerializeField] private ParticleSystem Attack2_Left = null;
        [SerializeField] private ParticleSystem Attack3_Left = null;
        [SerializeField] private ParticleSystem Attack4_Left = null;

        // Weapon state
        public bool isUsingWeapon; // Buat toggle manual di masing-masing prefab karakter menggunakan weapon atau tidak

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

                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);
                // Debug.Log("Hit " + hitEnemies.Length + " enemies.");
                foreach (Collider2D enemy in hitEnemies)
                {
                    Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                    AI_Defense enemyDefense = enemy.GetComponent<AI_Defense>();
                    if (enemyRb != null && !enemyDefense.isBlocking)
                    {
                        GameObject enemyStateObject = GameObject.FindGameObjectWithTag("EnemyState");

                        if (enemyStateObject != null)
                        {
                            EnemyState enemyState = enemyStateObject.GetComponent<EnemyState>();
                            if (enemyState != null)
                            {
                                ApplyKnockback(enemy,combo);
                                enemyState.TakeDamage(attackPower,combo);
                            }
                        }

                        else
                        {
                            Debug.Log("EnemyState not found.");
                        }
                    }
                }
                AttackAnimation(hitEnemies);

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

        private void AttackAnimation(Collider2D[] hitEnemies)
        {
            CharacterController2D1 character = GetComponent<CharacterController2D1>();
            if (character == null) return;
            switch (combo)
            {
                case 1:
                    if (character.IsFacingRight)
                    {
                        Attack1_Right.Play();
                    }
                    else
                    {
                        Attack1_Left.Play();
                    }
                    PlayAttackSound(1, hitEnemies.Length > 0);
                    animator.SetTrigger("attack1");
                    StartCoroutine(ResetToIdleAfterTime(1f)); 
                    // Debug.Log("Attack 1 triggered");
                    break;
                case 2:
                    if (character.IsFacingRight)
                    {
                        Attack2_Right.Play();
                    }
                    else
                    {
                        Attack2_Left.Play();
                    }
                    PlayAttackSound(2, hitEnemies.Length > 0);
                    animator.SetTrigger("attack2");
                    StartCoroutine(ResetToIdleAfterTime(1f));
                    // Debug.Log("Attack 2 triggered");
                    break;
                case 3:
                    if (character.IsFacingRight)
                    {
                        Attack3_Right.Play();
                    }
                    else
                    {
                        Attack3_Left.Play();
                    }
                    PlayAttackSound(3, hitEnemies.Length > 0);
                    animator.SetTrigger("attack3");
                    StartCoroutine(ResetToIdleAfterTime(1f)); 
                    // Debug.Log("Attack 3 triggered");
                    break;
                case 4:
                    if (character.IsFacingRight)
                    {
                        Attack4_Right.Play();
                    }
                    else
                    {
                        Attack4_Left.Play();
                    }
                    PlayAttackSound(4, hitEnemies.Length > 0);
                    animator.SetTrigger("attack4");
                    StartCoroutine(ResetToIdleAfterTime(1f));
                    // Debug.Log("Attack 4 triggered");
                    break;
            }
        }

        private IEnumerator ResetToIdleAfterTime(float time)
        {
            yield return new WaitForSeconds(time); 
            animator.SetTrigger("Idle"); 
            // Debug.Log("Reset to Idle after " + time + " seconds.");
        }

        // Untuk determinasi apakah attacknya kena atau tidak

        private void PlayAttackSound(int comboNumber, bool hitEnemies)
        {
            if (hitEnemies)
            {
                PlayHitSound(comboNumber);
            }
            else
            {
                PlayMissSound(comboNumber);
            }
        }


        // Taruh hit dan miss soundnya disini
        
        private void PlayHitSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: NewSoundManager.Instance.PlaySound("IF_Attack1", transform.position); break;
                case 2: NewSoundManager.Instance.PlaySound("IF_Attack2", transform.position); break;
                case 3: NewSoundManager.Instance.PlaySound("IF_Attack3", transform.position); break;
                case 4: NewSoundManager.Instance.PlaySound("IF_Attack4", transform.position); break;
            }
        }

        private void PlayMissSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: NewSoundManager.Instance.PlaySound("Attack_Miss1", transform.position); break;
                case 2: NewSoundManager.Instance.PlaySound("Attack_Miss2", transform.position); break;
                case 3: NewSoundManager.Instance.PlaySound("Kick_Miss", transform.position); break;
                case 4: NewSoundManager.Instance.PlaySound("IF_Attack4", transform.position); break;
                
            }
        }


        private void OnDrawGizmosSelected()
        {
            if (attackPoint == null) return;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
