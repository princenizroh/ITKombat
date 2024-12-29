using UnityEngine; 
using System.Collections;
using Unity.Netcode;
using System;

namespace ITKombat
{
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
        private float horizontalMove = 0f;

        private bool isBlocking = false;
        private bool isCrouching = false;
        private bool isWalking = false;
        private bool isWalkingSoundPlaying = false;
        public bool canMove = true;
        public bool jump = false;
        private string currentAnimationState = "";
        private float animationCooldown = 0.1f; // waktu cooldown animasi
        private float lastAnimationTime = 0f;

        private NetworkVariable<PlayerState> playerStateNetwork = 
            new NetworkVariable<PlayerState>(PlayerState.Idle, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Server);

        private enum PlayerState
        {
            Idle,
            Walking,
            Jumping,
            Blocking,
        }

        private void Start()
        {
            anim = GetComponent<Animator>();
            controller = GetComponent<CharacterController2D1>();

            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            }

            playerStateNetwork.OnValueChanged += OnPlayerStateChanged;
        }

        private void Update()
        {
            if (isWalkingSoundPlaying)
            {
                NewSoundManager.Instance.Footstep("Walk_Floor", transform.position);
            }
            if (IsOwner) return;
            if (!canMove)
            {
                horizontalMove = 0f;
                ChangeAnimationState("Idle");
                return;
            }

            if (isCrouching)
            {
                ChangeAnimationState("Crouch");
            }
            else if (!isCrouching && horizontalMove == 0 && !jump && !isDashing)
            {
                ChangeAnimationState("Idle");
            }
            else if (isWalking)
            {
                ChangeAnimationState("Walk");
            }

            if (isBlocking)
            {
                ChangeAnimationState("Block");
            }
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;

            if (!isDashing)
            {
                controller.Move(horizontalMove * Time.deltaTime, isCrouching, false);
            }
        }

        private void UpdateAnimation(PlayerState state)
        {
            string currentStateName = anim.GetCurrentAnimatorStateInfo(0).IsName(state.ToString()) ? state.ToString() : "";
            if (currentStateName == state.ToString()) return;
            switch (state)
            {
                case PlayerState.Idle:
                    anim.SetTrigger("Idle");
                    break;
                case PlayerState.Walking:
                    anim.SetTrigger("Walk");
                    break;
                case PlayerState.Jumping:
                    anim.SetTrigger("Jump");
                    break;
                case PlayerState.Blocking:
                    anim.SetTrigger("Block");
                    break;
            }
        }
        private void ChangeAnimationState(string newState)
        {
            if (currentAnimationState == newState || Time.time - lastAnimationTime < animationCooldown) return;

            anim.SetTrigger(newState);
            currentAnimationState = newState;
            lastAnimationTime = Time.time;
        }


        private void OnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            if (oldState == newState) return;
            Debug.Log($"[Client] State changed from {oldState} to {newState}");
            UpdateAnimation(newState);
        }

        [ServerRpc]
        private void UpdatePlayerStateServerRpc(PlayerState newState)
        {
            Debug.Log($"[Server] Updating state to {newState}");
            if (playerStateNetwork.Value != newState)
            {
                playerStateNetwork.Value = newState;
            }
        }

        public void OnMoveLeft()
        {
            if (!canMove || horizontalMove < 0) return;

            isWalking = true;
            horizontalMove = -moveSpeed;
            isWalkingSoundPlaying = true;
            UpdatePlayerStateServerRpc(PlayerState.Walking);
        }
        

        public void OnMoveRight()
        {
            if (!canMove || horizontalMove > 0) return;

            isWalking = true;
            horizontalMove = moveSpeed;
            isWalkingSoundPlaying = true;
            UpdatePlayerStateServerRpc(PlayerState.Walking);
        }

        public void OnStopMoving()
        {
            if (horizontalMove == 0) return;
            isWalking = false;
            horizontalMove = 0f;
            isWalkingSoundPlaying = false;
            UpdatePlayerStateServerRpc(PlayerState.Idle);
        }

        public void OnJump()
        {
            if (!canMove) return;

            jump = true;
            anim.SetTrigger("Jump");
            NewSoundManager.Instance.PlaySound("Jump", transform.position);
        }

        public void OnCrouchDown()
        {
            isCrouching = true;
            anim.SetTrigger("Crouch");
            NewSoundManager.Instance.PlaySound("Crouch", transform.position);
        }

        public void OnCrouchUp()
        {
            isCrouching = false;
            anim.SetTrigger("Idle");
        }

        public void OnBlockDown()
        {
            isBlocking = true;
            anim.SetTrigger("Block");
        }

        public void OnBlockUp()
        {
            isBlocking = false;
            anim.SetTrigger("Idle");
        }

        public void OnDash()
        {
            if (!canDash) return;

            StartCoroutine(Dash());
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

        private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
        {
            if (clientId == OwnerClientId)
            {
                Debug.Log("Server disconnected");
            }
        }
    }
}
