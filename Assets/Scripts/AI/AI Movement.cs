using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D enemy;
    public float moveSpeed = 5f; // Kecepatan gerak AI
    public float crouchSpeed = 2.5f; // Kecepatan saat crouch
    public float jumpForce = 5f; // Jika AI bisa melompat
    public float crouchDistance = 1f; // Jarak di mana AI akan crouch
    private bool canJump = true;
    private bool isCrouching = false; // Status apakah AI sedang crouch
    private Transform player; // Referensi ke posisi player
    private bool facingRight = false; // Menyimpan informasi arah wajah AI
    public float jumpCooldown = 2f; // Cooldown untuk lompat

    void Start()
    {
        enemy = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Mendapatkan referensi player
    }

    void Update()
    {
        MoveTowardsPlayer();

        // Jika AI di tanah, maka lompat bisa dilakukan
        if (Mathf.Abs(enemy.velocity.y) < 0.1f)
        {
            canJump = true;
        }

        // Cek apakah AI harus crouch
        CheckCrouch();
    }

    void MoveTowardsPlayer()
    {
        // Hitung arah dari AI ke player
        Vector2 direction = (player.position - transform.position).normalized;

        // Jika sedang crouch, gunakan kecepatan crouch, jika tidak gunakan moveSpeed biasa
        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;

        // Jika AI berada di sebelah kiri player, maka bergerak ke kanan, dan sebaliknya
        enemy.velocity = new Vector2(direction.x * currentSpeed, enemy.velocity.y);

        // Flip AI ke arah player jika diperlukan
        FlipTowardsPlayer(direction.x);
    }

    void FlipTowardsPlayer(float moveDirection)
    {
        // Jika arah movement positif (ke kanan) dan AI menghadap kiri, flip ke kanan
        if (moveDirection > 0 && !facingRight)
        {
            Flip();
        }
        // Jika arah movement negatif (ke kiri) dan AI menghadap kanan, flip ke kiri
        else if (moveDirection < 0 && facingRight)
        {
            Flip();
        }
    }

    // Method untuk membalik arah wajah AI
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1; // Membalikkan skala sumbu X untuk memutar AI
        transform.localScale = theScale;
    }

    public void Jump()
    {
        if (canJump)
        {
            enemy.velocity = new Vector2(enemy.velocity.x, jumpForce);
            StartCoroutine(JumpCooldown());
        }
    }

    IEnumerator JumpCooldown()
    {
        canJump = false; // Tidak bisa melompat selama cooldown
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true; // Lompat bisa dilakukan lagi setelah cooldown
    }

    // Method untuk memeriksa apakah AI harus crouch atau tidak
    void CheckCrouch()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= crouchDistance && !isCrouching)
        {
            Crouch();
        }
        else if (distanceToPlayer > crouchDistance && isCrouching)
        {
            StandUp();
        }
    }

    // Method untuk melakukan crouch
    void Crouch()
    {
        isCrouching = true;
        Debug.Log("Enemy is crouching");
        // Anda bisa menambahkan animasi crouch di sini jika diperlukan
        // animator.SetBool("isCrouching", true);
        // Misalnya mengubah skala AI agar terlihat lebih kecil saat crouch
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.5f, transform.localScale.z);
    }

    // Method untuk berdiri dari crouch
    void StandUp()
    {
        isCrouching = false;
        Debug.Log("Enemy stopped crouching");
        // Anda bisa menghentikan animasi crouch di sini jika diperlukan
        // animator.SetBool("isCrouching", false);
        // Kembalikan skala normal
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 2f, transform.localScale.z);
    }
}
