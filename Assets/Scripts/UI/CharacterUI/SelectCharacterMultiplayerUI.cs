using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
namespace ITKombat
{
    public class SelectCharacterMultiplayerUI : MonoBehaviour
    {
        [SerializeField] private Button readyButton;
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI lobbyCodeText;

        private void Awake()
        {
            readyButton.onClick.AddListener(() =>
            {
                CharacterSelectReadyMultiplayer.Instance.SetPlayerReady();
            });
        }

        private void Start()
        {
            Lobby lobby = LobbyRoomManager.Instance.GetLobby();

            lobbyNameText.text = "Lobby Name: " + lobby.Name;
            lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
        }
    }
}
