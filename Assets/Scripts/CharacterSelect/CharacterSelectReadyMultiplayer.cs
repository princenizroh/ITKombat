using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Multiplay;
using UnityEngine;


namespace ITKombat
{
    public class CharacterSelectReadyMultiplayer : NetworkBehaviour
    {
        public static event EventHandler OnInstanceCreated;

        public static void ResetStaticData() {
            OnInstanceCreated = null;
        }


        public static CharacterSelectReadyMultiplayer Instance { get; private set; }




        public event EventHandler OnReadyChanged;
        public event EventHandler OnGameStarting;


        private Dictionary<ulong, bool> playerReadyDictionary;


        private void Awake() {
            Instance = this;

            playerReadyDictionary = new Dictionary<ulong, bool>();

            OnInstanceCreated?.Invoke(this, EventArgs.Empty);
        }

        private async void Start() {
    #if DEDICATED_SERVER
            Debug.Log("DEDICATED_SERVER CHARACTER SELECT");

            Debug.Log("ReadyServerForPlayersAsync");
            await MultiplayService.Instance.ReadyServerForPlayersAsync();

            Camera.main.enabled = false;
    #endif
        }


        public void SetPlayerReady() {
            SetPlayerReadyServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
            SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);

            Debug.Log("SetPlayerReadyServerRpc " + serverRpcParams.Receive.SenderClientId);
            playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

            bool allClientsReady = true;
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
                if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId]) {
                    // This player is NOT ready
                    allClientsReady = false;
                    break;
                }
            }

            if (allClientsReady) {
                // OnGameStarting?.Invoke(this, EventArgs.Empty);
                Debug.Log("All players are ready. Starting countdown.");
                LobbyRoomManager.Instance.DeleteLobby();
                Debug.Log("Lobby Deleted");

                Loader.LoadNetwork(Loader.Scene.ClientTest);
            }
        }

        [ClientRpc]
        private void SetPlayerReadyClientRpc(ulong clientId) {
            playerReadyDictionary[clientId] = true;

            OnReadyChanged?.Invoke(this, EventArgs.Empty);
        }


        public bool IsPlayerReady(ulong clientId) {
            return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
        }
    }
}
