using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

namespace ITKombat
{
    public class PlayerMovementNope : NetworkBehaviour
    {
        [SerializeField] private Transform spawnedObjectPrefab;
        private Transform spawnedObjectTransform;
        [SerializeField] private float movePower = 5f;
        [SerializeField] private float jumpPower = 5f;  // Added jump power
        public float dashSpeed;
        private bool moveLeft = false; 
        private bool moveRight = false;  
        private int direction = 1; 
        private bool isJumping = false; 
        private bool isGrounded = false; 
        private bool isDashing = false;
        private Animator anim;
        private Rigidbody2D rb;

        [SerializeField] private GameObject ground; 

        public static event EventHandler OnAnyPlayerSpawned;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>(); 
        }

        void Update()
        {
            if (!IsOwner) return;

            HandleInput();

            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            }
            Run(); 
        }

        private void HandleInput() // New method to handle input
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
                spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
            }

            if (Input.GetKeyDown(KeyCode.P) && spawnedObjectTransform != null)
            {
                Debug.Log("Destroying object");
                spawnedObjectTransform.GetComponent<NetworkObject>().Despawn(true);
                Destroy(spawnedObjectTransform.gameObject);
            }
        }

        private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
        {
            if (clientId == OwnerClientId)
            {
                Debug.Log("Server disconnected");
            }
        }

        private void Run()
        {
            if (isJumping || isDashing) return;  
            Vector3 moveVelocity = Vector3.zero;
            anim.ResetTrigger("Dash"); 

            if (moveLeft || Input.GetAxisRaw("Horizontal") < 0)
            {
                if (direction != -1)
                {
                    direction = -1;
                    Flip();
                }
                moveVelocity = Vector3.left;
                anim.SetTrigger("Walk"); 
            }

            if (moveRight || Input.GetAxisRaw("Horizontal") > 0)
            {
                if (direction != 1)
                {
                    direction = 1;
                    Flip();
                }
                moveVelocity = Vector3.right;
                anim.SetTrigger("Walk");
            }

            if (moveVelocity != Vector3.zero)
            {
                transform.position += moveVelocity * movePower * Time.deltaTime;
            }
            else if (isGrounded) 
            {
                anim.SetTrigger("Idle");
            }
        }

        private void Flip()
        {
            transform.localScale = new Vector3(direction, 1, 1);
        }

        // Button Input Methods for Movement
        public void MoveLeft()
        {
            if (!isJumping && !isDashing) anim.SetTrigger("Walk"); 
            moveLeft = true;
            direction = -1;
        }

        public void StopMoveLeft()
        {
            moveLeft = false;
            if (!isJumping && !isDashing) anim.SetTrigger("Idle"); 
        }

        public void MoveRight()
        {
            if (!isJumping && !isDashing) anim.SetTrigger("Walk"); 
            moveRight = true;
            direction = 1;
        }

        public void StopMoveRight()
        {
            moveRight = false;
            if (!isJumping && !isDashing) anim.SetTrigger("Idle"); 
        }

        public void Dash()
        {
            if (!isJumping && !isDashing)
            {
                isDashing = true;
                Vector3 dashVelocity = new Vector3(direction * dashSpeed, 0, 0);
                rb.linearVelocity = dashVelocity; 
                anim.SetTrigger("Dash"); 
                Debug.Log("Player dashed " + (direction == 1 ? "right" : "left"));
                
                StartCoroutine(EndDash());
            }
        }

        private IEnumerator EndDash()
        {
            yield return new WaitForSeconds(0.2f); 
            rb.linearVelocity = Vector2.zero; 
            isDashing = false;
            anim.SetTrigger("Idle");
        }

        public void JumpInput()
        {
            if (isGrounded && !isJumping)
            {
                Debug.Log("Player is jumping");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower); 
                anim.SetTrigger("Jump"); 
                isJumping = true;
                isGrounded = false;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject == ground) 
            {
                isGrounded = true;
                isJumping = false;
                anim.SetTrigger("Idle");
                Debug.Log("Player touched the ground, set to Idle");
            }
        }

        private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(
            new MyCustomData{
              _int = 56,
              _bool = true
            }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public override void OnNetworkSpawn()
        {
            randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
            {
                Debug.Log(OwnerClientId + " Random number changed from " + newValue._int + " to " + newValue._bool + "; " + newValue.message);
            };
        }

        [ServerRpc]
        private void TestServerRpc(ServerRpcParams serverRpcParams)
        {
            Debug.Log("Server RPC called by client: " + serverRpcParams.Receive.SenderClientId);
        }

        [ClientRpc]
        private void TestClientRpc(ClientRpcParams clientRpcParams)
        {
            Debug.Log("Client RPC called");
        }
    }
}
