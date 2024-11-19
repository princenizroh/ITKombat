using UnityEngine;
using System.Collections;

namespace ITKombat
{
    public class PlayerState : MonoBehaviour
    {
        public static PlayerState Instance;
        public float maxHealth = 100f;
        public float currentHealth;
        public string attackTag = "Attack"; 
        public HealthBar healthBar;
        public MatchManager matchManager;
        private Animator playerAnimator;
        public AudioSource[] hitAudioSources;
        public string[] hitAnimationTriggers = { "Hit1", "Hit2", "Hit3" };
        public string idleAnimationTrigger = "Idle";
        public Rigidbody2D rb;
        public float knockbackForce = 1f;

        private void Awake()
        {
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
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);

            // Dapatkan referensi ke MatchManager
            if (matchManager == null)
            {
                matchManager = MatchManager.Instance;
            }
        }

        private void Update()
        {
            float attackPower = GetDamageFromPlayer();
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
                ApplyKnockback();
                PlayRandomHitSound();
            }
        }

        private void HandleDeath()
        {
            Debug.Log("Player mati!");

            // Call the match manager to check the current state of the match
            if (matchManager != null)
            {
                matchManager.PlayerDied();
            }
            else
            {
                Debug.LogWarning("MatchManager not assigned in PlayerState!");
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(attackTag))
            {
                Debug.Log("Player attacked by: " + collision.gameObject.name);
                float attackPower = GetDamageFromPlayer();
                TakeDamage(attackPower); 
            }
        }

        public float GetDamageFromPlayer()
        {
            PlayerIFAttack playerAttack = GetComponent<PlayerIFAttack>();
            if (playerAttack != null)
            {
                return playerAttack.attackPower; // Pastikan attackPower bernilai lebih dari 0
            }
            return 0f;
        }


        private void ApplyKnockback()
        {
            // Pastikan sumber knockback berasal dari arah collision
            Collider2D attacker = Physics2D.OverlapCircle(transform.position, 1f, LayerMask.GetMask("Attack"));
            if (attacker != null)
            {
                Vector2 knockbackDirection = (transform.position - attacker.transform.position).normalized;
                rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
            else
            {
                Debug.LogWarning("No attacker found for knockback calculation.");
            }
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
            healthBar.UpdateHealth(currentHealth,maxHealth);
            Debug.Log("" + currentHealth);
        }
    }
}
