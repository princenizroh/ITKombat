using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace ITKombat
{
    public class LobbyListSingleUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lobbyNameText;


        private Lobby lobby;


        private void Awake() {
            GetComponent<Button>().onClick.AddListener(() => {
                LobbyRoomManager.Instance.JoinWithId(lobby.Id);
            });
        }

        public void SetLobby(Lobby lobby) {
            this.lobby = lobby;
            lobbyNameText.text = lobby.Name;
        }
    }
}
