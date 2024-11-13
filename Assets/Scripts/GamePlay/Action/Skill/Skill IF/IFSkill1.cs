using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "IF_Skill1", menuName = "Skills/IF/IF_Skill1", order = 1)]
    public class IFSkill1 : Skills
    {
        // Masukin sound dan anim disini
        
        [SerializeField] private Transform attackPoint;
        private float damage = 30f;
        private float force = 5f;
        private float radius = 1f;

        public LayerMask enemy;

        public override void Activate(GameObject parent)
        {
            // Masukin sound dan anim disini
            
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, radius, enemy);
            foreach (Collider2D enemy in hitEnemies)
            {
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                AI_Defense enemyDefense = enemy.GetComponent<AI_Defense>();
                if (enemyRb != null && !enemyDefense.isBlocking)
                {
                    enemyRb.AddForce(parent.transform.right * force, ForceMode2D.Impulse);

                    GameObject enemyStateObject = GameObject.FindGameObjectWithTag("EnemyState");

                    if (enemyStateObject != null)
                    {
                        EnemyState enemyState = enemyStateObject.GetComponent<EnemyState>();
                        if (enemyState != null)
                        {
                            enemyState.TakeDamage(damage);
                        }
                    }
                    else
                    {
                        Debug.Log("EnemyState not found.");
                    }
                }
                else
                {
                    Debug.Log("Skill 1 masih cooldown.");
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
