// PlayerState.cs
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
        private Animator playerAnimator;
        public AudioSource[] hitAudioSources;
        public string[] hitAnimationTriggers = { "Hit1", "Hit2", "Hit3" };
        public string idleAnimationTrigger = "Idle";
        public Rigidbody2D rb;
        public float knockbackForce = 1f;

        private int currentRound = 1;

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
        }

        private void Update()
        {
            float attackPower = GetDamageFromPlayer();

            if (Input.GetKeyDown(KeyCode.H))
            {
                // float attackPower = GetDamageFromPlayer();
                if (attackPower > 0)
                {
                    TakeDamage(attackPower);
                }
                else
                {
                    Debug.LogWarning("Attack power is zero or negative!");
                }
            }
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            healthBar.UpdateHealth(currentHealth, maxHealth);
            Debug.Log($"Player taking {damage} damage. Current health: {currentHealth}");

            if (currentHealth <= 0)
            {
                Debug.Log("Player mati!");
                if (currentRound == 1)
                {
                    Debug.Log("Round 1 berakhir");
                    currentRound++;
                    StartNewRound();
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
                TakeDamage(attackPower); 
            }
        }

        public float GetDamageFromPlayer()
        {
            PlayerAttackTestNope playerAttack = GetComponent<PlayerAttackTestNope>();
            if (playerAttack != null)
            {
                return playerAttack.attackPower; // Pastikan attackPower bernilai lebih dari 0
            }
            return 0f;
        }

        private void EndGame()
        {
            Debug.Log("Game Berakhir");
            // Implementasikan logika end game, bisa panggil UI atau lainnya.
        }

        private void ApplyKnockback()
        {
            Vector2 knockbackDirection = (transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)).normalized;
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }

        private IEnumerator PlayRandomHitAnimation()
        {
            string randomHitAnimation = hitAnimationTriggers[Random.Range(0, hitAnimationTriggers.Length)];
            playerAnimator.SetTrigger(randomHitAnimation);
            yield return new WaitForSeconds(0.5f);
            playerAnimator.SetTrigger(idleAnimationTrigger);
        }

        private void PlayRandomHitSound()
        {
            if (hitAudioSources.Length > 0)
            {
                AudioSource randomAudioSource = hitAudioSources[Random.Range(0, hitAudioSources.Length)];
                randomAudioSource.Play();
            }
        }
    }
}
