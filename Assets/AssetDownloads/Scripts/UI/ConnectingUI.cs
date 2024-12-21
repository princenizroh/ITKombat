using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour {



    private void Start() {
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame += KitchenGameMultiplayer_OnTryingToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameManager_OnFailedToJoinGame;

        Hide();
    }

    private void KitchenGameManager_OnFailedToJoinGame(object sender, System.EventArgs e) {
        Hide();
    }

    private void KitchenGameMultiplayer_OnTryingToJoinGame(object sender, System.EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame -= KitchenGameMultiplayer_OnTryingToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameManager_OnFailedToJoinGame;
    }

}