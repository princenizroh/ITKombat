using UnityEngine;

namespace ITKombat{
    public class MoveFelix : MonoBehaviour
{
    public float moveSpeed = 5f; // Kecepatan berjalan
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Vector2 movement;
    private bool moveLeft = false; // Menyimpan apakah tombol kiri ditekan
    private bool moveRight = false; // Menyimpan apakah tombol kanan ditekan
    public bool canMove = true;
    float horizontalMove = 0f;
    private bool isWalkingSoundPlaying = false;

        void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Reset movement setiap frame
        // movement.x = 0;
        // if (!canMove)
        // {
        //     horizontalMove = 0f;  // Player can't move if canMove is false
        //     return;
        // }

        // Jika tombol kiri ditekan, bergerak ke kiri
        if (moveLeft)
        {
            movement.x = -1;
            SoundManager.Instance.PlaySound3D("WalkFloor", transform.position);
            animator.SetTrigger("isWalk"); // Set IsMoving ke true saat bergerak
            Debug.Log("Waling left");
        }
        // Jika tombol kanan ditekan, bergerak ke kanan
        else if (moveRight)
        {
            movement.x = 1;
            SoundManager.Instance.PlaySound3D("WalkFloor", transform.position);
            animator.SetTrigger("isWalk"); // Set IsMoving ke true saat bergerak
        }
        else
        {
            animator.SetTrigger("isIdle"); // Set IsMoving ke false jika tidak bergerak
        }


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
    public void OnMoveLeft()
        {
            if (moveLeft)
            {
                isWalkingSoundPlaying = true;
                horizontalMove = -moveSpeed;
                animator.SetTrigger("isWalk");
            }
        }
    public void OnMoveRight()
    {
        if (moveRight)
        {
            isWalkingSoundPlaying = true;
            horizontalMove = moveSpeed;
            animator.SetTrigger("isWalk");
        }
    }
    public void OnStopMoving()
    {
        isWalkingSoundPlaying = false;
        horizontalMove = 0f;
        animator.SetTrigger("isIdle");
    }

    // void FixedUpdate()
    // {
    //     // Menggerakkan karakter
    //     rb.linearVelocity = new Vector2(movement.x * speed, rb.linearVelocity.y);
    // }

    // Method untuk UI Button - Dipanggil saat tombol kiri ditekan
    public void OnMoveLeftDown()
    {
        Debug.Log("Move Left Down");
        moveLeft = true;
    }

    public void OnMoveLeftUp()
    {
        Debug.Log("Move Left Up");
        moveLeft = false;
    }

    public void OnMoveRightDown()
    {
        Debug.Log("Move Right Down");
        moveRight = true;
    }

    public void OnMoveRightUp()
    {
        Debug.Log("Move Right Up");
        moveRight = false;
    }
}

}
