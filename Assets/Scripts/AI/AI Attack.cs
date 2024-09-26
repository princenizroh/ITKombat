using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttack : MonoBehaviour
{
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    private float lastAttackTime;
    public Transform player;

    void Update()
    {
        // Mengecek jarak AI dengan Player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Jika dalam jangkauan attack range dan cooldown selesai, lakukan attack
        if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            PerformRandomAttack();
            lastAttackTime = Time.time;
        }
    }

    // Fungsi untuk melakukan serangan acak
    void PerformRandomAttack()
    {
        int randomAttack = Random.Range(1, 3); // Random antara attack1 dan attack2

        if (randomAttack == 1)
        {
            Attack1();
        }
        else
        {
            Attack2();
        }
    }

    // Fungsi Attack 1
    void Attack1()
    {
        Debug.Log("AI performed Attack1!");
        // Tambahkan logika serangan 1 di sini
    }

    // Fungsi Attack 2
    void Attack2()
    {
        Debug.Log("AI performed Attack2!");
        // Tambahkan logika serangan 2 di sini
    }
}
