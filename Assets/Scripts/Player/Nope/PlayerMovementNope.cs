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

        public static event EventHandler OnAnyPlayerSpawned;


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
        }

        private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
        {
            if (clientId == OwnerClientId)
            {
                Debug.Log("Server disconnected");
            }
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
