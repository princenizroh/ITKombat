using UnityEngine;
using System.Collections;
using Unity.Netcode;

namespace ITKombat
{
    public class EnemyState : NetworkBehaviour
    {
        public static EnemyState Instance;
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
        public bool checkDamage = false;

        public bool canAttack;

        private void Awake()
        {
            Instance = this;
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

        public bool CheckDamage()
        {
            return checkDamage;
        }
        private void OnHealthChanged(float oldValue, float newValue)
        {
            currentHealth.Value = maxHealth.Value;
            healthBar.SetMaxHealth(maxHealth.Value);
            Debug.Log("currentHealth.Value " + currentHealth.Value);
        }

        [Tooltip("Take Damage Singleplayer with AI")]
        public void TakeDamage(float damage)
        {   
            currentHealth.Value -= damage;
            Debug.Log("currentHealth.Value " + currentHealth.Value);
            healthBar.UpdateHealth(currentHealth.Value, maxHealth.Value);
            Debug.Log("currentHealth.Value " + currentHealth.Value);
            if (currentHealth.Value <= 0)
            {
                HandleDeath();
            }
            else
            {
                checkDamage = true;
                canAttack = false;
                if (AI_Attack.Instance != null)
                {
                    AI_Attack.Instance.GetCanAttack(canAttack);
                }
                else
                {
                    Debug.LogWarning("AI_Attack.Instance is null. Skipping GetCanAttack.");
                }
                Debug.Log("Check Damage" + checkDamage);

                Debug.Log("Memanggil OnEnemyDamaged");
                AIDamageChecker.Instance.OnEnemyDamaged();
                PlayerDamageChecker.Instance.OnEnemyDamaged();
                StartCoroutine(ResetCheckDamage()); // Atur ulang ke false setelah durasi tertentu.
                StartCoroutine(ResetAICanAttack());
                // ApplyKnockback();
                // AttackedAnimation(combo);
                PlayRandomHitSound();
            
            }
            checkDamage = false; //Hapus kalau mau langsung 4
        }

        [Tooltip("Take Damage Multiplayer")]
        [ServerRpc(RequireOwnership = false)]
        public void TakeDamageServerRpc(float damage)
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

            if (healthBar != null)
            {
                healthBar.UpdateHealth(currentHealth.Value, maxHealth.Value);
            }

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
        }
        private IEnumerator ResetCheckDamage()
        {
            yield return new WaitForSeconds(0.1f); // Sesuaikan durasi ini sesuai kebutuhan.
            checkDamage = false;
            // Debug.Log("Pemain tidak lagi menerima serangan.");
        }

        private IEnumerator ResetAICanAttack()
        {
            Debug.Log("Tunggu 5 Detik");

            yield return new WaitForSeconds(1f); // Sesuaikan durasi 
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
            Debug.Log("currentHealth.Value " + currentHealth.Value);
        }
    }
}
