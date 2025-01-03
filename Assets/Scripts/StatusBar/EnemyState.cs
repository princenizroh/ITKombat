using UnityEngine;
using System.Collections;

namespace ITKombat
{
    public class EnemyState : MonoBehaviour
    {
        public static EnemyState Instance;
        public float maxHealth = 100f;
        public float currentHealth;
        public string attackTag = "Attack";
        public HealthBar healthBar;
        public MatchManager matchManager;

        public AudioSource[] hitAudioSources;
        public string[] hitAnimationTriggers = { "Hit1", "Hit2", "Hit3" };
        public string idleAnimationTrigger = "Idle";
        public Rigidbody2D rb;
        public float knockbackForce = 1f;
        public bool checkDamage = false;

        public bool canAttack;

        private void Awake()
        {
            Instance = this;

        }


        private void Start()
        {
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);

            // Dapatkan referensi ke MatchManager
            if (matchManager == null)
            {
                matchManager = MatchManager.Instance;
            }
        }

        public bool CheckDamage()
        {
            return checkDamage;
        }

        public void TakeDamage(float damage)
        {   
            currentHealth -= damage;
            healthBar.UpdateHealth(currentHealth, maxHealth);
            
            if (currentHealth <= 0)
            {
                HandleDeath();
            }
            else
            {
                checkDamage = true;
                canAttack = false;
                AI_Attack.Instance.GetCanAttack(canAttack);
                Debug.Log("Check Damage" + checkDamage);

                Debug.Log("Memanggil OnEnemyDamaged");
                AIDamageChecker.Instance.OnEnemyDamaged();
                StartCoroutine(ResetCheckDamage()); // Atur ulang ke false setelah durasi tertentu.
                StartCoroutine(ResetAICanAttack());
                // ApplyKnockback();
                // AttackedAnimation(combo);
                // PlayRandomHitSound();
            
            }
            checkDamage = false; //Hapus kalau mau langsung 4
        }

        private IEnumerator ResetCheckDamage()
        {
            yield return new WaitForSeconds(0.1f); // Sesuaikan durasi ini sesuai kebutuhan.
            checkDamage = false;
            Debug.Log("Pemain tidak lagi menerima serangan.");
        }

        private IEnumerator ResetAICanAttack()
        {
            Debug.Log("Tunggu 5 Detik");

            yield return new WaitForSeconds(10f); // Sesuaikan durasi 
            Debug.Log("Sudah 5 Detik");
            canAttack = true;
            AI_Attack.Instance.GetCanAttack(canAttack);
        }

        // public void HitAnimation()
        // {
        //     stateMachine.SetState(new LightReactionState());
        // }

        public void TakeDamageFromSkill(float skillDamage)
        {
            currentHealth -= skillDamage;
            healthBar.UpdateHealth(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                HandleDeath();
            }
            else
            {
                // ApplyKnockback();
                PlayRandomHitSound();
            }
        }

        private void HandleDeath()
        {
            Debug.Log("Player mati!");

            // Call the match manager to check the current state of the match
            if (matchManager != null)
            {
                matchManager.EnemyDied();
            }
            else
            {
                Debug.LogWarning("MatchManager not assigned in PlayerState!");
            }
        }

         private void AttackedAnimation(float combo)
        {
            // CharacterController2D1 character = GetComponent<CharacterController2D1>();
            // if (character == null) return;
            switch (combo)
            {
                case 1:
                    // if (character.IsFacingRight)
                    // {
                    //     //vfx disini
                    // }
                    // else
                    // {
                    //     //vfx disini
                    // }
                    //anim & sound disini
                    Debug.Log("Kena pukul sekali cik");
                    break;
                case 2:
                    // if (character.IsFacingRight)
                    // {
                    //     //vfx disini
                    // }
                    // else
                    // {
                    //     //vfx disini
                    // }
                    //anim & sound disini
                    Debug.Log("Kena pukul 2x cik");
                    break;
                case 3:
                    // if (character.IsFacingRight)
                    // {
                    //     //vfx disini
                    // }
                    // else
                    // {
                    //     //vfx disini
                    // }
                    //anim & sound disini
                    Debug.Log("Kena pukul 3x cik");
                    break;
                case 4:
                    // if (character.IsFacingRight)
                    // {
                    //     //vfx disini
                    // }
                    // else
                    // {
                    //     //vfx disini
                    // }
                    //anim & sound disini
                    Debug.Log("Kena pukul 4x cik");
                    break;
            }
        }


        // [System.Obsolete]
        // private void OnTriggerEnter2D(Collider2D collision)
        // {
        //     if (collision.CompareTag(attackTag))
        //     {
        //         Debug.Log("Player attacked by: " + collision.gameObject.name);
        //         float attackPower = GetDamageFromPlayer();
        //         Debug.Log("Player attacked power: " + attackPower);
        //         TakeDamage(attackPower); 
        //     }
        // }

        // [System.Obsolete]
        // public float GetDamageFromPlayer()
        // {
        //     PlayerIFAttack playerAttack = FindObjectOfType<PlayerIFAttack>(); // Mengambil dari objek pemain secara global
        //     if (playerAttack != null)
        //     {
        //         float damage = playerAttack.attackPower;
        //         if (damage > 0)
        //         {
        //             return damage; // Pastikan nilai lebih dari 0
        //         }
        //     }
        //     return 0f;
        // }

        // private void EndGame()
        // {
        //     Debug.Log("Game Berakhir");
        //     // Implementasikan logika end game, bisa panggil UI atau lainnya.
        // }

        // private void Die()
        // {
        //     Debug.Log("Enemy mati!");
        //     // Implementasi logika kematian enemy
        // }

        // private void ApplyKnockback()
        // {
        //     Vector2 knockbackDirection = (transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)).normalized;
        //     rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        // }

        // private IEnumerator PlayRandomHitAnimation()
        // {
        //     string randomHitAnimation = hitAnimationTriggers[Random.Range(0, hitAnimationTriggers.Length)];
        //     enemyAnimator.SetTrigger(randomHitAnimation);
        //     yield return new WaitForSeconds(0.5f);
        //     enemyAnimator.SetTrigger(idleAnimationTrigger);
        // }

        private void PlayRandomHitSound()
        {
            if (hitAudioSources.Length > 0)
            {
                AudioSource randomAudioSource = hitAudioSources[Random.Range(0, hitAudioSources.Length)];
                randomAudioSource.Play();
            }
        }

        // Reset health to maxHealth
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            healthBar.UpdateHealth(currentHealth,maxHealth);
            Debug.Log("currentHealth " + currentHealth);
        }
    }
}
