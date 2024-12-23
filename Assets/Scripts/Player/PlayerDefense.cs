using UnityEngine;
using System.Collections;

namespace ITKombat
{
    public class PlayerDefense : MonoBehaviour
    {
        [Header("Defense Settings")]
        public float blockCooldown = 1.0f;
        public bool isBlocking = false;
        public float defenseMultiplier = 0.7f;

        [Header("Parry Settings")]
        public float parryWindow = 0.2f;
        public int parryDamage = 10;
        private float lastBlockTime = -1f;
        private bool isParrying = false;

        [Header("Colliders")]
        public Transform defensePoint;
        public LayerMask enemyLayer;
        public float defenseRadius = 1f;

        private Animator anim;
        private GameObject parentPlayer;
        private PlayerState playerState;

        private AI_Attack aiAttack;
        private PlayerIFAttack playerAttack;
        public CharacterStat characterStats;

        private void Start()
        {
            parentPlayer = transform.root.gameObject;
            anim = GetComponent<Animator>();
            playerState = parentPlayer.GetComponent<PlayerState>();

            if (playerState == null)
            {
                Debug.LogError("PlayerState script is missing on the player GameObject!");
            }

            if (aiAttack == null)
            {
                Debug.LogError("AI_Attack script is missing on the player GameObject!");
            }
        }

        public void StartBlocking()
        {
            isBlocking = true;
            lastBlockTime = Time.time;
            Debug.Log(gameObject.name + " started blocking.");
            Debug.Log(isBlocking);
        }

        public void EndBlocking()
        {
            isBlocking = false;
            isParrying = false;
            Debug.Log(gameObject.name + " stopped blocking.");
            Debug.Log(isBlocking);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Attack"))
            {
                GameObject attacker = collision.transform.root.gameObject;

                // Handle attacks from both players and AI
                if (attacker != parentPlayer && (attacker.CompareTag("Enemy") || attacker.GetComponent<AI_Attack>()))
                {
                    // Handle blocking
                    if (isBlocking)
                    {
                        BlockSuccess(collision);

                        // Check if parry timing is correct
                        if (Time.time - lastBlockTime <= parryWindow)
                        {
                            PerformParry(collision);
                        }
                    }
                    else
                    {
                        // Handle non-blocked attacks
                        HandleAttack(collision, attacker);
                    }
                }
            }
        }

