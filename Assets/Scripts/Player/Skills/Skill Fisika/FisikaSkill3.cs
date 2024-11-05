using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "Fisika_Skill3", menuName = "Skills/Fisika/Fisika_Skill3", order = 3)]
    public class FisikaSkill3 : Skills
    {
        // Masukin sound dan anim disini

        private float attackRadius = 1f;
        private float damage = 30f;
        private float force = 5f;

        // Cooldown Settings
        private float skill1Cooldown = 10f;

        public override void Activate(GameObject parent)
        {
            // Masukin sound dan anim disini

            Vector3 skillPosition = parent.transform.position;

            // Mendeteksi semua objek dalam radius serangan
            Collider[] hitColliders = Physics.OverlapSphere(skillPosition, attackRadius);

            foreach (Collider hitCollider in hitColliders)
            {
                // Pastikan target adalah lawan dan memiliki komponen HealthBarTest
                HealthBarTest targetHealth = hitCollider.GetComponent<HealthBarTest>();
                if (targetHealth != null && hitCollider.CompareTag("Enemy"))
                {
                    targetHealth.TakeDamage(damage); // Berikan damage ke target
                    Debug.Log("Skill 1 Aktif - Memberikan " + damage + " damage ke " + hitCollider.gameObject.name);
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
