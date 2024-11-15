using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "Kimia_Skill2", menuName = "Skills/Kimia/Kimia_Skill2", order = 2)]
    public class KimiaSkill2 : Skills
    {
        // Masukin sound dan anim disini
        public GameObject shieldEffect;  // Visual effect for the shield
        private GameObject currentShield;
        public float reflectionDamageMultiplier = 1.0f;

        private bool shieldActive = false;

        public override void Activate(GameObject parent)
        {
            // Create the shield effect around the player
            currentShield = Instantiate(shieldEffect, parent.transform.position, Quaternion.identity, parent.transform);
            shieldActive = true; // Mark the shield as active
            Debug.Log("Reflective shield activated.");
        }

        public override void BeginCooldown(GameObject parent)
        {
            if (currentShield != null) Destroy(currentShield);
            shieldActive = false;
            Debug.Log("Reflective shield deactivated.");
        }

        public void ReflectDamage (GameObject player, float damage)
        {
            // Reflect damage to the attacker
            float reflectedDamage = damage * reflectionDamageMultiplier;
/*            player.GetComponent<Health>().TakeDamage(reflectedDamage); // Assuming the enemy has a Health script
*/            Debug.Log("Reflected " + reflectedDamage + " damage to the attacker.");

        }
    }
}
