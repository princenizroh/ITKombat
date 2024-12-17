using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Newtonsoft.Json;
namespace ITKombat
{
    public class MatchmakerUI : MonoBehaviour
    {
        public const string DEFAULT_QUEUE = "Queue-A";

        private FirebaseAuth auth;
        private FirebaseUser firebaseUser;
        [SerializeField] private Button findMatchButton;
        [SerializeField] private Transform lookingForMatchTransform;


        private CreateTicketResponse createTicketResponse;
        private float pollTicketTimer;
        private float pollTicketTimerMax = 1.1f;

        private async void Start()
        {
            try
            {
                // Inisialisasi Unity Services
                await UnityServices.InitializeAsync();

                InitializeFirebase();

                Debug.Log("Unity Services initialized and user signed in.");

            }
            catch (Exception e)
            {
                Debug.LogError("Failed to initialize Unity Services: " + e.Message);
            }
        }

        private void InitializeFirebase()
        {
            auth = FirebaseAuth.DefaultInstance;
            // auth.SignInWithEmailAndPasswordAsync("zaky@gmail.com", "admin123")
            // .ContinueWith(task => {
            //     if (task.IsCompleted && !task.IsFaulted)
            //     {
            //         firebaseUser = task.Result.User;
            //         Debug.Log("Firebase User ID: " + firebaseUser.UserId);
            //     }
            //     else
            //     {
            //         Debug.LogError("Firebase sign-in failed: " + task.Exception);
            //     }
            // });
        }
        private void Awake() {
            lookingForMatchTransform.gameObject.SetActive(false);

            findMatchButton.onClick.AddListener(() => {
                FindMatch();
            });
        }


        // Fungsi untuk A/B test pada matchmaker 
        // private async void PoolTest()
        // {
        //     var ticketResponse = await MatchmakerService.Instance.CreateTicketAsync(
        //         new List<Unity.Services.Matchmaker.Models.Player>(), 
        //         new CreateTicketOptions { QueueName = "Queue-A" }
        //         );   
        //     object abTestingSerialize = (object)ticketResponse.AbTestingResult;
        //     string abTestingJsonOutput = JsonConvert.SerializeObject(abTestingSerialize, Formatting.Indented);
        //     Debug.Log(abTestingJsonOutput);
        // }

        private async void FindMatch() {
            Debug.Log("FindMatch");

            string playerId = firebaseUser != null ? firebaseUser.UserId : AuthenticationService.Instance.PlayerId;
            var players = new List<Unity.Services.Matchmaker.Models.Player> {
                new Unity.Services.Matchmaker.Models.Player(playerId, 
                new MatchmakingPlayerData {
                    Skill = 100,
                })
            };

            var options = new CreateTicketOptions { QueueName = DEFAULT_QUEUE };
            lookingForMatchTransform.gameObject.SetActive(true);
            Debug.Log("Player ID for matchmaking: " + playerId);
                
            createTicketResponse = await MatchmakerService.Instance.CreateTicketAsync(players, options);

            object abTestingSerialize = (object)createTicketResponse.AbTestingResult;
            string abTestingJsonOutput = JsonConvert.SerializeObject(abTestingSerialize, Formatting.Indented);
            Debug.Log("Testing " + abTestingJsonOutput);
            Debug.Log("Ticket Id" + createTicketResponse.Id);

            // Wait a bit, don't poll right away
            pollTicketTimer = pollTicketTimerMax;
        }

        [Serializable]
        public class MatchmakingPlayerData {
            public int Skill;
        }


        private void Update() {
            if (createTicketResponse != null) {
                // Has ticket
                pollTicketTimer -= Time.deltaTime;
                if (pollTicketTimer <= 0f) {
                    pollTicketTimer = pollTicketTimerMax;

                    Debug.Log("Polling Matchmaker Ticket...");
                    PollMatchmakerTicket();
                }
            }
        }

        private async void PollMatchmakerTicket() {
            Debug.Log("PollMatchmakerTicket");
            TicketStatusResponse ticketStatusResponse = await MatchmakerService.Instance.GetTicketAsync(createTicketResponse.Id);

            if (ticketStatusResponse == null) {
                // Null means no updates to this ticket, keep waiting
                Debug.Log("Null means no updates to this ticket, keep waiting");
                return;
            }

            // Not null means there is an update to the ticket
            if (ticketStatusResponse.Type == typeof(MultiplayAssignment)) {
                // It's a Multiplay assignment
                MultiplayAssignment multiplayAssignment = ticketStatusResponse.Value as MultiplayAssignment;

                Debug.Log("multiplayAssignment.Status " + multiplayAssignment.Status);
                switch (multiplayAssignment.Status) {
                    case MultiplayAssignment.StatusOptions.Found:
                        createTicketResponse = null;

                        Debug.Log(multiplayAssignment.Ip + " " + multiplayAssignment.Port);

                        string ipv4Address = multiplayAssignment.Ip;
                        ushort port = (ushort)multiplayAssignment.Port;
                        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port);

                        GameMultiplayerManager.Instance.StartClient();
                        break;
                    case MultiplayAssignment.StatusOptions.InProgress:
                        // Still waiting...
                        break;
                    case MultiplayAssignment.StatusOptions.Failed:
                        createTicketResponse = null;
                        Debug.Log("Failed to create Multiplay server!");
                        lookingForMatchTransform.gameObject.SetActive(false);
                        break;
                    case MultiplayAssignment.StatusOptions.Timeout:
                        createTicketResponse = null;
                        Debug.Log("Multiplay Timeout!");
                        lookingForMatchTransform.gameObject.SetActive(false);
                        break;
                }
            }

        }
    }
}
