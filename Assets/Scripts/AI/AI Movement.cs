using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Movement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float moveInterval = 1f; // Interval waktu antara pergerakan
    private float nextMoveTime = 0f; // Waktu berikutnya AI bisa bergerak
    private float nextAttackTime = 0f;
    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, player.position) <= attackRange && Time.time >= nextAttackTime)
        {
            Touch();
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
        else if (Time.time >= nextMoveTime)
        {
            MoveTowardsPlayer();
            nextMoveTime = Time.time + moveInterval; // Atur waktu berikutnya untuk bergerak
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
    }

    void Attack()
    {
        int randomAttack = Random.Range(1, 4); 
        Debug.Log("Enemy melakukan serangan: Attack" + randomAttack);
    }

    void Touch()
    {
        Debug.Log("Enemy Menyentuh Player, Berhenti Bergerak");
        rb.velocity = Vector2.zero;
    }
}
