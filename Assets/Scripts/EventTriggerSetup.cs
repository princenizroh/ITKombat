using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace ITKombat
{
    public class EventTriggerSetup : MonoBehaviour
    {
        public EventTrigger leftButtonTrigger;  // EventTrigger untuk tombol kiri
        public EventTrigger rightButtonTrigger; // EventTrigger untuk tombol kanan

        public EventTrigger blockButtonTrigger; // EventTrigger untuk tombol kanan

        public EventTrigger crouchButtonTrigger; // EventTrigger untuk tombol kanan

        private void Start()
        {
            StartCoroutine(SetupEventTrigger());
        }

        private IEnumerator SetupEventTrigger()
        {
            GameObject player = null;

            // Tunggu hingga prefab Player muncul di scene
            while (player == null)
            {
                player = GameObject.FindWithTag("Player");
                yield return null; // Tunggu satu frame
            }

            // Tambahkan Event Trigger
            PlayerMovement_2 playerMovement = player.GetComponent<PlayerMovement_2>();
            if (playerMovement != null)
            {
                SetupLeftButtonEvents(playerMovement);
                SetupRightButtonEvents(playerMovement);
                SetupBlockButtonEvents(playerMovement);
                SetupCrouchButtonEvents(playerMovement);
            }
            else
            {
                Debug.LogError("PlayerMovement_2 script not found on Player prefab.");
            }
        }

        private void SetupLeftButtonEvents(PlayerMovement_2 playerMovement)
        {
            if (leftButtonTrigger == null)
            {
                Debug.LogError("Left button EventTrigger not assigned.");
                return;
            }

            // PointerDown untuk tombol kiri
            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            pointerDownEntry.callback.AddListener((data) => { playerMovement.OnMoveLeft(); });
            leftButtonTrigger.triggers.Add(pointerDownEntry);

            // PointerUp untuk tombol kiri
            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            pointerUpEntry.callback.AddListener((data) => { playerMovement.OnStopMoving(); });
            leftButtonTrigger.triggers.Add(pointerUpEntry);

            Debug.Log("Left button events set up successfully.");
        }

        private void SetupRightButtonEvents(PlayerMovement_2 playerMovement)
        {
            if (rightButtonTrigger == null)
            {
                Debug.LogError("Right button EventTrigger not assigned.");
                return;
            }

            // PointerDown untuk tombol kanan
            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            pointerDownEntry.callback.AddListener((data) => { playerMovement.OnMoveRight(); });
            rightButtonTrigger.triggers.Add(pointerDownEntry);

            // PointerUp untuk tombol kanan
            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            pointerUpEntry.callback.AddListener((data) => { playerMovement.OnStopMoving(); });
            rightButtonTrigger.triggers.Add(pointerUpEntry);

            Debug.Log("Right button events set up successfully.");
        }

        private void SetupBlockButtonEvents(PlayerMovement_2 playerMovement)
        {
            if (blockButtonTrigger == null)
            {
                Debug.LogError("Right button EventTrigger not assigned.");
                return;
            }

            // PointerDown untuk tombol kanan
            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            pointerDownEntry.callback.AddListener((data) => { playerMovement.OnBlockDown(); });
            blockButtonTrigger.triggers.Add(pointerDownEntry);

            // PointerUp untuk tombol kanan
            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            pointerUpEntry.callback.AddListener((data) => { playerMovement.OnBlockUp(); });
            blockButtonTrigger.triggers.Add(pointerUpEntry);

            Debug.Log("Right button events set up successfully.");
        }

        private void SetupCrouchButtonEvents(PlayerMovement_2 playerMovement)
        {
            if (crouchButtonTrigger == null)
            {
                Debug.LogError("Right button EventTrigger not assigned.");
                return;
            }

            // PointerDown untuk tombol kanan
            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            pointerDownEntry.callback.AddListener((data) => { playerMovement.OnCrouchDown(); });
            crouchButtonTrigger.triggers.Add(pointerDownEntry);

            // PointerUp untuk tombol kanan
            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            pointerUpEntry.callback.AddListener((data) => { playerMovement.OnCrouchUp(); });
            crouchButtonTrigger.triggers.Add(pointerUpEntry);

            Debug.Log("Right button events set up successfully.");
        }
    }
}
