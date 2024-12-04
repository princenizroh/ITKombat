using System.Collections;
using UnityEngine;
using Unity.Netcode;

namespace ITKombat
{
    public class PlayerFisika_Attack : NetworkBehaviour
    {
        public static PlayerFisika_Attack Instance;
        public Transform attackPoint;
        public float attackRadius = 1.2f; // Jangkauan serangan Fisika
        public float attackCooldown = 0.6f; // Cooldown lebih cepat
        public float attackPower = 8f; // Kekuatan serangan Fisika
        public int maxCombo = 3; // Fisika hanya memiliki 3 combo
        public LayerMask enemyLayer;
        private int combo = 0;
        private float timeSinceLastAttack;

        private Animator animator;

        // VFX Right
        [SerializeField] private ParticleSystem Attack1_Right_Fisika = null;
        [SerializeField] private ParticleSystem Attack2_Right_Fisika = null;
        [SerializeField] private ParticleSystem Attack3_Right_Fisika = null;

        // VFX Left
        [SerializeField] private ParticleSystem Attack1_Left_Fisika = null;
        [SerializeField] private ParticleSystem Attack2_Left_Fisika = null;
        [SerializeField] private ParticleSystem Attack3_Left_Fisika = null;

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
                AttackAnimation(hitEnemies);
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
                        float attackForce = 2.0f; // Kekuatan knockback Fisika
                        Vector2 direction = (enemyCollider.transform.position - attackPoint.position).normalized;
                        enemyRb.AddForce(direction * attackForce, ForceMode2D.Impulse);
                    }
                }
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
                        Attack1_Right_Fisika.Play();
                    }
                    else
                    {
                        Attack1_Left_Fisika.Play();
                    }
                    PlayAttackSound(1, hitEnemies.Length > 0);
                    animator.SetTrigger("fisika_attack1");
                    StartCoroutine(ResetToIdleAfterTime(0.8f));
                    break;
                case 2:
                    if (character.IsFacingRight)
                    {
                        Attack2_Right_Fisika.Play();
                    }
                    else
                    {
                        Attack2_Left_Fisika.Play();
                    }
                    PlayAttackSound(2, hitEnemies.Length > 0);
                    animator.SetTrigger("fisika_attack2");
                    StartCoroutine(ResetToIdleAfterTime(0.8f));
                    break;
                case 3:
                    if (character.IsFacingRight)
                    {
                        Attack3_Right_Fisika.Play();
                    }
                    else
                    {
                        Attack3_Left_Fisika.Play();
                    }
                    PlayAttackSound(3, hitEnemies.Length > 0);
                    animator.SetTrigger("fisika_attack3");
                    StartCoroutine(ResetToIdleAfterTime(0.8f));
                    break;
            }
        }

        private IEnumerator ResetToIdleAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            animator.SetTrigger("Idle");
        }

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

        private void PlayHitSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: NewSoundManager.Instance.PlaySound("Fisika_Hit1", transform.position); break;
                case 2: NewSoundManager.Instance.PlaySound("Fisika_Hit2", transform.position); break;
                case 3: NewSoundManager.Instance.PlaySound("Fisika_Hit3", transform.position); break;
            }
        }

        private void PlayMissSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: NewSoundManager.Instance.PlaySound("Fisika_Miss1", transform.position); break;
                case 2: NewSoundManager.Instance.PlaySound("Fisika_Miss2", transform.position); break;
                case 3: NewSoundManager.Instance.PlaySound("Fisika_Miss3", transform.position); break;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (attackPoint == null) return;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