        private void HandleAttack(Collider2D attack, GameObject attacker)
        {
            // Determine attack power from either AI or player attacks
            if (attacker.GetComponent<PlayerIFAttack>() != null)
            {
                PlayerIFAttack playerAttackScript = attacker.GetComponent<PlayerIFAttack>();

                // Use characterStats to get the base attack power
                float attackPower = playerAttackScript.characterStats.characterBaseAtk;

                // Get the defender's base defense and apply the defense multiplier
                float defenderBaseDef = characterStats.characterBaseDef;

                // Calculate the total defense
                float totalDefense = defenderBaseDef * defenseMultiplier;

                // Apply the damage mitigation formula
                float mitigation = 1 - (defenderBaseDef / (defenderBaseDef + 100));

                // Calculate the damage multiplier based on the combo stage
                float damageMultiplier = 0.2f;  // Default to the 1st and 2nd combo

                // Check the combo stage for the attack
                if (playerAttackScript != null)
                {
                    int comboStage = playerAttackScript.combo; // Get the current combo stage

                    // Adjust the multiplier based on the combo stage
                    if (comboStage == 3)
                    {
                        damageMultiplier = 0.4f;  // 3rd combo
                    }
                    else if (comboStage >= 4)
                    {
                        damageMultiplier = 0.8f;  // Final combo
                    }
                }

                // Calculate the damage after applying defense and mitigation
                float damageAfterDefense = (attackPower - totalDefense) * damageMultiplier;

                // Apply mitigation to the damage
                float finalDamage = damageAfterDefense * mitigation;

                Debug.Log(gameObject.name + " was hit by " + attacker.name + " for " + finalDamage + " damage after mitigation.");

                // Use PlayerState for health management
                if (playerState != null)
                {
                    playerState.TakeDamage(finalDamage, 1); // Combo set to 1 for simplicity
                }
            }
            else if (attacker.GetComponent<AI_Attack>() != null)
            {
                float attackPower = attacker.GetComponent<AI_Attack>().attackPower;

                // Get the defender's base defense and apply the defense multiplier
                float defenderBaseDef = characterStats.characterBaseDef;

                // Calculate the total defense
                float totalDefense = defenderBaseDef * defenseMultiplier;

                // Apply the damage mitigation formula
                float mitigation = 1 - (defenderBaseDef / (defenderBaseDef + 100));

                // Calculate the damage multiplier based on the combo stage
                float damageMultiplier = 0.2f;  // Default to the 1st and 2nd combo

                // Check the combo stage for the attack
                if (attacker.GetComponent<AI_Attack>() != null)
                {
                    int comboStage = attacker.GetComponent<AI_Attack>().currentCombo; // Get the current combo stage

                    // Adjust the multiplier based on the combo stage
                    if (comboStage == 3)
                    {
                        damageMultiplier = 0.4f;  // 3rd combo
                    }
                    else if (comboStage >= 4)
                    {
                        damageMultiplier = 0.8f;  // Final combo
                    }
                }

                // Calculate the damage after applying defense and mitigation
                float damageAfterDefense = (attackPower - totalDefense) * damageMultiplier;

                // Apply mitigation to the damage
                float finalDamage = damageAfterDefense * mitigation;

                Debug.Log(gameObject.name + " was hit by AI for " + finalDamage + " damage after mitigation.");

                // Use PlayerState for health management
                if (playerState != null)
                {
                    playerState.TakeDamage(finalDamage, 1); // Combo set to 1 for simplicity
                }
            }
        }

        private void BlockSuccess(Collider2D attack)
        {
            if (isBlocking)
            {
                Debug.Log("Attack was blocked. No damage taken.");
                return;
            }

            Debug.Log("Attack blocked!");

            AI_Attack aiAttacker = attack.transform.root.GetComponent<AI_Attack>();
            if (aiAttacker != null)
            {
                aiAttacker.currentCombo = 0; // Interrupt the AI combo
                aiAttacker.canAttack = false; // Temporarily disable attacks
                StartCoroutine(EnableAIAttackCooldown(aiAttacker));
            }

            PlayerIFAttack playerAttack = attack.GetComponent<PlayerIFAttack>();
            if (playerAttack != null)
            {
                attack.enabled = false;
            }
        }

        private IEnumerator EnableAIAttackCooldown(AI_Attack aiAttacker)
        {
            yield return new WaitForSeconds(1.0f); // Block effect duration
            aiAttacker.canAttack = true; // Re-enable AI attacks
        }

        private void PerformParry(Collider2D attack)
        {
            isParrying = true;
            Debug.Log("Perfect parry! Counterattack initiated.");

            // Handle both AI and player attacks
            if (attack.gameObject.GetComponent<PlayerIFAttack>() != null)
            {
                PlayerState enemyState = attack.GetComponent<PlayerIFAttack>().GetComponent<PlayerState>();
                if (enemyState != null)
                {
                    enemyState.TakeDamage(parryDamage, 1);
                    Debug.Log("Enemy took " + parryDamage + " damage from parry!");
                }
            }
            else if (attack.gameObject.GetComponent<AI_Attack>() != null)
            {
                AI_Attack aiAttacker = attack.GetComponent<AI_Attack>();
                if (aiAttacker != null)
                {
                    Debug.Log("Parried AI attack! AI combo interrupted.");
                    aiAttacker.currentCombo = 0; // Interrupt AI combo
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (defensePoint == null)
                return;

            Gizmos.DrawWireSphere(defensePoint.position, defenseRadius);
        }
    }
}
