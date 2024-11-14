using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "Kelautan_Skill1", menuName = "Skills/Kelautan/Kelautan_Skill1", order = 1)]
    public class KelautanSkill1 : Skills
    {
        // Masukin sound dan anim disini
        public float damagePerHit = 10f;
        public int hitCount = 4;

        public override void Activate(GameObject parent)
        {
/*            // Masukin sound dan anim disini
            parent.GetComponent<Animator>().SetTrigger("ComboAttack");

            // Deal 4 hits immediately
            for (int i = 0; i < hitCount; i++)
            {
                GameObject target = DetectEnemyInFront(player);
                if (target != null)
                {
                    target.GetComponent<EnemyController>().TakeDamage(damagePerHit);
                    Debug.Log($"Enemy hit with combo attack {i + 1}");
                }
            }*/
        }

        public override void BeginCooldown(GameObject parent)
        {
            //Logic cooldown skill di taruh disini
            Debug.Log("Skill 1 Cooldown");
        }

        private GameObject DetectEnemyInFront(GameObject player)
        {
            
            return null;
        }
    }
}
