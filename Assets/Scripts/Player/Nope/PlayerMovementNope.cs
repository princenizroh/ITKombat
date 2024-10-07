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
        public float dashSpeed;
        private bool moveLeft = false; 
        private bool moveRight = false;  
        private int direction = 1; 
        private Animator anim;
        private Rigidbody2D rb;

        public static event EventHandler OnAnyPlayerSpawned;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>(); 
        }

        void Update()
        {
            if (!IsOwner) return;

            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            }

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
            
            Run();
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
            Vector3 moveVelocity = Vector3.zero;
            anim.ResetTrigger("Dash"); 
            anim.ResetTrigger("Jump"); 
            anim.SetTrigger("Walk"); 

            if (moveLeft || Input.GetAxisRaw("Horizontal") < 0)
            {
                direction = -1;
                moveVelocity = Vector3.left;

                transform.localScale = new Vector3(direction, 1, 1);
            }

            if (moveRight || Input.GetAxisRaw("Horizontal") > 0)
            {
                direction = 1;
                moveVelocity = Vector3.right;

                transform.localScale = new Vector3(direction, 1, 1);
            }

            // Ensure player moves only when there's velocity
            if (moveVelocity != Vector3.zero)
            {
                transform.position += moveVelocity * movePower * Time.deltaTime;
            }
        }

        // Button Input Methods for Movement
        public void MoveLeft()
        {
            anim.SetTrigger("Walk"); 
            moveLeft = true;
            direction = -1;
        }

        public void StopMoveLeft()
        {
            moveLeft = false;
        }

        public void MoveRight()
        {
            anim.SetTrigger("Walk"); 
            moveRight = true;
            direction = 1;
        }

        public void StopMoveRight()
        {
            moveRight = false;
        }

        public void Dash()
        {
            Vector3 dashVelocity = new Vector3(direction * dashSpeed, 0, 0);
            rb.linearVelocity = dashVelocity; // Corrected from linearVelocity to velocity
            Debug.Log("Player dashed " + (direction == 1 ? "right" : "left"));
            anim.SetTrigger("Dash"); 
        }

        public void JumpInput()
        {
            bool isGrounded = true; // Placeholder for ground check logic
            if (isGrounded)
            {
                Debug.Log("Player is jumping");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 5f); 
                anim.SetTrigger("Jump"); 
            }
        }

        // Check for ground collision
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                // Player is on the ground, set animation to Idle
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
