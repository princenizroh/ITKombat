using UnityEngine;

public class MoveFelix : MonoBehaviour
{
    public float speed = 5f; // Kecepatan berjalan
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Input dari keyboard (A dan D atau panah kiri/kanan)
        movement.x = Input.GetAxisRaw("Horizontal");

        // Kirim nilai ke Blend Tree melalui parameter 'Speed'
        animator.SetFloat("Speed", Mathf.Abs(movement.x));

        // Flip sprite jika arah berubah
        if (movement.x > 0)
        {
            spriteRenderer.flipX = false; // Jalan ke kanan
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = true; // Jalan ke kiri
        }
    }

    void FixedUpdate()
    {
        // Menggerakkan karakter
        rb.linearVelocity = new Vector2(movement.x * speed, rb.linearVelocity.y);
    }
}
