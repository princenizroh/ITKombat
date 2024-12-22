using UnityEngine;

namespace ITKombat
{
    public class ConnectingUI : MonoBehaviour
    {
        private void Start() {
            Debug.Log("ConnectingUI Start");
            GameMultiplayerManager.Instance.OnTryingToJoinGame += GameMultiplayerManager_OnTryingToJoinGame;
            Debug.Log("ConnectingUI Start 2");
            GameMultiplayerManager.Instance.OnFailedToJoinGame += GameMultiplayerManager_OnFailedToJoinGame;
            Debug.Log("ConnectingUI Start 3");

            Hide();
        }

        private void GameMultiplayerManager_OnFailedToJoinGame(object sender, System.EventArgs e) {
            Debug.Log("GameMultiplayerManager_OnFailedToJoinGame");
            Hide();
        }

        private void GameMultiplayerManager_OnTryingToJoinGame(object sender, System.EventArgs e) {
            Debug.Log("GameMultiplayerManager_OnTryingToJoinGame");
            Show();
        }
        public void Show()
        {
            Debug.Log("ConnectingUI Show");
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
        private void OnDestroy() {
            GameMultiplayerManager.Instance.OnTryingToJoinGame -= GameMultiplayerManager_OnTryingToJoinGame;
            GameMultiplayerManager.Instance.OnFailedToJoinGame -= GameMultiplayerManager_OnFailedToJoinGame;
        }
    }
}
