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
        private Lobby joinedLobby;
        private float heartbeatTimer;
        private float lobbyUpdateTimer;
        private string playerName;

        private async void Start() 
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            playerName = "Player1" + UnityEngine.Random.Range(10, 99); 

            Debug.Log(playerName);
        }

        private void Update()
        {
            HandleLobbyHeartbeat();
            HandleLobbyPollForUpdates();
            ButtonLobby();
        }

        private async void HandleLobbyHeartbeat()
        {
            if (hostLobby != null)
            {
                heartbeatTimer -= Time.deltaTime;
                if (heartbeatTimer < 0f)
                {
                    float heartbeatTimerMax = 15;
                    heartbeatTimer = heartbeatTimerMax;

                    await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
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
                    Player = GetPlayer(),
                    Data = new Dictionary<string, DataObject>
                    {
                        {
                            "GameMode", new DataObject(DataObject.VisibilityOptions.Public, "Ranked")
                        },
                        {
                            "Map", new DataObject(DataObject.VisibilityOptions.Public, "Laboratorium")
                        }
                    }
                };
                // Create a new lobby
                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
                hostLobby = lobby;
                joinedLobby = hostLobby;

                Debug.Log("Create Lobby!" + lobby.Name + "" + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
                PrintPlayers(hostLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log("Failed to create lobby: " + e.Message);
            }
        }

        private async void HandleLobbyPollForUpdates()
        {
            
            if (joinedLobby != null)
            {
                lobbyUpdateTimer -= Time.deltaTime;
                if (lobbyUpdateTimer < 0f)
                {
                    float lobbyUpdateTimerMax = 1.1f;
                    lobbyUpdateTimer = lobbyUpdateTimerMax;

                    Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                    joinedLobby = lobby;
                }
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
                      new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                    },
                    Order = new List<QueryOrder> { 
                      new QueryOrder(false, QueryOrder.FieldOptions.Created)
                    }
                };
                QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

                Debug.Log("Lobbies found: " + queryResponse.Results.Count);

                foreach (Lobby lobby in queryResponse.Results)
                {
                    Debug.Log("Lobby: " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Data["GameMode"].Value + " " + lobby.LobbyCode);
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
            if (Input.GetKeyDown(KeyCode.U))
            {
                UpdateLobbyGameMode("Unranked");
            }
        }

        private async void JoinLobbyByCode(string lobbyCode)
        {
            try
            {
                JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
                {
                    Player = GetPlayer()
                };
                Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
                joinedLobby = lobby;

                Debug.Log("Joined lobby: " + lobbyCode);

                PrintPlayers(joinedLobby);
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

        private Player GetPlayer()
        {
            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {
                        "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)
                    }
                }
            };
        }


        private void PrintPlayers()
        {
            PrintPlayers(joinedLobby);
        }
        private void PrintPlayers(Lobby lobby)
        {
            Debug.Log("Players in lobby: " + lobby.Name + " " + lobby.Data["GameMode"].Value + "" + lobby.Data["Map"].Value);
            foreach (Player player in lobby.Players)
            {
                Debug.Log("Player: " + player.Id + " " + player.Data["PlayerName"].Value);
            }
        }

        private async void UpdateLobbyGameMode(string gameMode)
        {
            try
            {

                hostLobby =await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {
                            "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode)
                        }
                    }
                });
                joinedLobby = hostLobby;
                PrintPlayers(hostLobby);
            } 
            catch (LobbyServiceException e)
            {
                Debug.Log("Failed to update lobby: " + e.Message);
            }
        }

        private async void UpdatePlayerName(string newPlayerName)
        {
            try
            {
                playerName = newPlayerName;
                await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {
                            "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)
                        }
                    }
                });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log("Failed to update player: " + e.Message);
            }
        }

        private async void LeaveLobby()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                Debug.Log("Left lobby");
            }
            catch (LobbyServiceException e)
            {
                Debug.Log("Failed to leave lobby: " + e.Message);
            }
        }

        private async void KickPlayer()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log("Failed to kick player: " + e.Message);
            }

        }

        private async void MigrateLobbyHost()
        {
            try
              {
                  hostLobby = await LobbyService.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
                  {
                      HostId = joinedLobby.Players[1].Id
                  });
                  joinedLobby = hostLobby;

              }
              catch (LobbyServiceException e)
              {
                  Debug.Log("Failed to migrate lobby host: " + e.Message);
              }
        }

        private async void DeleteLobby()
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
            } 
            catch (LobbyServiceException e)
            {
                Debug.Log("Failed to delete lobby: " + e.Message);
            }
        }
    }

}
