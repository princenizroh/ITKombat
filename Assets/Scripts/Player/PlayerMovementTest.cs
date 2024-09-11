using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
namespace ITKombat
{
    public class PlayerMovementTest : NetworkBehaviour
    {
        [SerializeField] private Transform spawnedObjectPrefab;
        private Transform spawnedObjectTransform;
        private Rigidbody2D player;
        public float jumpForce = 5f;
        public float moveSpeed = 100;
        public float direction;
        private bool moveLeft, moveRight;
        private bool canJump = true; //jump 1x
        //inputSystem script
        PlayerControls controls;

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
        }

        void Update()
        {
            if (!IsOwner) return;

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
            player.velocity = new Vector2(direction * moveSpeed * Time.deltaTime, player.velocity.y);

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                moveLeft = true;
                direction = -1;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                moveRight = true;
                direction = 1;
            }
            else
            {
                moveLeft = false;
                moveRight = false;
            }
            if (moveLeft)
            {
                transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            }

            if (moveRight)
            {
                transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            }

            if (Mathf.Abs(player.velocity.y) < 0.1f)
            {
                canJump = true;
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
            if (canJump)
            {
                player.velocity = new Vector2(player.velocity.x, jumpForce);
                canJump = false;
            }
        }

        public void LeftInputButtonDown()
        {
            //kiri
            moveLeft = true;
            direction = -1;
        }

        public void LeftInputButtonUp() 
        {
            moveLeft = false;
        }

        public void RightInputButtonDown()
        {
            //kanan
            moveRight = true;
            direction = 1;
        }

        public void RightInputButtonUp() 
        {
            moveRight = false;
        
        }
    }
}
