using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectUI : MonoBehaviour {


    [SerializeField] private Button playAgainButton;


    private void Awake() {
        playAgainButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start() {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

        Hide();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        if (clientId == NetworkManager.ServerClientId) {
            // Server is shutting down
            Show();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

}