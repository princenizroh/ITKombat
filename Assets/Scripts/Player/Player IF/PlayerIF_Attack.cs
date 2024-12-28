using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.VFX;
using System.Runtime.CompilerServices;
using UnityEngine.Audio;
using Mono.CSharp;

namespace ITKombat
{
    [RequireComponent(typeof(FelixStateMachine))]
    public class PlayerIFAttack : NetworkBehaviour 
    {
        private FelixStateMachine meleeStateMachine;

        public static PlayerIFAttack Instance;
        public Transform attackPoint;
        public float attackRadius = 0.15f;
        public float attackCooldown = 0.5f;
        public int maxCombo = 4;
        public LayerMask enemyLayer;
        public int combo = 0;
        private float timeSinceLastAttack;

        private bool canAttack = true;
        // Animator
        private Animator animator;

        [SerializeField] public Collider2D hitbox;
        [SerializeField] public GameObject Hiteffect;

        // VFX Right
        // [SerializeField] private ParticleSystem Attack1_Right = null;
        // [SerializeField] private ParticleSystem Attack2_Right = null;
        // [SerializeField] private ParticleSystem Attack3_Right = null;
        // [SerializeField] private ParticleSystem Attack4_Right = null;

        // VFX Left
        // [SerializeField] private ParticleSystem Attack1_Left = null;
        // [SerializeField] private ParticleSystem Attack2_Left = null;
        // [SerializeField] private ParticleSystem Attack3_Left = null;
        // [SerializeField] private ParticleSystem Attack4_Left = null;

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

        private void Start()
        {
            if (characterStats == null)
            {
                characterStats = GetComponent<CharacterStat>();
                if (characterStats == null)
                {
                    Debug.LogError("CharacterStat component is missing from this GameObject!");
                }
            }
        
            meleeStateMachine = GetComponent<FelixStateMachine>();
        }

    // Update is called once per frame
        public void OnButtonDown()
        {
        if (canAttack)
        {
            if (meleeStateMachine == null)
            {
                // Debug.LogError("FelixStateMachine is null! Ensure it's added to the GameObject.");
                return;
            }

            if (meleeStateMachine.CurrentState == null)
            {
                // Debug.LogError("CurrentState is null! Check the state initialization in FelixStateMachine.");
                return;
            }

            if (meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
            {
                meleeStateMachine.SetNextState(new GroundEntryState());
            }
        }
            

        }

        public bool GetCanAttack (bool CanAttack)
        {
            canAttack = CanAttack;
            // Debug.Log("Berhasil Get Player Can Attack = " + CanAttack);
            return canAttack;
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
                    PlayerDefense enemyPlayerDefense = enemy.GetComponent<PlayerDefense>();

                    if ((enemyDefense != null && enemyDefense.isBlocking) || (enemyPlayerDefense != null && enemyPlayerDefense.isBlocking))
                    {
                        isBlocked = true; // Mark if at least one enemy is blocking
                    }

                    if ((enemyRb != null && !enemyDefense.isBlocking) || (enemyRb != null && !enemyPlayerDefense.isBlocking))
                    {
                        GameObject enemyStateObject = GameObject.FindGameObjectWithTag("EnemyState");

                        if (enemyStateObject != null)
                        {
                            EnemyState enemyState = enemyStateObject.GetComponent<EnemyState>();
                            if (enemyState != null)
                            {
                                ApplyKnockback(enemy,combo);
                                enemyState.TakeDamage(attackPower);
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

        public void ApplyKnockback(Collider2D enemyCollider, float currentCombo)
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
                    // Debug.LogWarning("No Rigidbody2D found on the enemy.");
                }
            }
            else
            {
                // Debug.LogWarning("No enemy detected within knockback radius.");
            }
        }

        private void AttackAnimation(Collider2D[] hitEnemies, bool isBlocked)
        {
            switch (combo)
            {
                case 1:
                    // if (character.IsFacingRight)
                    // {
                    //     // Attack1_Right.Play();
                    // }
                    // else
                    // {
                    //     // Attack1_Left.Play();
                    // }
                    PlayAttackSound(1, hitEnemies.Length > 0, isBlocked);
                    animator.SetTrigger("Attack1");
                    StartCoroutine(ResetToIdleAfterTime(1f)); 
                    // Debug.Log("Attack 1 triggered");
                    break;
                case 2:
                    // if (character.IsFacingRight)
                    // {
                    //     Attack2_Right.Play();
                    // }
                    // else
                    // {
                    //     Attack2_Left.Play();
                    // }
                    PlayAttackSound(2, hitEnemies.Length > 0, isBlocked);
                    animator.SetTrigger("Attack2");
                    StartCoroutine(ResetToIdleAfterTime(1f));
                    // Debug.Log("Attack 2 triggered");
                    break;
                case 3:
                    // if (character.IsFacingRight)
                    // {
                    //     Attack3_Right.Play();
                    // }
                    // else
                    // {
                    //     Attack3_Left.Play();
                    // }
                    PlayAttackSound(3, hitEnemies.Length > 0, isBlocked);
                    animator.SetTrigger("Attack3");
                    StartCoroutine(ResetToIdleAfterTime(1f)); 
                    // Debug.Log("Attack 3 triggered");
                    break;
                case 4:
                    // if (character.IsFacingRight)
                    // {
                    //     Attack4_Right.Play();
                    // }
                    // else
                    // {
                    //     Attack4_Left.Play();
                    // }
                    PlayAttackSound(4, hitEnemies.Length > 0, isBlocked);
                    animator.SetTrigger("Attack4");
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

        public void PlayAttackSound(int comboNumber, bool hitEnemies, bool isBlocked)
        {
            if (isBlocked == true)
            {
                PlayBlockedSound(comboNumber);
                return;
            }
        
            if (hitEnemies)
            {
                PlayHitSound(comboNumber);
                return;
            }
            PlayMissSound(comboNumber);
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

        private void PlayBlockedSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
                case 2: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
                case 3: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
                case 4: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
            }
        }


        private void OnDrawGizmosSelected()
        {
            if (attackPoint == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
