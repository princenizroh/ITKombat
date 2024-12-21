using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour {


    [SerializeField] private KitchenObjectSO kitchenObjectSO;


    private IKitchenObjectParent kitchenObjectParent;
    private FollowTransform followTransform;


    protected virtual void Awake() {
        followTransform = GetComponent<FollowTransform>();
    }

    public KitchenObjectSO GetKitchenObjectSO() {
        return kitchenObjectSO;
    }

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) {
        SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
        //SetKitchenObjectParent(kitchenObjectParent.GetNetworkObject());
        //SetKitchenObjectParentClientRpc(kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
        SetKitchenObjectParent(kitchenObjectParentNetworkObjectReference);
        SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
    }

    [ClientRpc]
    public void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
        if (IsServer) return;
        SetKitchenObjectParent(kitchenObjectParentNetworkObjectReference);
    }

    public void SetKitchenObjectParent(NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        if (this.kitchenObjectParent != null) {
            this.kitchenObjectParent.ClearKitchenObject();
        }

        this.kitchenObjectParent = kitchenObjectParent;

        if (kitchenObjectParent.HasKitchenObject()) {
            Debug.LogError("IKitchenObjectParent already has a KitchenObject!");
        }

        kitchenObjectParent.SetKitchenObject(this);

        followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
    }

    public IKitchenObjectParent GetKitchenObjectParent() {
        return kitchenObjectParent;
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }

    public void ClearKitchenObjectOnParent() {
        kitchenObjectParent.ClearKitchenObject();
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
        if (this is PlateKitchenObject) {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        } else {
            plateKitchenObject = null;
            return false;
        }
    }



    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
        KitchenGameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectSO, kitchenObjectParent);
    }

    public static void DestroyKitchenObject(KitchenObject kitchenObject) {
        KitchenGameMultiplayer.Instance.DestroyKitchenObject(kitchenObject);
    }

}