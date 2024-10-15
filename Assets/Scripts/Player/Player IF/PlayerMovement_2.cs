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
        private bool isCrouching = false;
        private bool isCrouchAttacking = false;
        bool useKeyboardInput = true;
        bool jump = false;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            if (useKeyboardInput)
            {
                horizontalMove = Input.GetAxisRaw("Horizontal") * moveSpeed;

                if (Input.GetButtonDown("Crouch"))
                {
                    OnCrouchDown();
                }
                else if (Input.GetButtonUp("Crouch"))
                {
                    OnCrouchUp();
                }
                // Handle dash input (e.g., double-tap right or press a dash key)
                if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
                {
                    StartCoroutine(Dash());
                }
            }

            if (isCrouching)
            {
                // Continuously trigger crouch animation while crouching
                anim.SetTrigger("Crouch");
            }
            else if (!isCrouching && horizontalMove == 0 && !jump && !isDashing)
            {
                anim.SetTrigger("Idle");
            }
        }

        private void FixedUpdate()
        {
            if (!isDashing)
            {
                controller.Move(horizontalMove * Time.deltaTime, isCrouching, jump);
            }
            jump = false;
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
        }

        public void OnJump()
        {
            jump = true;
            anim.SetTrigger("Jump");
        }

        public void OnCrouchDown()
        {
            isCrouching = true;
            anim.SetTrigger("Crouch");
            Debug.Log("Player is crouching");
        }

        public void OnCrouchUp()
        {
            isCrouching = false;
            anim.SetTrigger("Idle");
            Debug.Log("Player stopped crouching");
        }

        public void OnCrouchAttack()
        {
            if (isCrouching && !isCrouchAttacking)
            {
                Debug.Log("Player is performing a crouch attack");
                anim.SetTrigger("CrouchAttack");
                isCrouchAttacking = true;

                StartCoroutine(CrouchAttackCooldown(0.5f));
            }
        }

        public bool IsCrouching { get { return isCrouching; } }

        private IEnumerator CrouchAttackCooldown(float duration)
        {
            yield return new WaitForSeconds(duration);
            isCrouchAttacking = false;
        }

        public void OnDash()
        {
            if (canDash)
            {
                StartCoroutine(Dash());
            }
        }

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
    }
}
