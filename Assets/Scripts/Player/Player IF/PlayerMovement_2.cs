using UnityEngine;
using System.Collections;

namespace ITKombat
{
    public class PlayerMovement_2 : MonoBehaviour
    {
        public CharacterController2D1 controller;
        private Animator anim;

        public float dashSpeed = 50f;
        public float dashDuration = 0.2f;
        public float dashCooldown = 1f;
        private bool canDash = true;
        private bool isDashing = false;

        public float moveSpeed = 50f;
        float horizontalMove = 0f;
        private bool isBlocking = false;
        private bool isCrouching = false;
        private bool isCrouchAttacking = false;
        bool useKeyboardInput = true;
        bool jump = false;
        public bool canMove = true;

        private bool isWalkingSoundPlaying = false;
        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            if(isWalkingSoundPlaying)
            {
                SoundManager.Instance.PlaySound3D("WalkFloor", transform.position);
            }

            if (canMove && useKeyboardInput)
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

            if (!canMove)
            {
                horizontalMove = 0f;  // Player can't move if canMove is false
                return;
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
            
            if (isBlocking)
            {
                // Continuously trigger block animation while blocking
                anim.SetTrigger("Block");
            }
        }
        
        private void FixedUpdate()
        {
            if (canMove && !isDashing)
            {
                controller.Move(horizontalMove * Time.deltaTime, isCrouching, jump);
            }
            jump = false;
        }


        public void OnMoveLeft()
        {
            if (canMove)
            {
                useKeyboardInput = false;
                horizontalMove = -moveSpeed;
                anim.SetTrigger("Walk");
                SoundManager.Instance.PlaySound3D("WalkFloor", transform.position);
            }

        }

        public void OnMoveRight()
        {
            if (canMove)
            {
                useKeyboardInput = false;
                horizontalMove = moveSpeed;
                anim.SetTrigger("Walk");
                SoundManager.Instance.PlaySound3D("WalkFloor", transform.position);
            }

        }

        public void OnStopMoving()
        {
            isWalkingSoundPlaying = false;
            useKeyboardInput = false;
            horizontalMove = 0f;
            anim.SetTrigger("Idle");
        }

        public void OnJump()
        {
            jump = true;
            anim.SetTrigger("Jump");
            SoundManager.Instance.PlaySound3D("Jump", transform.position);
        }

        public void OnCrouchDown()
        {
            isCrouching = true;
            anim.SetTrigger("Crouch");
            SoundManager.Instance.PlaySound3D("Crouch", transform.position);
            Debug.Log("Player is crouching");
        }

        public void OnCrouchUp()
        {
            isCrouching = false;
            anim.SetTrigger("Idle");
            Debug.Log("Player stopped crouching");
        }
        // Metode untuk mulai block
        public void OnBlockDown()
        {
            isBlocking = true;
            anim.SetTrigger("Block"); // Memicu animasi Block
            Debug.Log("Player is blocking");
        }

        public void OnBlockUp()
        {
            isBlocking = false;
            anim.SetTrigger("Idle"); // Kembali ke animasi Idle setelah block selesai
            Debug.Log("Player stopped blocking");
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
            SoundManager.Instance.PlaySound3D("Dash", transform.position);

            float dashDirection = controller.m_FacingRight ? 1f : -1f;
            controller.Dash(dashSpeed * dashDirection, dashDuration);

            yield return new WaitForSeconds(dashDuration);

            isDashing = false;

            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }
    }
}
