using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace ITKombat 
{
    public class WaitingPlayersUI : MonoBehaviour
    {
        [SerializeField] private Button start;
        private void Start()
        {
            Debug.Log("WaitingPlayersUI Start");
            ServerBattleRoomState.Instance.OnLocalPlayerReadyChanged += ServerBattleRoomState_OnLocalPlayerReadyChanged;
            ServerBattleRoomState.Instance.OnStateChanged += Instance_OnStateChanged;

            // Hide();
        }

        private void Awake()
        {
            start.onClick.AddListener(() =>
            {
                Debug.Log("Start Called");
                ServerBattleRoomState.Instance.OnInteractAction(this, EventArgs.Empty);
            });
        }

        private void ServerBattleRoomState_OnLocalPlayerReadyChanged(object sender, EventArgs e)
        {
            Debug.Log("ServerBattleRoomState_OnLocalPlayerReadyChanged");
            if (ServerBattleRoomState.Instance.IsLocalPlayerReady())
            {
                Debug.Log("ServerBattleRoomState_IsLocalPlayerReady");
                Show();
            }
        }

        private void Instance_OnStateChanged(object sender, EventArgs e)
        {
            if (ServerBattleRoomState.Instance.IsCountdownToStartActive())
            {
                Debug.Log("Instance_OnStateChanged");
                Hide();
            }
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
