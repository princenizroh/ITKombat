using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

namespace ITKombat
{
    public class LobbyManager : MonoBehaviour
    {
        private Lobby hostLobby;
        private float heartbeatTimer;
        private string playerName = "Player1" + UnityEngine.Random.Range(10, 99);
        private async void Start() 
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private void Update()
        {
            HandleLobbyHeartbeat();
            ButtonLobby();
        }

        private void HandleLobbyHeartbeat()
        {
            if (hostLobby != null)
            {
                heartbeatTimer -= Time.deltaTime;
                if (heartbeatTimer < 0f)
                {
                    float heartbeatTimerMax = 15;
                    heartbeatTimer = heartbeatTimerMax;

                    LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
                }
            }


        }

        private async void CreateLobby()
        {
            try
            {
                string lobbyName = "MyLobby";
                int maxPlayers = 4;
                CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = false,
                };
                // Create a new lobby
                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
                hostLobby = lobby;

                Debug.Log("Create Lobby!" + lobby.Name + "" + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
                PrintPlayers(hostLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log("Failed to create lobby: " + e.Message);
            }
        }

        private async void ListLobbies()
        {
            try
            {
                QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
                {
                    Count = 25,
                    Filters = new List<QueryFilter> { 
                      new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                    },
                    Order = new List<QueryOrder> { 
                      new QueryOrder(false, QueryOrder.FieldOptions.Created)
                    }
                };
                QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

                Debug.Log("Lobbies found: " + queryResponse.Results.Count);

                foreach (Lobby lobby in queryResponse.Results)
                {
                    Debug.Log("Lobby: " + lobby.Name + " " + lobby.MaxPlayers);
                }
            }   catch (LobbyServiceException e)
            {
                Debug.Log("Failed to list lobbies: " + e.Message);
            }
        }

        private void ButtonLobby()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                // JoinLobby();
                QuickJoinLobby();
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                CreateLobby();
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                ListLobbies();
            }
        }

        private async void JoinLobby(string lobbyCode)
        {
            try
            {

                await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode);

                Debug.Log("Joined lobby: " + lobbyCode);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log("Failed to join lobby: " + e.Message);
            }
        }

        private async void QuickJoinLobby()
        {
            try
            {
                await LobbyService.Instance.QuickJoinLobbyAsync();
                Debug.Log("Quick join lobby");
            } 
            catch (LobbyServiceException e)
            {
                Debug.Log("Failed to quick join lobby: " + e.Message);
            }
        }

        private void PrintPlayers(Lobby lobby)
        {
            Debug.Log("Players in lobby: " + lobby.Name);
            foreach (Player player in lobby.Players)
            {
                Debug.Log("Player: " + player.Id);
            }
        }
    }

}
