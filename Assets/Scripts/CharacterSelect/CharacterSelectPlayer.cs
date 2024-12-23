using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
namespace ITKombat
{
    public class CharacterSelectPlayer : MonoBehaviour
    {    
        [SerializeField] private int playerIndex;
        [SerializeField] private GameObject readyGameObject;
        [SerializeField] private CharacterSelectVisual characterSelectVisual;
        // [SerializeField] private PlayerVisual playerVisual;
        // [SerializeField] private Button kickButton;
        // [SerializeField] private TextMeshPro playerNameText;


        private void Awake() {
            // kickButton.onClick.AddListener(() => {
                // PlayerData playerData = GameMultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
                // KitchenGameLobby.Instance.KickPlayer(playerData.playerId.ToString());
                // GameMultiplayerManager.Instance.KickPlayer(playerData.clientId);
            // });
        }

        private void Start() {
            GameMultiplayerManager.Instance.OnPlayerDataNetworkListChanged += GameMultiplayerManager_OnPlayerDataNetworkListChanged;
            CharacterSelectReadyMultiplayer.Instance.OnReadyChanged += CharacterSelectReadyMultiplayer_OnReadyChanged;

            // kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

            UpdatePlayer();
        }

        private void CharacterSelectReadyMultiplayer_OnReadyChanged(object sender, System.EventArgs e) {
            UpdatePlayer();
        }

        private void GameMultiplayerManager_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e) {
            UpdatePlayer();
        }

        private void UpdatePlayer() {
            if (GameMultiplayerManager.Instance.IsPlayerIndexConnected(playerIndex)) {
                Show();

                PlayerDataMultiplayer playerData = GameMultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);

                readyGameObject.SetActive(CharacterSelectReadyMultiplayer.Instance.IsPlayerReady(playerData.clientId));
                Debug.Log("ReadyGameObject Active");
                // characterSelectVisual.SetPlayerPrefab(GameMultiplayerManager.Instance.GetPlayerPrefab(playerData.prefabId));
                characterSelectVisual.SetPlayerPrefab(GameMultiplayerManager.Instance.GetPlayerPrefab(playerData.prefabId));

                Debug.Log("CharacterSelectVisual SetPlayerPrefab");
                // playerNameText.text = playerData.playerName.ToString();

            } else {
                Hide();
            }
        }

        private void Show() {
            gameObject.SetActive(true);
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

        // private void OnDestroy() {
        //     GameMultiplayerManager.Instance.OnPlayerDataNetworkListChanged -= GameMultiplayerManager_OnPlayerDataNetworkListChanged;
        // }
    }
}
