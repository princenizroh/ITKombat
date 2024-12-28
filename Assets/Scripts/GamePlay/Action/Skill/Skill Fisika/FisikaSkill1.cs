using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "Fisika_Skill1", menuName = "Skills/Fisika/Fisika_Skill1", order = 1)]
    public class FisikaSkill1 : Skills
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
                PlayerState targetPlayerState = hitCollider.GetComponent<PlayerState>();
                if (targetPlayerState != null && hitCollider.CompareTag("Enemy"))
                {
                    targetPlayerState.TakeDamageFromSkill(damage); // Berikan damage ke target menggunakan PlayerState
                    // NewSoundManager.Instance.PlaySound("Fisika_Skill1", parent.transform.position);
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
