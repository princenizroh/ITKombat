using UnityEngine;
using System.Collections;

namespace ITKombat
{
    public class PlayerMovement_2 : MonoBehaviour
    {
        public CharacterController2D1 controller;
        private Animator anim;

        public float dashSpeed = 5f;
        public float dashDuration = 0.2f;
        public float dashCooldown = 1f;
        private bool canDash = true;
        private bool isDashing = false;

        public float moveSpeed = 50f;
        float horizontalMove = 0f;
        private float idleThreshold = 0.1f;
        private float idleTimer = 0f;
        private bool isAttacking = false;
        bool jump = false;
        bool crouch = false;
        bool useKeyboardInput = true;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        [System.Obsolete]
        private void Update()
    {
        if (useKeyboardInput)
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * moveSpeed;

            if (horizontalMove == 0 && !jump && !crouch && !isDashing)
            {
                idleTimer += Time.deltaTime; 
                if (idleTimer >= idleThreshold)
                {
                    anim.SetTrigger("Idle");
                }
            }
            else
            {
                idleTimer = 0f; 
            }

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

        [System.Obsolete]
        private void FixedUpdate()
        {
            if (!isDashing)
            {
                controller.Move(horizontalMove * Time.deltaTime, crouch, jump);
            }
            jump = false;
        }

        [System.Obsolete]
        private IEnumerator Dash()
        {
            isDashing = true;
            canDash = false;
            anim.SetTrigger("Dash");

            float dashDirection = controller.m_FacingRight ? 1f : -1f;
            controller.Dash(dashSpeed * dashDirection, dashDuration);

            yield return new WaitForSeconds(dashDuration);

            isDashing = false;

            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }

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
            idleTimer = 0f;
        }

        public void OnJump() 
        {
            jump = true;
            anim.SetTrigger("Jump");
        }

        public void OnCrouchDown()
        {
            crouch = true;
            useKeyboardInput = false;
            anim.SetTrigger("Crouch");
        }

        public void OnCrouchUp()
        {
            crouch = false;
            useKeyboardInput = false;
            anim.SetTrigger("Idle");
        }

        [System.Obsolete]
        public void OnDash()
        {
            if (canDash)
            {
                StartCoroutine(Dash());
            }
        }
    }
}
