
using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "Mesin_Skill3", menuName = "Skills/Mesin/Mesin_Skill3", order = 3)]
    public class MesinSkill3 : Skills
    {
        // Masukin sound dan anim disini
        public GameObject aoeEffectPrefab;  // Prefab for AoE effect
        public float damagePerSecond = 5f;
        public float skillPointsRequired = 1f; // Skill points for AoE area

        private GameObject aoeInstance;
        private bool isAoEActive = false;

        public override void Activate(GameObject parent)
        {
/*            if (parent.GetComponent<PlayerStats>().skillPoints >= skillPointsRequired)
            {
                // Spawn AoE effect
                Vector3 spawnPosition = parent.transform.position;  // Define the AoE area based on skill points
                aoeInstance = Instantiate(aoeEffectPrefab, spawnPosition, Quaternion.identity);
                isAoEActive = true;

                Debug.Log("AoE attack activated.");
            }*/
        }

        public override void BeginCooldown(GameObject parent)
        {
            if (isAoEActive)
            {
                Destroy(aoeInstance);  // Remove the AoE effect after activeTime
                isAoEActive = false;
                Debug.Log("AoE attack deactivated.");
            }
        }

        private void ApplyAoEDamage(GameObject player)
        {
/*            // This method should be called during the AoE active time to deal damage
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(player.transform.position, skillPointsRequired); // Detects enemies in the AoE radius

            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Health>().TakeDamage(damagePerSecond * Time.deltaTime);
            }*/
        }
    }
}
