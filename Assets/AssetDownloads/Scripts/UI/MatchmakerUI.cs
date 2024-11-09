using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.UI;

public class MatchmakerUI : MonoBehaviour {


    public const string DEFAULT_QUEUE = "Queue-A";


    [SerializeField] private Button findMatchButton;
    [SerializeField] private Transform lookingForMatchTransform;


    private CreateTicketResponse createTicketResponse;
    private float pollTicketTimer;
    private float pollTicketTimerMax = 1.1f;


    private void Awake() {
        lookingForMatchTransform.gameObject.SetActive(false);

        findMatchButton.onClick.AddListener(() => {
            FindMatch();
        });
    }

    private async void FindMatch() {
        Debug.Log("FindMatch");

        lookingForMatchTransform.gameObject.SetActive(true);

        createTicketResponse = await MatchmakerService.Instance.CreateTicketAsync(new List<Unity.Services.Matchmaker.Models.Player> {
             new Unity.Services.Matchmaker.Models.Player(AuthenticationService.Instance.PlayerId, 
             new MatchmakingPlayerData {
                  Skill = 100,
             })
        }, new CreateTicketOptions { QueueName = DEFAULT_QUEUE });

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

                    KitchenGameMultiplayer.Instance.StartClient();
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