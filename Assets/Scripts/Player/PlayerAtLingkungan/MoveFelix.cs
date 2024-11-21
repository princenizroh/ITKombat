using UnityEngine;

namespace ITKombat
{
    public class MoveFelix : MonoBehaviour
    {
        public float speed = 4f; // Kecepatan berjalan
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Animator animator;

        private Vector2 movement;
        private bool moveLeft = false; // Menyimpan apakah tombol kiri ditekan
        private bool moveRight = false; // Menyimpan apakah tombol kanan ditekan

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            // Reset movement setiap frame
            movement.x = 0;

            // Jika tombol kiri ditekan, bergerak ke kiri
            if (moveLeft)
            {
                movement.x = -1;
                spriteRenderer.flipX = true; // Menghadap kiri
                NewSoundManager.Instance.PlaySound("Walk_Floor", transform.position);
                Debug.Log("Move Left: Sprite flipped to left");
            }

            // Jika tombol kanan ditekan, bergerak ke kanan
            else if (moveRight)
            {
                movement.x = 1;
                spriteRenderer.flipX = false; // Menghadap kanan
                NewSoundManager.Instance.PlaySound("Walk_Floor", transform.position);
                Debug.Log("Move Right: Sprite flipped to right");
            }

            // Kirim nilai ke Blend Tree melalui parameter 'Speed'
            animator.SetFloat("Speed", Mathf.Abs(movement.x));
        }

        void FixedUpdate()
        {
            // Menggerakkan karakter
            rb.linearVelocity = new Vector2(movement.x * speed, rb.linearVelocity.y);
        }

        // Method untuk UI Button - Dipanggil saat tombol kiri ditekan
        public void OnMoveLeftDown()
        {
            Debug.Log("Move Left Down");
            moveLeft = true;
            animator.SetTrigger("isWalk");
        }

        public void OnMoveLeftUp()
        {
            Debug.Log("Move Left Up");
            moveLeft = false;
            animator.SetTrigger("isIdle");
        }

        public void OnMoveRightDown()
        {
            Debug.Log("Move Right Down");
            moveRight = true;
            animator.SetTrigger("isWalk");
        }

        public void OnMoveRightUp()
        {
            Debug.Log("Move Right Up");
            moveRight = false;
            animator.SetTrigger("isIdle");
        }
    }
}
