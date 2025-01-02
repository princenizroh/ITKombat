using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "Mesin_Skill2", menuName = "Skills/Mesin/Mesin_Skill2", order = 2)]
    public class MesinSkill2 : Skills
    {
        [SerializeField] private float dashDistance = 5f;
        [SerializeField] private float dashDuration = 0.2f;

        [SerializeField] private float damage = 20f;
        [SerializeField] private float knockbackForce = 7f;

        [SerializeField] private Vector2 hitboxSize = new Vector2(2f, 2f);
        [SerializeField] private Vector2 hitboxOffset = Vector2.zero;

        public LayerMask enemyLayer;
        public LayerMask playerLayer;

        private bool isDashing = false;

        public override void Activate(GameObject parent)
        {
            if (isDashing) return; // Prevent multiple activations during a dash

            Rigidbody2D rb = parent.GetComponent<Rigidbody2D>();
            if (rb == null) return;

            string targetTag = parent.CompareTag("Player") ? "EnemyState" : "PlayerState";
            LayerMask targetLayer = parent.CompareTag("Player") ? enemyLayer : playerLayer;
            Vector2 dashDirection = new Vector2(parent.transform.localScale.x, 0).normalized;

            MonoBehaviour monoBehaviour = parent.GetComponent<MonoBehaviour>();
            if (monoBehaviour != null)
            {
                monoBehaviour.StartCoroutine(DashCoroutine(rb, dashDirection, parent, targetLayer, damage, targetTag, hitboxSize, hitboxOffset));
            }
        }

        private System.Collections.IEnumerator DashCoroutine(Rigidbody2D rb, Vector2 direction, GameObject parent, LayerMask targetLayer, float damage, string targetTag, Vector2 hitboxSize, Vector2 hitboxOffset)
        {
            isDashing = true;

            Vector2 initialPosition = rb.position;
            Vector2 targetPosition = initialPosition + direction * dashDistance;
            float elapsedTime = 0f;

            while (elapsedTime < dashDuration)
            {
                rb.MovePosition(Vector2.Lerp(initialPosition, targetPosition, elapsedTime / dashDuration));
                elapsedTime += Time.deltaTime;

                Vector2 hitboxPosition = rb.position + new Vector2(hitboxOffset.x * Mathf.Sign(direction.x), hitboxOffset.y);
                DetectAndDamageEnemies(hitboxPosition, hitboxSize, targetLayer, damage, parent, targetTag);

                yield return null;
            }

            rb.linearVelocity = Vector2.zero;
            isDashing = false;
        }

        private void DetectAndDamageEnemies(Vector2 position, Vector2 size, LayerMask layer, float damage, GameObject parent, string targetTag)
        {
            Collider2D[] hitTargets = Physics2D.OverlapBoxAll(position, size, 0f, layer);

            foreach (Collider2D target in hitTargets)
            {
                Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
                if (targetRb != null)
                {
                    Vector2 direction = (target.transform.position - parent.transform.position).normalized;
                    targetRb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);

                    if (targetTag == "EnemyState")
                    {
                        EnemyState enemyState = target.GetComponent<EnemyState>();
                        if (enemyState != null)
                        {
                            enemyState.TakeDamageFromSkill(damage);
                            Debug.Log($"Enemy {target.name} took {damage} damage.");
                        }
                    }
                    else if (targetTag == "PlayerState")
                    {
                        PlayerState playerState = target.GetComponent<PlayerState>();
                        if (playerState != null)
                        {
                            playerState.TakeDamageFromSkill(damage);
                            Debug.Log($"Player {target.name} took {damage} damage.");
                        }
                    }
                }
            }
        }

        public override void BeginCooldown(GameObject parent)
        {
            Debug.Log("Skill 2 Cooldown started.");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector3 gizmoPosition = (Vector3)hitboxOffset;
            Gizmos.DrawWireCube(gizmoPosition, hitboxSize);
        }
    }
}
