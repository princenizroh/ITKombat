using UnityEngine;
using System.Collections;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;

namespace ITKombat
{
    public class PlayerState : NetworkBehaviour
    {
        public FelixStateMachine stateMachine;
        public static PlayerState Instance;
        public NetworkVariable<float> maxHealth = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<float> currentHealth = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public string attackTag = "Attack"; 
        public HealthBar healthBar;
        public MatchManager matchManager;
        public AudioSource[] hitAudioSources;
        public string[] hitAnimationTriggers = { "Hit1", "Hit2", "Hit3" };
        public string idleAnimationTrigger = "Idle";
        public Rigidbody2D rb;
        public float knockbackForce = 1f;
        public event DamageTaken OnTakeDamage; // Event to notify damage taken
        public delegate void DamageTaken(GameObject player);
        public PlayerSkill playerSkill; // Reference to PlayerSkill

        public bool canAttack;
        public bool checkDamage = false;

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
            if (IsServer)
            {
                maxHealth.Value = 100f;
                currentHealth.Value = maxHealth.Value;
            }
            Debug.Log("Current Health: " + currentHealth.Value);
            if (healthBar != null)
            {
                healthBar.SetMaxHealth(maxHealth.Value);
                healthBar.UpdateHealth(currentHealth.Value, maxHealth.Value);
            }

            // healthBar.SetMaxHealth(maxHealth);

            // Dapatkan referensi ke MatchManager
            if (matchManager == null)
            {
                matchManager = MatchManager.Instance;
            }
        }
        public override void OnNetworkSpawn()
        {
            Debug.Log("OnNetworkSpawn");
            maxHealth.OnValueChanged += OnMaxHealthChanged;
            currentHealth.OnValueChanged += OnCurrentHealthChanged;
        }
        private void OnMaxHealthChanged(float oldValue, float newValue)
        {
            if (healthBar != null)
            {
                Debug.Log("On Max Health Max health changed to: " + newValue);
                healthBar.SetMaxHealth(newValue);
                Debug.Log("On Max Health currentHealth.Value " + currentHealth.Value);
            }
        }

        private void OnCurrentHealthChanged(float oldValue, float newValue)
        {
            if (healthBar != null)
            {
                healthBar.UpdateHealth(newValue, maxHealth.Value);
            }
        }

        private void Update()
        {
            // float attackPower = GetDamageFromPlayer();
            if (Input.GetKeyDown(KeyCode.O))
            {
                TakeDamageServerRpc(10f,1f);
            }
        }

        public void UpdateHealth()
        {
            // Update health or perform any logic when health is updated
            healthBar.UpdateHealth(currentHealth.Value, maxHealth.Value);
        }
        
        [Tooltip("Take Damage SinglePlayer with AI ")]
        public void TakeDamage(float damage,float combo)
        {
            // Check if Skill 2 is active and block attack
            PlayerSkill playerSkill = GetComponent<PlayerSkill>();
            if (playerSkill != null && playerSkill.skill2Asset != null)
            {
                var skill2 = playerSkill.skill2Asset;
                if (skill2.IsActive() && skill2.BlockAttack())
                {
                    Debug.Log("Attack blocked by 2nd skill!");
                    return; // Exit the method, attack is blocked
                }
            }

            currentHealth.Value -= damage;
            healthBar.UpdateHealth(currentHealth.Value, maxHealth.Value);
            if (currentHealth.Value <= 0)
            {
                HandleDeath();
            }
            else
            {
                checkDamage = true;
                canAttack = false;
                PlayerIFAttack.Instance.GetCanAttack(canAttack);
                PlayerDamageChecker.Instance.OnEnemyDamaged();
                StartCoroutine(ResetCheckDamage());
                StartCoroutine(ResetCanAttack());
                // ApplyKnockback();
                Debug.Log(currentHealth.Value);
                AttackedAnimation(combo);
                PlayRandomHitSound();
            }
            OnTakeDamage?.Invoke(gameObject);
                // AttackedAnimation(combo);
                // PlayRandomHitSound();
            
            checkDamage = false;
        }

        [Tooltip("Take Damage Multiplayer")]
        [ServerRpc(RequireOwnership = false)]
        public void TakeDamageServerRpc(float damage, float combo)
        {
            if (!IsServer) 
            {
                Debug.LogWarning("TakeDamageServerRpc called on client!");
                return;
            }
            if (damage >= currentHealth.Value)
            {
                damage = currentHealth.Value; 
            }
            currentHealth.Value -= damage;
            Debug.Log($"Damage taken: {damage}, Remaining health: {currentHealth.Value}");

            healthBar.UpdateHealth(currentHealth.Value, maxHealth.Value);

            if (currentHealth.Value <= 0)
            {
                HandleDeath();
            }
            else
            {
                checkDamage = true;
                canAttack = false;
                StartCoroutine(ResetCheckDamage());
            }
            checkDamage = false;
            OnTakeDamage?.Invoke(gameObject);
        }


        private IEnumerator ResetCheckDamage()
        {
            yield return new WaitForSeconds(0.1f); // Sesuaikan durasi ini sesuai kebutuhan.
            checkDamage = false;
            // Debug.Log("Pemain tidak lagi menerima serangan.");
        }

         private IEnumerator ResetCanAttack()
        {
            yield return new WaitForSeconds(1f); // Sesuaikan durasi 
            canAttack = true;
            PlayerIFAttack.Instance.GetCanAttack(canAttack);
            // Debug.Log("Berhasil Reset Can Attack Player Menjadi" + canAttack);
        }

        public void TakeDamageFromSkill(float skillDamage)
        {
            currentHealth.Value -= skillDamage;
        
            healthBar.UpdateHealth(currentHealth.Value, maxHealth.Value);

            if (currentHealth.Value <= 0)
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
                matchManager.PlayerDied();
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
                    break;
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
            currentHealth.Value = maxHealth.Value;
            healthBar.UpdateHealth(currentHealth.Value,maxHealth.Value);
            Debug.Log("" + currentHealth.Value);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ResetHealthServerRpc()
        {
            ResetHealth();
        }   

        public float GetCurrentHealth()
        {
            return currentHealth.Value;
        }
    }
}
