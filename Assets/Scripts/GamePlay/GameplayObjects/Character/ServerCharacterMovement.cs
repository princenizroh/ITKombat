using UnityEngine; 
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using System;
using Unity.Collections;

namespace ITKombat
{
            public enum AnimationState
        {
            Idle = 0,
            Walk = 1,
            Block = 2,
            Crouch = 3,
            CrouchAttack = 4,
            Dash = 5,
        }
    public class ServerCharacterMovement : NetworkBehaviour
    {
        public static ServerCharacterMovement LocalInstance { get; private set; }
        private CharacterController2D1 controller;
        private Animator anim;

        public float dashSpeed = 50f;
        public float dashDuration = 0.2f;
        public float dashCooldown = 1f;
        private bool canDash = true;
        private bool isDashing = false;

        [SerializeField] private float moveSpeed = 25f;
        float horizontalMove = 0f;
        
        
        private bool isBlocking = false;
        private NetworkVariable<bool> isBlockingNetwork = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private bool isCrouching = false;
        private bool isCrouchAttacking = false;
        bool useKeyboardInput = true;
        bool jump = false;
        public bool canMove = true;
        private bool isWalking = false;
        private bool isWalkingSoundPlaying = false;

        // Menggunakan satu NetworkVariable untuk menyimpan state animasi
        private NetworkVariable<AnimationState> animationStateNetwork = new NetworkVariable<AnimationState>(AnimationState.Idle, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


        private void Start()
        {
            anim = GetComponent<Animator>();
            controller = GetComponent<CharacterController2D1>();

            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            }
        
        }
        

        private void Update()
        {
            if (isWalkingSoundPlaying)
            {
                NewSoundManager.Instance.Footstep("Walk_Floor", transform.position);
            }
            if (!IsOwner) return;
    
            if (!canMove)
            {
                horizontalMove = 0f;
                return;
            }
            if (IsHost)
            {
                if (Input.GetKey(KeyCode.E))
                {
                    OnBlockDown();
                }
                else if (Input.GetKeyUp(KeyCode.E))
                {
                    OnBlockUp();
                }
            }

            if (IsClient)
            {
                if (Input.GetKey(KeyCode.R))
                {
                    OnBlockDown();
                }
                else if (Input.GetKeyUp(KeyCode.R))
                {
                    OnBlockUp();
                }
            }

            if (isCrouching)
            {
                anim.SetTrigger("Crouch");
            }
            else if (!isCrouching && horizontalMove == 0 && !jump && !isDashing)
            {
                anim.SetTrigger("Idle");
            }

            if (isBlocking)
            {
                anim.SetTrigger("Block");
            }

            if (isWalking)
            {
                anim.SetTrigger("Walk");
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsOwner)
            {
                LocalInstance = this;
            }
            isBlockingNetwork.OnValueChanged += OnIsBlockingChanged;
            animationStateNetwork.OnValueChanged += OnAnimationStateChanged;

            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            }

            
        }

        private void OnIsBlockingChanged(bool oldValue, bool newValue)
        {
            // Sinkronisasi animasi block berdasarkan perubahan nilai di network
            if (newValue)
            {
                SetAnimationStateServerRpc(AnimationState.Block);
            }
            else
            {
                SetAnimationStateServerRpc(AnimationState.Idle);
            }
        }
            private void OnAnimationStateChanged(AnimationState oldState, AnimationState newState)
            {
                // Terapkan animasi berdasarkan state
                switch (newState)
                {
                    case AnimationState.Idle:
                        anim.SetTrigger("Idle");
                        break;
                    case AnimationState.Walk:
                        anim.SetTrigger("Walk");
                        break;
                    case AnimationState.Block:
                        anim.SetTrigger("Block");
                        break;
                    case AnimationState.Crouch:
                        anim.SetTrigger("Crouch");
                        break;
                    case AnimationState.CrouchAttack:
                        anim.SetTrigger("CrouchAttack");
                        break;
                    case AnimationState.Dash:
                        anim.SetTrigger("Dash");
                        break;
                }
            }
            [ServerRpc]
            private void SetAnimationStateServerRpc(AnimationState state)
            {
                if (animationStateNetwork.Value != state)
                {
                    animationStateNetwork.Value = state;
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
            if (!IsOwner) return;
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
                isWalking = true;
                useKeyboardInput = false;
                horizontalMove = -moveSpeed;
                isWalkingSoundPlaying = true;
                SetAnimationStateServerRpc(AnimationState.Walk); // Walk
            }
        }

        public void OnMoveRight()
        {
            if (canMove)
            {
                isWalking = true;
                useKeyboardInput = false;
                horizontalMove = moveSpeed;
                isWalkingSoundPlaying = true;
                SetAnimationStateServerRpc(AnimationState.Walk); // Walk
            }
        }

        public void OnStopMoving()
        {
            isWalking = false;
            isWalkingSoundPlaying = false;
            useKeyboardInput = false;
            horizontalMove = 0f;
            SetAnimationStateServerRpc(AnimationState.Idle); // Idle
        }

        public void OnJump()
        {
            jump = true;
            anim.SetTrigger("Jump");
            NewSoundManager.Instance.PlaySound("Jump", transform.position);
        }

        public void OnCrouchDown()
        {
            isCrouching = true;
            anim.SetTrigger("Crouch");
            NewSoundManager.Instance.PlaySound("Crouch", transform.position);
            Debug.Log("Player is crouching");
        }

        public void OnCrouchUp()
        {
            isCrouching = false;
            anim.SetTrigger("Idle");
            Debug.Log("Player stopped crouching");
        }

        public void OnBlockDown()
        {
            if (!IsOwner) return;

            isBlocking = true;
            SetBlockingStateServerRpc(true);
            SetAnimationStateServerRpc(AnimationState.Block);
        }

        public void OnBlockUp()
        {
            if (!IsOwner) return;

            isBlocking = false;
            SetBlockingStateServerRpc(false);
            SetAnimationStateServerRpc(AnimationState.Idle);
        }

        [ServerRpc]
        private void SetBlockingStateServerRpc(bool state)
        {
            isBlockingNetwork.Value = state; // Update state di server
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
            NewSoundManager.Instance.PlaySound("Dash", transform.position);

            float dashDirection = controller.m_FacingRight ? 1f : -1f;
            controller.Dash(dashSpeed * dashDirection, dashDuration);

            yield return new WaitForSeconds(dashDuration);

            isDashing = false;

            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }
    }
}
