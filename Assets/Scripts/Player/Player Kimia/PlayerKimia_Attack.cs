using System.Collections;
using UnityEngine;
using Unity.Netcode;

namespace ITKombat
{
    public class PlayerKimia_Attack : NetworkBehaviour
    {
        public static PlayerKimia_Attack Instance;
        public Transform attackPoint;
        public float attackRadius = 1.2f; // Jangkauan sedang
        public float attackCooldown = 0.7f; // Cooldown lebih cepat dari Kelautan
        public float attackPower = 7f; // Kekuatan serangan sedang
        public int maxCombo = 4; // Maksimal 4 combo
        public LayerMask enemyLayer;
        private int combo = 0;
        private float timeSinceLastAttack;

        private Animator animator;

        // VFX Right
        [SerializeField] private ParticleSystem Attack1_Right_Kimia = null;
        [SerializeField] private ParticleSystem Attack2_Right_Kimia = null;
        [SerializeField] private ParticleSystem Attack3_Right_Kimia = null;
        [SerializeField] private ParticleSystem Attack4_Right_Kimia = null;

        // VFX Left
        [SerializeField] private ParticleSystem Attack1_Left_Kimia = null;
        [SerializeField] private ParticleSystem Attack2_Left_Kimia = null;
        [SerializeField] private ParticleSystem Attack3_Left_Kimia = null;
        [SerializeField] private ParticleSystem Attack4_Left_Kimia = null;

        // Weapon state
        public bool isUsingWeapon;

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
            if (Time.time - timeSinceLastAttack > attackCooldown)
            {
                if (combo == 0 || Time.time - timeSinceLastAttack > attackCooldown * 2)
                {
                    combo = 1;
                }
                else
                {
                    combo++;
                    if (combo > maxCombo)
                    {
                        combo = 1;
                    }
                }

                timeSinceLastAttack = Time.time;

                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);

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
                                enemyState.TakeDamage(attackPower, combo);
                            }
                        }
                        else
                        {
                            Debug.Log("EnemyState not found.");
                        }
                    }
                }
                AttackAnimation(hitEnemies, isBlocked);
            }
            else
            {
                animator.SetTrigger("Idle");
            }
        }

        void ApplyKnockback(Collider2D enemyCollider, float currentCombo)
        {
            if (enemyCollider != null)
            {
                Rigidbody2D enemyRb = enemyCollider.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    if (currentCombo == maxCombo) // Knockback hanya pada combo terakhir
                    {
                        float attackForce = 1.8f; // Kekuatan knockback menengah
                        Vector2 direction = (enemyCollider.transform.position - attackPoint.position).normalized;
                        enemyRb.AddForce(direction * attackForce, ForceMode2D.Impulse);
                    }
                }
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
                        Attack1_Right_Kimia.Play();
                    }
                    else
                    {
                        Attack1_Left_Kimia.Play();
                    }
                    PlayAttackSound(1, hitEnemies.Length > 0, isBlocked);
                    animator.SetTrigger("kimia_attack1");
                    StartCoroutine(ResetToIdleAfterTime(1f));
                    break;
                case 2:
                    if (character.IsFacingRight)
                    {
                        Attack2_Right_Kimia.Play();
                    }
                    else
                    {
                        Attack2_Left_Kimia.Play();
                    }
                    PlayAttackSound(2, hitEnemies.Length > 0, isBlocked);
                    animator.SetTrigger("kimia_attack2");
                    StartCoroutine(ResetToIdleAfterTime(1f));
                    break;
                case 3:
                    if (character.IsFacingRight)
                    {
                        Attack3_Right_Kimia.Play();
                    }
                    else
                    {
                        Attack3_Left_Kimia.Play();
                    }
                    PlayAttackSound(3, hitEnemies.Length > 0, isBlocked);
                    animator.SetTrigger("kimia_attack3");
                    StartCoroutine(ResetToIdleAfterTime(1f));
                    break;
                case 4:
                    if (character.IsFacingRight)
                    {
                        Attack4_Right_Kimia.Play();
                    }
                    else
                    {
                        Attack4_Left_Kimia.Play();
                    }
                    PlayAttackSound(4, hitEnemies.Length > 0, isBlocked);
                    animator.SetTrigger("kimia_attack4");
                    StartCoroutine(ResetToIdleAfterTime(1f));
                    break;
            }
        }

        private IEnumerator ResetToIdleAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            animator.SetTrigger("Idle");
        }

        private void PlayAttackSound(int comboNumber, bool hitEnemies, bool isBlocked)
        {
            if (isBlocked)
            {
                PlayBlockedSound(comboNumber);
            }
            else if (hitEnemies)
            {
                PlayHitSound(comboNumber);
            }
            else
            {
                PlayMissSound(comboNumber);
            }
        }

        private void PlayHitSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: NewSoundManager.Instance.PlaySound("Kimia_Attack1", transform.position); break;
                case 2: NewSoundManager.Instance.PlaySound("Kimia_Attack2", transform.position); break;
                case 3: NewSoundManager.Instance.PlaySound("Kimia_Attack3", transform.position); break;
                case 4: NewSoundManager.Instance.PlaySound("Kimia_Attack4", transform.position); break;
            }
        }

        private void PlayMissSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: NewSoundManager.Instance.PlaySound("Attack_Miss1", transform.position); break;
                case 2: NewSoundManager.Instance.PlaySound("Attack_Miss2", transform.position); break;
                case 3: NewSoundManager.Instance.PlaySound("Attack_Miss1", transform.position); break;
                case 4: NewSoundManager.Instance.PlaySound("Attack_Miss_BluntWeapon1", transform.position); break;
            }
        }

        private void PlayBlockedSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: NewSoundManager.Instance.PlaySound("Kimia_Blocked1", transform.position); break;
                case 2: NewSoundManager.Instance.PlaySound("Kimia_Blocked2", transform.position); break;
                case 3: NewSoundManager.Instance.PlaySound("Kimia_Blocked3", transform.position); break;
                case 4: NewSoundManager.Instance.PlaySound("Kimia_Blocked4", transform.position); break;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (attackPoint == null) return;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
