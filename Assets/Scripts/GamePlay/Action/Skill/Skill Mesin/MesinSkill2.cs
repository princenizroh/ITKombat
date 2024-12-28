using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "Mesin_Skill2", menuName = "Skills/Mesin/Mesin_Skill2", order = 2)]
    public class MesinSkill2 : Skills
    {
        // Masukin sound dan anim disini

        public float dashDistance = 5f;
        public float damage = 20f;
        public float knockbackForce = 7f;

        private bool isDashing = false;  // Track if the dash is active

        public override void Activate(GameObject parent)
        {
       /*     isDashing = true;

            // Dash forward
            Rigidbody2D rb = parent.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(dashDistance * parent.transform.localScale.x, rb.velocity.y); // Dash direction based on player facing

            // Detect if the player hits another player during the dash
            GameObject target = DetectEnemy(player);
            if (target != null)
            {
                // Deal damage to the other player
                target.GetComponent<Health>().TakeDamage(damage);

                // suara skill2 mesin
                // NewSoundManager.Instance.PlaySound("Mesin_Skill2", parent.transform.position);

                // Apply knockback
                Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
                Vector2 knockback = (target.transform.position - parent.transform.position).normalized * knockbackForce;
                targetRb.AddForce(knockback, ForceMode2D.Impulse);

                Debug.Log("Dash attack hit the target and dealt significant damage.");
            }*/
        }
            
        public override void BeginCooldown(GameObject parent)
        {
            // Reset velocity to stop further movement after the dash
            Rigidbody2D rb = parent.GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector2.zero;  // Stop the player from moving further
            Debug.Log("Skill 1 Cooldown");
        }

        private GameObject DetectEnemy (GameObject player)
        {
            float detectionRadius = 1.5f; // Define the radius in which to detect enemies
            LayerMask enemyLayer = LayerMask.GetMask("Enemy"); // Assume enemies are in a layer called "Enemy"

            Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(player.transform.position, detectionRadius, enemyLayer);

            if (enemiesInRange.Length > 0)
            {
                // Return the first detected enemy (or handle multiple as needed)
                return enemiesInRange[0].gameObject;
            }

            return null; // No enemies detected
        }
    }
}
