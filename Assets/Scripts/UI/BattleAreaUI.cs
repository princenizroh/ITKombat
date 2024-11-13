using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ITKombat
{
    public class BattleAreaUI : MonoBehaviour
    {
        [SerializeField] private Button attackButton;
        [SerializeField] private GameObject leftButtonMove;
        [SerializeField] private GameObject rightButtonMove;

        private void Awake()
        {
            AttackButton();
            LeftMovementButton();
            RightMovementButton();
        }

        private void AttackButton()
        {
            attackButton.onClick.AddListener(() =>
            {
                GameObject playerIF = GameObject.FindGameObjectWithTag("Player");
                if (playerIF != null)
                {
                    ServerCharacterAction playerIFState = playerIF.GetComponent<ServerCharacterAction>();
                    if (playerIFState != null)
                    {
                        playerIFState.PerformAttack();
                    }
                }
                else
                {
                    Debug.Log("Player GameObject not found.");
                }
            });
        }

        private void LeftMovementButton()
        {
            EventTrigger trigger = leftButtonMove.AddComponent<EventTrigger>();

            // Menambahkan PointerDown
            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
            pointerDownEntry.eventID = EventTriggerType.PointerDown;
            pointerDownEntry.callback.AddListener((data) => { OnLeftButtonDown(); });
            trigger.triggers.Add(pointerDownEntry);

            // Menambahkan PointerUp
            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
            pointerUpEntry.eventID = EventTriggerType.PointerUp;
            pointerUpEntry.callback.AddListener((data) => { OnLeftButtonUp(); });
            trigger.triggers.Add(pointerUpEntry);
        }

        private void RightMovementButton()
        {
            EventTrigger trigger = rightButtonMove.AddComponent<EventTrigger>();

            // Menambahkan PointerDown
            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
            pointerDownEntry.eventID = EventTriggerType.PointerDown;
            pointerDownEntry.callback.AddListener((data) => { OnRightButtonDown(); });
            trigger.triggers.Add(pointerDownEntry);

            // Menambahkan PointerUp
            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
            pointerUpEntry.eventID = EventTriggerType.PointerUp;
            pointerUpEntry.callback.AddListener((data) => { OnRightButtonUp(); });
            trigger.triggers.Add(pointerUpEntry);
        }

        private void OnLeftButtonDown()
        {
            GameObject playerIF = GameObject.FindGameObjectWithTag("Player");
            if (playerIF != null)
            {
                ServerCharacterMovement playerIFState = playerIF.GetComponent<ServerCharacterMovement>();
                if (playerIFState != null)
                {
                    playerIFState.OnMoveLeft();
                }
                else
                {
                    Debug.LogError("ServerCharacterMovement component is missing on the Player GameObject.");
                }
            }
            else
            {
                Debug.LogError("Player GameObject not found.");
            }
        }

        private void OnLeftButtonUp()
        {
            GameObject playerIF = GameObject.FindGameObjectWithTag("Player");
            if (playerIF != null)
            {
                ServerCharacterMovement playerIFState = playerIF.GetComponent<ServerCharacterMovement>();
                if (playerIFState != null)
                {
                    playerIFState.OnStopMoving();
                }
                else
                {
                    Debug.LogError("ServerCharacterMovement component is missing on the Player GameObject.");
                }
            }
            else
            {
                Debug.LogError("Player GameObject not found.");
            }
        }

        private void OnRightButtonDown()
        {
            GameObject playerIF = GameObject.FindGameObjectWithTag("Player");
            if (playerIF != null)
            {
                ServerCharacterMovement playerIFState = playerIF.GetComponent<ServerCharacterMovement>();
                if (playerIFState != null)
                {
                    playerIFState.OnMoveRight();
                }
                else
                {
                    Debug.LogError("ServerCharacterMovement component is missing on the Player GameObject.");
                }
            }
            else
            {
                Debug.LogError("Player GameObject not found.");
            }
        }

        private void OnRightButtonUp()
        {
            GameObject playerIF = GameObject.FindGameObjectWithTag("Player");
            if (playerIF != null)
            {
                ServerCharacterMovement playerIFState = playerIF.GetComponent<ServerCharacterMovement>();
                if (playerIFState != null)
                {
                    playerIFState.OnStopMoving();
                }
                else
                {
                    Debug.LogError("ServerCharacterMovement component is missing on the Player GameObject.");
                }
            }
            else
            {
                Debug.LogError("Player GameObject not found.");
            }
        }
    }
}
