using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 250f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float moveInterval = 2f; // Interval waktu antara pergerakan
    private float nextMoveTime = 0f; // Waktu berikutnya AI bisa bergerak
    private float nextAttackTime = 0f;
    private Transform player;
    private Rigidbody2D rb;
    private bool facingRight = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, player.position) <= attackRange && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
        else if (Time.time >= nextMoveTime)
        {
            MoveTowardsPlayer();
            nextMoveTime = Time.time + moveInterval; // Atur waktu berikutnya untuk bergerak
        }

        // Periksa dan sesuaikan arah AI untuk selalu menghadap ke player
        FlipTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
    }

    void Attack()
    {
        int randomAttack = Random.Range(1, 4); // Menghasilkan angka acak antara 1 hingga 3
        Debug.Log("Enemy melakukan serangan: Attack" + randomAttack);
        // Jika menggunakan animator, Anda dapat memicu animasi serangan di sini
        // animator.SetTrigger("Attack" + randomAttack);
    }

    // Method untuk memastikan AI selalu menghadap ke player
    void FlipTowardsPlayer()
    {
        if (player != null)
        {
            // Jika posisi AI di sebelah kiri player dan tidak menghadap kanan, atau di kanan dan tidak menghadap kiri
            if ((player.position.x > transform.position.x && !facingRight) || (player.position.x < transform.position.x && facingRight))
            {
                Flip();
            }
        }
    }

    // Method untuk membalikkan arah AI
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1; // Membalik skala pada sumbu X
        transform.localScale = theScale;
    }
}
