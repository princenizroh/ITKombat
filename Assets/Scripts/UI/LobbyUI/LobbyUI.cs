using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies.Models;
using TMPro;

namespace ITKombat
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private Button leaveGameButton;
        [SerializeField] private Button quickJoinGameButton;
        [SerializeField] private Button joinCodeGameButton;
        [SerializeField] private TMP_InputField joinCodeInputField;
        [SerializeField] private TMP_InputField playerNameInputField;

        private void Awake()
        {
            leaveGameButton.onClick.AddListener(() => {
                LobbyRoomManager.Instance.LeaveLobby();
                Loader.Load(Loader.Scene.BattleMode);
            });
            quickJoinGameButton.onClick.AddListener(() => {
                LobbyRoomManager.Instance.QuickJoinLobby();
            });
            joinCodeGameButton.onClick.AddListener(() => {
                LobbyRoomManager.Instance.JoinWithCode(joinCodeInputField.text);
            });
        }

        private void Start()
        {
            playerNameInputField.text = GameMultiplayerManager.Instance.GetPlayerName();
            playerNameInputField.onEndEdit.AddListener((string text) => {
                GameMultiplayerManager.Instance.SetPlayerName(text);
            });
        }
    }
}

