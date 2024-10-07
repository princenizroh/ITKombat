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
        private bool moveLeft = false; 
        private bool moveRight = false;  
        private int direction = 1; 
        private Animator anim;

        public static event EventHandler OnAnyPlayerSpawned;

        private float dashTime = 0.2f; 
        public float dashSpeed = 15f; 
        private bool isDashing = false;
        private float lastDashTime = 0; 
        private int dashDirection = 0;

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

            if (!isDashing)
            {
                TestingKey();
                Run();
            }
            else
            {
                PerformDash();
            }
        }

        private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
        {
            if (clientId == OwnerClientId)
            {
                Debug.Log("Server disconnected");
            }
        }

        private void TestingKey()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                JumpInput();
                Debug.Log("Jump");
            }
            else
            {
                moveLeft = false;
                moveRight = false;
            }
        }

// Update the JumpInput method to be public
        public void JumpInput()
        {
            bool isGrounded = true; 
            if (isGrounded)
            {
                Debug.Log("Player is jumping");
            }
        }
        private void Run()
        {
            Vector3 moveVelocity = Vector3.zero;
            anim.SetBool("isRun", false);

            if (moveLeft || Input.GetAxisRaw("Horizontal") < 0)
            {
                direction = -1;
                moveVelocity = Vector3.left;

                transform.localScale = new Vector3(direction, 1, 1);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);
            }

            if (moveRight || Input.GetAxisRaw("Horizontal") > 0)
            {
                direction = 1;
                moveVelocity = Vector3.right;

                transform.localScale = new Vector3(direction, 1, 1);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);
            }

            transform.position += moveVelocity * movePower * Time.deltaTime;
        }

        // Button Inputs
        public void MoveLeft()
        {
            moveLeft = true;
            direction = -1;
        }

        public void StopMoveLeft()
        {
            moveLeft = false;
        }

        public void MoveRight()
        {
            moveRight = true;
            direction = 1;
        }

        public void StopMoveRight()
        {
            moveRight= false;
        }

        // Dash mechanics
        public void Dash(int direction)
        {
            float dashSpeed = 10f; 
            Vector3 dashVelocity = new Vector3(direction * dashSpeed, 0, 0);
            transform.position += dashVelocity * Time.deltaTime;
            Debug.Log("Player dashed " + (direction == 1 ? "right" : "left"));
        }

        private void PerformDash()
        {
            float dashDuration = Time.time - lastDashTime;
            if (dashDuration < dashTime)
            {
                Vector3 dashVelocity = new Vector3(dashDirection * dashSpeed, 0, 0);
                transform.position += dashVelocity * Time.deltaTime;
            }
            else
            {
                isDashing = false; 
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
                Debug.Log(OwnerClientId + "Random number changed from " + newValue._int + " to " + newValue._bool + ";" + newValue.message);
            };
        }

        [ServerRpc]
        private void TestServerRpc(ServerRpcParams serverRpcParams)
        {
            Debug.Log("Server RPC called" + OwnerClientId + ";" + serverRpcParams.Receive.SenderClientId);
        }
        
        [ClientRpc]
        private void TestClientRpc(ClientRpcParams clientRpcParams)
        {
            Debug.Log("Client RPC called");
        }
    }
}
