using ITKombat;
using UnityEngine;

[CreateAssetMenu]
public class Skill1 : Skills 
{
    public float damage = 10f; // Jumlah damage yang diberikan oleh Skill 1
    public float attackRadius = 10f; // Radius untuk mendeteksi target
    public override void Activate(GameObject parent)
    {
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