using UnityEngine;
using System.Collections;

namespace ITKombat
{
    public class EnemyState : MonoBehaviour
    {
        public float maxHealth = 100f;
        public float currentHealth;
        public string attackTag = "Attack";
        public HealthBar healthBar;
        private Animator enemyAnimator;
        public AudioSource[] hitAudioSources;
        public string[] hitAnimationTriggers = { "Hit1", "Hit2", "Hit3" };
        public string idleAnimationTrigger = "Idle";
        public Rigidbody2D rb;
        public float knockbackForce = 1f;

        private int currentRound = 1;

        private void Start()
        {
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
            Debug.Log("Enemy health: " + currentHealth);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                float attackPower = GetDamageFromPlayer();
                if (attackPower > 0)
                {
                    TakeDamage(attackPower);
                    Debug.Log("Player attacked with power: " + attackPower);
                    Debug.Log(healthBar.healthSlider.value);
                }
                else
                {
                    Debug.LogWarning("Attack power is zero or negative!");
                }
            }
        }

        public void TakeDamage(float damage)
        {
            Debug.Log("TakeDamage called. Current health: " + currentHealth + ", Damage: " + damage);
            currentHealth -= damage;
            healthBar.UpdateHealth(currentHealth, maxHealth);
            Debug.Log($"Enemy taking {damage} damage. Current health: {currentHealth}");

            if (currentHealth <= 0)
            {
                Debug.Log("Player mati!");
                if (currentRound == 1)
                {
                    Debug.Log("Round 1 berakhir");
                    currentRound++;
                    // StartNewRound();
                    MatchManager.Instance.PlayerVictory();
                }
                else
                {
                    Debug.Log("Game berakhir");
                    EndGame();
                }
            }
            else
            {
                ApplyKnockback();
                // StartCoroutine(PlayRandomHitAnimation());
                PlayRandomHitSound();
            }
        }

        private void StartNewRound()
        {
            currentHealth = maxHealth;
            healthBar.UpdateHealth(currentHealth, maxHealth);
            MatchManager.Instance.ShowRoundStartNotification(currentRound);
            

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(attackTag))
            {
                Debug.Log("Player attacked by: " + collision.gameObject.name);
                float attackPower = GetDamageFromPlayer();
                Debug.Log("Player attacked power: " + attackPower);
                TakeDamage(attackPower); 
            }
        }

        public float GetDamageFromPlayer()
        {
            PlayerAttackTestNope playerAttack = FindObjectOfType<PlayerAttackTestNope>(); // Mengambil dari objek pemain secara global
            if (playerAttack != null)
            {
                float damage = playerAttack.attackPower;
                if (damage > 0)
                {
                    return damage; // Pastikan nilai lebih dari 0
                }
            }
            return 0f;
        }

        private void EndGame()
        {
            Debug.Log("Game Berakhir");
            // Implementasikan logika end game, bisa panggil UI atau lainnya.
        }

        private void Die()
        {
            Debug.Log("Enemy mati!");
            // Implementasi logika kematian enemy
        }

        private void ApplyKnockback()
        {
            Vector2 knockbackDirection = (transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)).normalized;
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }

        private IEnumerator PlayRandomHitAnimation()
        {
            string randomHitAnimation = hitAnimationTriggers[Random.Range(0, hitAnimationTriggers.Length)];
            enemyAnimator.SetTrigger(randomHitAnimation);
            yield return new WaitForSeconds(0.5f);
            enemyAnimator.SetTrigger(idleAnimationTrigger);
        }

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
        }
    }
}
