using ITKombat;
using UnityEngine;

[CreateAssetMenu]
public class Skill2 : Skills
{
    public float damage = 20f; // Jumlah damage yang diberikan oleh Skill 2
    public float attackRadius = 7f; // Radius untuk mendeteksi target

    public override void Activate(GameObject parent)
    {
        Vector3 attackPosition = parent.transform.position;

        // Mendeteksi semua objek dalam radius serangan
        Collider[] hitColliders = Physics.OverlapSphere(attackPosition, attackRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            // Pastikan target adalah lawan dan memiliki komponen HealthBarTest
            HealthBarTest targetHealth = hitCollider.GetComponent<HealthBarTest>();
            if (targetHealth != null && hitCollider.CompareTag("Enemy"))
            {
                targetHealth.TakeDamage(damage); // Berikan damage ke target
                Debug.Log("Skill 2 Aktif - Memberikan " + damage + " damage ke " + hitCollider.gameObject.name);
            }
        }
    }
    public override void BeginCooldown(GameObject parent)
    {
        //Logic cooldown skill di taruh disini
        Debug.Log("Skill 2 Cooldown");
    }
}