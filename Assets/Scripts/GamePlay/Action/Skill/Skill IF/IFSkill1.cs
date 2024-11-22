using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "IF_Skill1", menuName = "Skills/IF/IF_Skill1", order = 1)]
    public class IFSkill1 : Skills
    {
        // Masukin sound dan anim disini

        [SerializeField] private Transform playerAttackPoint;
        [SerializeField] private Transform enemyAttackPoint;
        private float damage = 30f;
        private float force = 5f;
        private float radius = 1f;

        public LayerMask enemyLayer;
        public LayerMask playerLayer;

        public override void Activate(GameObject parent)
        {
            // Masukin sound dan anim disini

            if (parent.CompareTag("Player"))
            {
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(playerAttackPoint.position, radius, enemyLayer);
                foreach (Collider2D enemy in hitEnemies)
                {
                    Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                    if (enemyRb != null)
                    {
                        Vector2 direction = (enemy.transform.position - playerAttackPoint.position).normalized;

                        enemyRb.AddForce(direction * force, ForceMode2D.Impulse);

                        GameObject enemyStateObject = GameObject.FindGameObjectWithTag("EnemyState");

                        if (enemyStateObject != null)
                        {
                            EnemyState enemyState = enemyStateObject.GetComponent<EnemyState>();
                            if (enemyState != null)
                            {
                                enemyState.TakeDamageFromSkill(damage);
                            }
                        }
                        else
                        {
                            Debug.Log("EnemyState not found.");
                        }
                    }
                }
            }
            else if (parent.CompareTag("Enemy"))
            {
                Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enemyAttackPoint.position, radius, playerLayer);
                foreach (Collider2D player in hitPlayer)
                {
                    Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        Vector2 direction = (player.transform.position - enemyAttackPoint.position).normalized;

                        playerRb.AddForce(direction * force, ForceMode2D.Impulse);

                        GameObject playerStateObject = GameObject.FindGameObjectWithTag("PlayerState");

                        if (playerStateObject != null)
                        {
                            PlayerState playerState = playerStateObject.GetComponent<PlayerState>();
                            if (playerState != null)
                            {
                                playerState.TakeDamageFromSkill(damage);
                            }
                        }
                        else
                        {
                            Debug.Log("PlayerState not found.");
                        }
                    }
                }
            }
        }


        public override void BeginCooldown(GameObject parent)
        {
            //Logic cooldown skill di taruh disini
            Debug.Log("Skill 1 Cooldown");
        }
    }
}
