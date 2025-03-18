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
        private BoxCollider2D defensePoint;
        public LayerMask enemyLayer;
        public float defenseRadius = 1f;

        private Animator anim;
        private GameObject parentPlayer;

        private AI_Attack aiAttack;
        public CharacterStat characterStats;

        private void Start()
        {
            parentPlayer = transform.root.gameObject;
            anim = GetComponent<Animator>();
            defensePoint = GetComponent<BoxCollider2D>();
        }

        public void StartBlocking()
        {
            isBlocking = true;
            lastBlockTime = Time.time;
        }

        public void EndBlocking()
        {
            isBlocking = false;
            isParrying = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Attack"))
            {
                GameObject attacker = collision.transform.root.gameObject;

                if (attacker != parentPlayer && (attacker.CompareTag("Enemy") || (attacker.GetComponent<AI_Attack>() || attacker.GetComponent<PlayerIFAttack>())))
                {
                    if (isBlocking)
                    {
                        BlockSuccess(collision);
                        if (Time.time - lastBlockTime <= parryWindow)
                        {
                            PerformParry(collision);
                        }
                    }
                    else
                    {
                        HandleAttack(collision, attacker);
                    }
                }
            }
        }

        private void HandleAttack(Collider2D attack, GameObject attacker)
        {
            if (attacker.GetComponent<PlayerIFAttack>() != null)
            {
                PlayerIFAttack playerAttackScript = attacker.GetComponent<PlayerIFAttack>();
                float attackPower = playerAttackScript.characterStats.characterBaseAtk;
                float defenderBaseDef = characterStats.characterBaseDef;
                float totalDefense = defenderBaseDef * defenseMultiplier;
                float mitigation = 1 - (defenderBaseDef / (defenderBaseDef + 100));

                float damageMultiplier = 0.2f;
                int comboStage = playerAttackScript.combo;
                if (comboStage == 3) damageMultiplier = 0.4f;
                else if (comboStage >= 4) damageMultiplier = 0.8f;

                float damageAfterDefense = Mathf.Max(0, (attackPower - totalDefense) * damageMultiplier);
                float finalDamage = Mathf.Max(0, damageAfterDefense * mitigation);

                PlayerState playerState = GameObject.FindGameObjectWithTag("EnemyState")?.GetComponent<PlayerState>();
                playerState?.TakeDamageServerRpc(finalDamage, 1);
            }
            else if (attacker.GetComponent<AI_Attack>() != null)
            {
                float attackPower = attacker.GetComponent<AI_Attack>().attackPower;
                float defenderBaseDef = characterStats.characterBaseDef;
                float totalDefense = defenderBaseDef * defenseMultiplier;
                float mitigation = 1 - (defenderBaseDef / (defenderBaseDef + 100));

                float damageMultiplier = 0.2f;
                int comboStage = attacker.GetComponent<AI_Attack>().currentCombo;
                if (comboStage == 3) damageMultiplier = 0.4f;
                else if (comboStage >= 4) damageMultiplier = 0.8f;

                float damageAfterDefense = Mathf.Max(0, (attackPower - totalDefense) * damageMultiplier);
                float finalDamage = Mathf.Max(0, damageAfterDefense * mitigation);

                PlayerState playerState = GameObject.FindGameObjectWithTag("PlayerState")?.GetComponent<PlayerState>();
                playerState?.TakeDamage(finalDamage, 1);
            }
        }

        private void BlockSuccess(Collider2D attack)
        {
            if (isBlocking)
            {
                return;
            }

            AI_Attack aiAttacker = attack.transform.root.GetComponent<AI_Attack>();
            if (aiAttacker != null)
            {
                aiAttacker.currentCombo = 0;
                aiAttacker.canAttack = false;
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
            yield return new WaitForSeconds(1.0f);
            aiAttacker.canAttack = true;
        }

        private void PerformParry(Collider2D attack)
        {
            isParrying = true;

            if (attack.gameObject.GetComponent<PlayerIFAttack>() != null)
            {
                PlayerState enemyState = attack.GetComponent<PlayerIFAttack>().GetComponent<PlayerState>();
                enemyState?.TakeDamage(parryDamage, 1);
            }
            else if (attack.gameObject.GetComponent<AI_Attack>() != null)
            {
                AI_Attack aiAttacker = attack.GetComponent<AI_Attack>();
                if (aiAttacker != null)
                {
                    aiAttacker.currentCombo = 0;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (defensePoint == null) return;
            Gizmos.DrawWireSphere(defensePoint.transform.position, defenseRadius);
        }
    }
}