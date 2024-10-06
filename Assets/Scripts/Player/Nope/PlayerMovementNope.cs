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
        private Rigidbody2D player;
        public float moveSpeed;
        public float direction;
        private bool moveLeft, moveRight;
        private bool canJump = true;
        public bool crouch;
        public float jumpForce;
        public float jumpCooldown ;
        public float movePower;
        
        //raycast
        bool grounded;
        public Vector2 boxSize;
        public float castDistance;
        public LayerMask groundLayer;

        private Animator anim;
        //inputSystem script
        PlayerControls controls;

        public static event EventHandler OnAnyPlayerSpawned;

        private void Awake()
        {
            controls = new PlayerControls();
            controls.Enable();

            controls.Player.Move.performed += info =>
            {
                direction = info.ReadValue<float>();
            };
        }

        void Start()
        {
            player = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            if (!IsOwner) return;
            TestingKey();
            Run();
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
                spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
                // TestClientRpc(new ClientRpcParams {
                //     Send = new ClientRpcSendParams {
                //         TargetClientIds = new List<ulong>{ 1 }}
                //     });
                // TestServerRpc(new ServerRpcParams());
                // randomNumber.Value = new MyCustomData
                // {
                //     _int = 10,
                //     _bool = false,
                //     message = "Hello World"
                // };
                //
                if (Input.GetKeyDown(KeyCode.P))
                {
                    Debug.Log("Destroying object");
                    spawnedObjectTransform.GetComponent<NetworkObject>().Despawn(true);
                    Destroy(spawnedObjectTransform.gameObject);
                }
            }
            player.linearVelocity = new Vector2(direction * moveSpeed * Time.deltaTime, player.linearVelocity.y);


            if (moveLeft)
            {
                transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            }

            if (moveRight)
            {
                transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            }

            if (Mathf.Abs(player.linearVelocity.y) < 0.1f)
            {
                canJump = true;
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
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                crouch = true;
                Debug.Log("Crouch");
            }
            else
            {
                moveLeft = false;
                moveRight = false;
                crouch = false;
            }
        }

        void Run()
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
                if (!anim.GetBool("isJump"))anim.SetBool("isRun", true);
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


        private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(
            new MyCustomData{
              _int = 56,
              _bool = true
            }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public override void OnNetworkSpawn()
        {
            randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
            {
                Debug.Log(OwnerClientId + "Random number changed from " + newValue._int+ " to " + newValue._bool + ";" + newValue.message);
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
            Debug.Log("s");
        }

        public struct MyCustomData : INetworkSerializable
        {
            public int _int;
            public bool _bool;
            public FixedString128Bytes message;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref _int);
                serializer.SerializeValue(ref _bool);
                serializer.SerializeValue(ref message);
            }
        }

        public void JumpInput()
        {
            if (canJump && Mathf.Abs(player.linearVelocity.y) < 0.1f && grounded)
            {
                player.linearVelocity = new Vector2(player.linearVelocity.x, jumpForce);
                StartCoroutine(JumpCooldown());
            }
        }

        IEnumerator JumpCooldown()
        {
            canJump = false;
            yield return new WaitForSeconds(jumpCooldown);
            canJump = true; 
        }

        public bool isGrounded()
        {
            if(Physics2D.BoxCast(transform.position, boxSize,0,-transform.up,castDistance,groundLayer)){
                Debug.Log("ground");
                return true;
            }
            else{
                return false;
            }
        }
        private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position-transform.up * castDistance, boxSize);
        }
    }
}
