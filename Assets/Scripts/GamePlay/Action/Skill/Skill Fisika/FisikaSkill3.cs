using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "Fisika_Skill3", menuName = "Skills/Fisika/Fisika_Skill3", order = 3)]
    public class FisikaSkill3 : Skills
    {
        // Masukin sound dan anim disini
        public GameObject aoePrefab;  // Prefab for the AoE effect
        public float slowAmount = 0.5f;
        public float aoeDuration = 3f;
        private GameObject aoeInstance;
        private bool isAoEActive = false;

        public override void Activate(GameObject parent)
        {
            // Masukin sound dan anim disini
            // Instantiate AoE effect at a specific point
            aoeInstance = Instantiate(aoePrefab, parent.transform.position, Quaternion.identity);
            isAoEActive = true;
            // NewSoundManager.Instance.PlaySound("Fisika_Skill3", parent.transform.position);
            Debug.Log("AoE slow activated.");
        }

        public override void BeginCooldown(GameObject parent)
        {
            if (isAoEActive)
            {
                Destroy(aoeInstance);  // Remove the AoE effect
                isAoEActive = false;
                Debug.Log("AoE slow deactivated.");
            }
        }

/*        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isAoEActive && other.CompareTag("Enemy"))
            {
                // Apply slow to the enemy
                other.GetComponent<EnemyController>().ApplySlow(slowAmount, aoeDuration);
                Debug.Log("Enemy slowed.");
            }
        }*/
    }
}
