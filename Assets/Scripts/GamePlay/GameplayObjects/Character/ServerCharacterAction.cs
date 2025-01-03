using UnityEngine;
using System.Collections;
using Unity.Netcode;
namespace ITKombat
{
    public class ServerCharacterAction : NetworkBehaviour
    {
        
        public static ServerCharacterAction Instance;
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

        // VFX Rightgc

        // Nanti ini taruh audiosource buat masing-masing soundnya
        // AudioSource buat weapon hit
        // AudioSource buat weapon miss
        // AudioSource buat non-weapon hit
        // AudioSource buat non-weapon miss

        // Audio sources for normal attacks
        // public AudioSource punchSound1;
        // public AudioSource punchSound2;
        // public AudioSource punchSound3;
        // public AudioSource punchSound4;

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

        private void Update()
        {
            if (!IsOwner) return;
            TestingKey();
        }

        private void TestingKey()
        { 
            // Handle dash input (e.g., double-tap right or press a dash key)
            if (Input.GetKeyDown(KeyCode.B))
            {
                PerformAttack();
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
            Debug.Log("Performing attack...");
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
                Debug.Log("Combo: " + combo);

                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);
                Debug.Log("Hit " + hitEnemies.Length + " enemies.");
                foreach (Collider2D enemy in hitEnemies)
                {
                    Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                    AI_Defense enemyDefense = enemy.GetComponent<AI_Defense>();
                    if (enemyRb != null && !enemyDefense.isBlocking)
                    {
                        enemyRb.AddForce(transform.right * attackForce, ForceMode2D.Impulse);

                        GameObject enemyStateObject = GameObject.FindGameObjectWithTag("EnemyState");

                        if (enemyStateObject != null)
                        {
                            EnemyState enemyState = enemyStateObject.GetComponent<EnemyState>();
                            if (enemyState != null)
                            {
                                enemyState.TakeDamage(attackPower);
                            }
                        }

                        else
                        {
                            Debug.Log("EnemyState not found.");
                        }
                    }
                }
                AttackAnimation(hitEnemies);

                Debug.Log("Performed attack.");
            }
            else
            {
                animator.SetTrigger("Idle");
                Debug.Log("Cooldown not exceeded, going to idle.");
            }
        }

        private void AttackAnimation(Collider2D[] hitEnemies)
        {
            CharacterController2D1 character = GetComponent<CharacterController2D1>();
            if (character == null) return;
            switch (combo)
            {
                case 1:
                    // if (character.IsFacingRight)
                    // {
                    //     Attack1_Right.Play();
                    // }
                    // else
                    // {
                    //     Attack1_Left.Play();
                    // }
                    PlayAttackSound(1, hitEnemies.Length > 0);
                    animator.SetTrigger("attack1");
                    StartCoroutine(ResetToIdleAfterTime(1f)); 
                    Debug.Log("Attack 1 triggered");
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
                    PlayAttackSound(2, hitEnemies.Length > 0);
                    animator.SetTrigger("attack2");
                    StartCoroutine(ResetToIdleAfterTime(1f));
                    Debug.Log("Attack 2 triggered");
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
                    PlayAttackSound(3, hitEnemies.Length > 0);
                    animator.SetTrigger("attack3");
                    StartCoroutine(ResetToIdleAfterTime(1f)); 
                    Debug.Log("Attack 3 triggered");
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
                    PlayAttackSound(4, hitEnemies.Length > 0);
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
                case 1: SoundManager.Instance.PlaySound3D("CharIF_Attack1", transform.position); break;
                case 2: SoundManager.Instance.PlaySound3D("CharIF_Attack2", transform.position); break;
                case 3: SoundManager.Instance.PlaySound3D("CharIF_Attack3", transform.position); break;
                case 4: SoundManager.Instance.PlaySound3D("CharIF_Attack4", transform.position); break;
            }

            // if (isUsingWeapon)
            // {
            //     // Play weapon hit sound based on combo number
                
            // }
            // else
            // {
            //     // Play punch hit sound based on combo number
            //     switch (comboNumber)
            //     {
            //         case 1: punchSound1.Play(); break;
            //         case 2: punchSound2.Play(); break;
            //         case 3: punchSound3.Play(); break;
            //         case 4: punchSound4.Play(); break;
            //     }
            // }
        }

        private void PlayMissSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: SoundManager.Instance.PlaySound3D("AttackMiss_noWeapon", transform.position); break;
                case 2: SoundManager.Instance.PlaySound3D("AttackMiss_noWeapon", transform.position); break;
                case 3: SoundManager.Instance.PlaySound3D("AttackMiss_noWeapon", transform.position); break;
                case 4: SoundManager.Instance.PlaySound3D("AttackMiss_noWeapon", transform.position); break;
            }

            // if (isUsingWeapon)
            // {
            //     // Play weapon miss sound based on combo number
            // }
            // else
            // {
            //     // Play punch miss sound based on combo number
            //     switch (comboNumber)
            //     {
            //         case 1: punchMissSound1.Play(); break;
            //         case 2: punchMissSound2.Play(); break;
            //         case 3: punchMissSound3.Play(); break;
            //         case 4: punchMissSound4.Play(); break;
            //     }
            // }
        }


        private void OnDrawGizmosSelected()
        {
            if (attackPoint == null) return;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
