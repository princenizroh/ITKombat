using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace ITKombat
{
    public class ConnectingResponseMessageUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text responseMessage;
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            closeButton.onClick.AddListener(() =>
            {
                Hide();
            });
        }

        private void Start()
        {
            GameMultiplayerManager.Instance.OnFailedToJoinGame += GameMultiplayerManager_OnFailedToJoinGame;
            Debug.Log("ConnectingResponseMessageUI Start");
            Hide();
        }

        private void GameMultiplayerManager_OnFailedToJoinGame(object sender, System.EventArgs e)
        {
            Show();
            string disconnectReason = NetworkManager.Singleton.DisconnectReason;
            responseMessage.text = "Failed to join game";
            Debug.Log("Disconnect Reason: " + disconnectReason);
            Debug.Log("Response Message Text: " + responseMessage.text);
            Debug.Log("GameMultiplayerManager_OnFailedToJoinGame");
            
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            GameMultiplayerManager.Instance.OnFailedToJoinGame -= GameMultiplayerManager_OnFailedToJoinGame;
        }
    }
}
