using UnityEngine;

namespace ITKombat
{
    public class Move : MonoBehaviour
    {
        private Animator anim;

        public float moveSpeed = 50f;
        private float horizontalMove = 0f;
        private Vector2 movement;
        private bool useKeyboardInput = true;
        private Rigidbody2D rb;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            // Pergerakan menggunakan input dari keyboard
            if (useKeyboardInput)
            {
                horizontalMove = Input.GetAxisRaw("Horizontal") * moveSpeed;

                // Set animasi berdasarkan pergerakan
                if (horizontalMove != 0)
                {
                    anim.SetTrigger("Walk");
                }
                else
                {
                    anim.SetTrigger("Idle");
                }
            }
        }

        private void FixedUpdate()
        {
            // Panggil fungsi Move pada controller untuk memindahkan karakter
            rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);
        }

        // Fungsi untuk menggerakkan ke kiri melalui tombol UI
        public void OnMoveLeft()
        {
            useKeyboardInput = false;
            horizontalMove = -moveSpeed;
            anim.SetTrigger("Walk");
        }

        // Fungsi untuk menggerakkan ke kanan melalui tombol UI
        public void OnMoveRight()
        {
            useKeyboardInput = false;
            horizontalMove = moveSpeed;
            anim.SetTrigger("Walk");
        }

        // Fungsi untuk menghentikan pergerakan melalui tombol UI
        public void OnStopMoving()
        {
            useKeyboardInput = false;
            horizontalMove = 0f;
            anim.SetTrigger("Idle");
        }
    }
}
