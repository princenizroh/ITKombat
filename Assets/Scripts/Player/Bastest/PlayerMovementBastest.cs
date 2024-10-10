using UnityEngine;

namespace ITKombat
{
    public class PlayerMovementBastest : MonoBehaviour
    {
        
        public CharacterController2D controller;
        private Animator anim;


        public float moveSpeed = 50f;
        float horizontalMove = 0f;
        bool jump = false;
        bool crouch = false;
        bool useKeyboardInput = true;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            if (useKeyboardInput)
            {
                horizontalMove = Input.GetAxisRaw("Horizontal") * moveSpeed;
                if (Input.GetButtonDown("Jump"))
                {
                    jump = true;
                }
                if (Input.GetButtonDown("Crouch"))
                {
                    crouch = true;
                }
                else if (Input.GetButtonUp("Crouch"))
                {
                    crouch = false;
                }
            }
        }

        private void FixedUpdate()
        {
            controller.Move(horizontalMove * Time.deltaTime, false, jump);
            jump = false;
        }

        //Method buat button
        public void OnMoveLeft()
        {
            useKeyboardInput = false;
            horizontalMove = -moveSpeed;
            anim.SetTrigger("Walk");
        }

        public void OnMoveRight()
        {
            useKeyboardInput = false;
            horizontalMove = moveSpeed;
            anim.SetTrigger("Walk");
        }

        public void OnStopMoving()
        {
            useKeyboardInput = false;
            horizontalMove = 0f;
            anim.SetTrigger("Idle");
        }

        public void OnJump() 
        {
            jump = true;
        }

        public void OnCrouchDown()
        {
            crouch = true;
        }

        public void OnCrouchUp()
        {
            crouch = false;
        }
    }
}
