using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter {


    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;


    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;


    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;


    private void Update() {
        if (!IsServer) {
            return;
        }

        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax) {
            spawnPlateTimer = 0f;

            if (KitchenGameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax) {
                SpawnPlate();
                SpawnPlateClientRpc();
            }
        }
    }

    [ServerRpc]
    private void SpawnPlateServerRpc() {
        SpawnPlate();
        SpawnPlateClientRpc();
    }

    [ClientRpc]
    private void SpawnPlateClientRpc() {
        if (IsServer) return;
        SpawnPlate();
    }

    private void SpawnPlate() {
        platesSpawnedAmount++;

        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }

    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            // Player is empty handed
            if (platesSpawnedAmount > 0) {
                // There's at least one plate here
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

                InteractLogicServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() {
        InteractLogic();
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc() {
        if (IsServer) return;
        InteractLogic();
    }

    private void InteractLogic() {
        platesSpawnedAmount--;

        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }

}