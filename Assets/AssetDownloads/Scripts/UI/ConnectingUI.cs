using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour {



    private void Start() {
        Debug.Log("ConnectingUI Start");
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame += KitchenGameMultiplayer_OnTryingToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameManager_OnFailedToJoinGame;

        // Hide();
    }

    private void KitchenGameManager_OnFailedToJoinGame(object sender, System.EventArgs e) {
        Debug.Log("KitchenGameManager_OnFailedToJoinGame");
        Hide();
    }

    private void KitchenGameMultiplayer_OnTryingToJoinGame(object sender, System.EventArgs e) {
        Debug.Log("KitchenGameMultiplayer_OnTryingToJoinGame");
        Show();
    }

    private void Show() {
        Debug.Log("Show ConnectingUI");
        gameObject.SetActive(true);
    }

    private void Hide() {
        Debug.Log("Hide ConnectingUI");
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame -= KitchenGameMultiplayer_OnTryingToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameManager_OnFailedToJoinGame;
    }

}