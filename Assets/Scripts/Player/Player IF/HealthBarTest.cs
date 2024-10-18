using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ITKombat
{
    public class HealthBarTest : MonoBehaviour
    {
        public Slider healthSlider;
        public Slider easeHealthSlider;
        public int maxHealth = 100;
        public float health = 100f;
        public float lerpSpeed = 5f;
        public string attackTag = "Attack"; 
        private int currentRound = 1;

        public float knockbackForce = 1f; 
        public Animator playerAnimator; 
        public AudioSource[] hitAudioSources; 
        public string[] hitAnimationTriggers = { "Hit1", "Hit2", "Hit3" };
        public string idleAnimationTrigger = "Idle";

        private Rigidbody2D rb;

        [SerializeField] private ParticleSystem HittedParticle = null;
        private void Start()
        {
            healthSlider.maxValue = maxHealth;
            easeHealthSlider.maxValue = maxHealth;
            rb = GetComponent<Rigidbody2D>(); 
            UpdateHealthBar();
        }

        private void Update()
        {
            if (healthSlider.value != easeHealthSlider.value)
            {
                easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, healthSlider.value, lerpSpeed * Time.deltaTime);
            }

            DebugKey();
        }

        private void DebugKey()
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                float attackPower = GetDamageFromPlayer();
                TakeDamage(attackPower); 
            }
        }

        // Handle collision with enemy
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(attackTag))
            {
                Debug.Log("Player attack the enemy: " + collision.gameObject.name);
                float attackPower = GetDamageFromPlayer();
                TakeDamage(attackPower); 
            }
        }

        public float GetDamageFromPlayer()
        {
            PlayerAttackTestNope playerAttack = GetComponent<PlayerAttackTestNope>();
            if (playerAttack != null)
            {
                return playerAttack.attackPower;
            }
            return 0f;
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
            Debug.Log($"Taking {damage} damage. Current health: {health}");

            if (health <= 0)
            {
                Debug.Log("Mati!");
                if (currentRound == 1)
                {
                    Debug.Log("Round 2");
                    currentRound++;
                    ResetHealth();
                }
                else
                {
                    Debug.Log("Game Berakhir");
                    ShowEndGameButton();
                }
            }
            else
            {
                HittedParticle.Play();

                ApplyKnockback();

                StartCoroutine(PlayRandomHitAnimation());

                PlayRandomHitSound();
            }
            
            UpdateHealthBar();
        }

        private void ResetHealth()
        {
            health = maxHealth;
            UpdateHealthBar();
        }

        public void UpdateHealthBar()
        {
            healthSlider.value = health;
        }

        private void ShowEndGameButton()
        {
            GameObject endGameButton = GameObject.Find("EndGameButton");
            if (endGameButton != null)
            {
                endGameButton.SetActive(true);
            }
        }

        private void ApplyKnockback()
        {
            Vector2 knockbackDirection = (transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)).normalized; // Adjust direction based on your game design
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }

        private IEnumerator PlayRandomHitAnimation()
        {
            string randomHitAnimation = hitAnimationTriggers[Random.Range(0, hitAnimationTriggers.Length)];
            playerAnimator.SetTrigger(randomHitAnimation); // Set trigger for random hit animation

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