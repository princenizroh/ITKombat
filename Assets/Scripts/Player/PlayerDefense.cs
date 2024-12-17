using UnityEngine;
using System.Collections;

namespace ITKombat
{
    public class PlayerDefense : MonoBehaviour
    {
        [Header("Defense Settings")]
        public float blockCooldown = 1.0f;
        public bool isBlocking = false;

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

        private void Start()
        {
            parentPlayer = transform.root.gameObject;
            anim = GetComponent<Animator>();
            playerState = parentPlayer.GetComponent<PlayerState>();

            aiAttack = FindObjectOfType<AI_Attack>();

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
        }

        public void EndBlocking()
        {
            isBlocking = false;
            isParrying = false;
            Debug.Log(gameObject.name + " stopped blocking.");
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
            float attackPower = 0;

            if (attacker.GetComponent<PlayerIFAttack>() != null)
            {
                attackPower = attacker.GetComponent<PlayerIFAttack>().attackPower;
            }
            else if (attacker.GetComponent<AI_Attack>() != null)
            {
                attackPower = attacker.GetComponent<AI_Attack>().attackPower;
            }

            Debug.Log(gameObject.name + " was hit by " + attacker.name);

            // Use PlayerState for health management
            if (playerState != null)
            {
                playerState.TakeDamage(attackPower, 1); // Combo set to 1 for simplicity
            }
        }

        private void BlockSuccess(Collider2D attack)
        {
            Debug.Log("Attack blocked!");

            if (attack.GetComponent<PlayerIFAttack>() != null || attack.GetComponent<AI_Attack>() != null)
            {
                attack.enabled = false;
                // Additional block effects can be triggered here
            }
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
