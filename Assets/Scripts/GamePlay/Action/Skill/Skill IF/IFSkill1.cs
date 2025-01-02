using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "IF_Skill1", menuName = "Skills/IF/IF_Skill1", order = 1)]
    public class IFSkill1 : Skills
    {
        // Damage and force values
        [SerializeField] private float damage = 30f;
        [SerializeField] private float force = 5f;

        // Hitbox configuration
        [SerializeField] private Vector2 hitboxSize = new Vector2(2f, 2f);
        [SerializeField] private Vector2 hitboxOffset = Vector2.zero;

        // Layer masks to determine valid targets
        public LayerMask enemyLayer;
        public LayerMask playerLayer;

        public override void Activate(GameObject parent)
        {
            Vector2 hitboxPosition;

            if (parent.CompareTag("Player"))
            {
                hitboxPosition = (Vector2)parent.transform.position + hitboxOffset;
                PerformAttack(hitboxPosition, hitboxSize, enemyLayer, damage, parent, "EnemyState");
            }
            else if (parent.CompareTag("Enemy"))
            {
                hitboxPosition = (Vector2)parent.transform.position + hitboxOffset;
                PerformAttack(hitboxPosition, hitboxSize, playerLayer, damage, parent, "PlayerState");
            }
        }

        private void PerformAttack(Vector2 position, Vector2 size, LayerMask layer, float damage, GameObject parent, string targetTag)
        {
            Collider2D[] hitTargets = Physics2D.OverlapBoxAll(position, size, 0f, layer);

            foreach (Collider2D target in hitTargets)
            {
                Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
                if (targetRb != null)
                {
                    Vector2 direction = (target.transform.position - parent.transform.position).normalized;
                    targetRb.AddForce(direction * force, ForceMode2D.Impulse);

                    GameObject targetStateObject = GameObject.FindGameObjectWithTag(targetTag);
                    if (targetStateObject != null)
                    {
                        if (targetTag == "EnemyState")
                        {
                            EnemyState enemyState = targetStateObject.GetComponent<EnemyState>();
                            if (enemyState != null)
                            {
                                enemyState.TakeDamageFromSkill(damage);
                                Debug.Log($"Enemy {target.name} took {damage} damage.");
                            }
                        }
                        else if (targetTag == "PlayerState")
                        {
                            PlayerState playerState = targetStateObject.GetComponent<PlayerState>();
                            if (playerState != null)
                            {
                                playerState.TakeDamageFromSkill(damage);
                                Debug.Log($"Player {target.name} took {damage} damage.");
                            }
                        }
                    }
                }
            }
        }

        public override void BeginCooldown(GameObject parent)
        {
            Debug.Log("Skill 1 Cooldown");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(Vector3.zero + (Vector3)hitboxOffset, hitboxSize);
        }
    }
}
