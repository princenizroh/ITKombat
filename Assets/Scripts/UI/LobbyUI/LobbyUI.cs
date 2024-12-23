using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ITKombat
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private Button createGameButton;
        [SerializeField] private Button joinGameButton;

        private void Awake()
        {
            createGameButton.onClick.AddListener(() => {
                GameMultiplayerManager.Instance.StartHost();
                Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
            });
            joinGameButton.onClick.AddListener(() => {
                GameMultiplayerManager.Instance.StartClient();
            });
        }
    }
}

