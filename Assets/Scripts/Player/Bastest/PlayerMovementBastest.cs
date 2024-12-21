using System.Collections;
using UnityEngine;

namespace ITKombat
{
    public class PlayerMovementBastest : MonoBehaviour
    {
        
        public CharacterController2D controller;
        private Animator anim;

        public float dashSpeed = 15f;
        public float dashDuration = 0.2f;
        public float dashCooldown = 1f;
        private bool canDash = true;
        private bool isDashing = false;

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

                // Handle dash input (e.g., double-tap right or press a dash key)
                if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
                {
                    StartCoroutine(Dash());
                }
            }
        }

        private void FixedUpdate()
        {
            if (!isDashing)
            {
                controller.Move(horizontalMove * Time.deltaTime, crouch, jump);
            }
            jump = false; ;
        }

        private IEnumerator Dash()
        {
            isDashing = true;
            canDash = false;
            anim.SetTrigger("Dash");

            // Call the controller's dash method
            float dashDirection = controller.m_FacingRight ? 1f : -1f;
            controller.Dash(dashSpeed * dashDirection, dashDuration);

            yield return new WaitForSeconds(dashDuration);

            isDashing = false;

            // Cooldown before the player can dash again
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
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
            Debug.Log("The " + gameObject.name + " is crouching");
            crouch = true;
        }

        public void OnCrouchUp()
        {
            Debug.Log("The " + gameObject.name + " stopped crouching");
            crouch = false;
        }

        public void OnDash()
        {
            if (canDash)
            {
                StartCoroutine(Dash());
            }
        }
    }
}
