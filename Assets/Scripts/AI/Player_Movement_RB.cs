using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update adalah fungsi yang dipanggil setiap frame
    void Update()
    {
        Move();
        Jump();
    }

    // Fungsi untuk pergerakan dengan WASD
    void Move()
    {
        // Ambil input horizontal dari tombol A dan D (atau panah kiri dan kanan)
        float moveInput = Input.GetAxis("Horizontal");

        // Gerakan karakter berdasarkan input
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    // Fungsi untuk lompat menggunakan tombol Spasi
    void Jump()
    {
        // Jika tombol spasi ditekan dan pemain berada di tanah
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Tambahkan gaya ke atas (lompatan)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
}