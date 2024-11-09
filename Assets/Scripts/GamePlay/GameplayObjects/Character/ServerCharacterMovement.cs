using UnityEngine;
using System.Collections;
using Unity.Netcode;
using System;
using Unity.Collections;
using NUnit.Framework.Internal;
namespace ITKombat
{
    public class ServerCharacterMovement : NetworkBehaviour
    {
        // private PlayerController2D controller;
        private CharacterController2D1 controller;
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
        private void Start()
        {
            anim = GetComponent<Animator>();
            // controller = GetComponent<PlayerController2D>();  // Ubah kode ini untuk flip yang udah benar menggunakan spriterenderer
            controller = GetComponent<CharacterController2D1>();
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            }
        }

        private void Update()
        {
            if (!IsOwner) return;

            TestingKey();
            

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
        private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
        {
            if (clientId == OwnerClientId)
            {
                Debug.Log("Server disconnected");
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
        private void TestingKey()
        { 
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

                if (Input.GetKey(KeyCode.Space))
                {
                    OnJump();
                }

                if (Input.GetKey(KeyCode.E))
                {
                    OnBlockDown();
                }
                else
                {
                    OnBlockUp();
                }
               
            }
        }
        public void OnMoveLeft()
        {
            if (canMove)
                useKeyboardInput = false;
                horizontalMove = -moveSpeed;
                anim.SetTrigger("Walk");
        }

        public void OnMoveRight()
        {
            if (canMove)
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
        }

        public void OnCrouchUp()
        {
            isCrouching = false;
            anim.SetTrigger("Idle");
        }
        // Metode untuk mulai block
        public void OnBlockDown()
        {
            isBlocking = true;
            anim.SetTrigger("Block"); // Memicu animasi Block
        }

        public void OnBlockUp()
        {
            isBlocking = false;
            anim.SetTrigger("Idle"); // Kembali ke animasi Idle setelah block selesai
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
