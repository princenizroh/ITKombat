using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "Mesin_Skill1", menuName = "Skills/Mesin/Mesin_Skill1", order = 1)]
    public class MesinSkill1 : Skills
    {
        // Masukin sound dan anim disini
        public float damage = 10f;
        public float knockbackForce = 5f;

        public override void Activate(GameObject parent)
        {
 /*           // Deal damage
            target.GetComponent<Health>().TakeDamage(damage);

            // Apply knockback to the enemy
            Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
            Vector2 knockback = (target.transform.position - parent.transform.position).normalized * knockbackForce;
            rb.AddForce(knockback, ForceMode2D.Impulse);
            Debug.Log("Thrust attack performed with knockback.");*/
        }

        public override void BeginCooldown(GameObject parent)
        {
            //Logic cooldown skill di taruh disini
            Debug.Log("Skill 1 Cooldown");
        }
    }
}
