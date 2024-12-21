using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "Kelautan_Skill3", menuName = "Skills/Kelautan/Kelautan_Skill3", order = 3)]
    public class KelautanSkill3 : Skills
    {
        // Masukin sound dan anim disini

        public float damage = 20f;
        public float stunDuration = 2f;

        public override void Activate(GameObject parent)
        {
/*            // Masukin sound dan anim disini

            player.GetComponent<Animator>().SetTrigger("StunAttack");

            // Detect and apply damage and stun
            GameObject target = DetectEnemyInFront(player);
            if (target != null)
            {
                target.GetComponent<EnemyController>().TakeDamage(damage);
                target.GetComponent<EnemyController>().ApplyStun(stunDuration);
                Debug.Log("Enemy hit and stunned.");
            }*/
        }

        public override void BeginCooldown(GameObject parent)
        {
            //Logic cooldown skill di taruh disini
            Debug.Log("Skill 1 Cooldown");
        }
    }
}
