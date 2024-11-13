using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrashCounter : BaseCounter {


    public static event EventHandler OnAnyObjectTrashed;

    new public static void ResetStaticData() {
        OnAnyObjectTrashed = null;
    }



    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {
            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());

            InteractLogicServerRpc();
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
        OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
    }

}