using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using System;
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
                // ServerBattleRoomState.Instance.OnInteractAction(this, EventArgs.Empty);
                // Code dibawah ini kalau misal ingin tanpa langsung play matchnya 
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
