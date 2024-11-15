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
        [SerializeField] private Button jumpButton;
        // [SerializeField] private Button dashButton;
        [SerializeField] private GameObject crouchButton;
        // [SerializeField] private Button Skill1Button;
        // [SerializeField] private Button Skill2Button;
        // [SerializeField] private Button Skill3Button;
        

        private void Awake()
        {
            AttackButton();
            LeftMovementButton();
            RightMovementButton();
            JumpButton();
            CrouchButton();
        }

        private void AttackButton()
        {
            attackButton.onClick.AddListener(() =>
            {
                GameObject playerIF = GameObject.FindGameObjectWithTag("Player");
                if (playerIF != null)
                {
                    ServerCharacterAction playerIFState = playerIF.GetComponent<ServerCharacterAction>();
                    PlayerIFAttack playerIFStateNonNetwork = playerIF.GetComponent<PlayerIFAttack>();
                    if (playerIFState != null)
                    {
                        playerIFState.PerformAttack();
                    }

                    else if (playerIFStateNonNetwork != null)
                    {
                        playerIFStateNonNetwork.PerformAttack();
                    }

                    else
                    {
                        Debug.Log("ServerCharacterAction component is missing on the Player GameObject.");
                    }
                }
                else
                {
                    Debug.Log("Player GameObject not found.");
                }
            });
        }
        
        private void JumpButton()
        {
            jumpButton.onClick.AddListener(() =>
            {
                GameObject playerIF = GameObject.FindGameObjectWithTag("Player");
                if (playerIF != null)
                {
                    ServerCharacterMovement playerIFState = playerIF.GetComponent<ServerCharacterMovement>();
                    PlayerMovement_2 playerIFStateNonNetwork = playerIF.GetComponent<PlayerMovement_2>();
                    if (playerIFState != null)
                    {
                        playerIFState.OnJump();
                    }

                    if (playerIFStateNonNetwork != null)
                    {
                        playerIFStateNonNetwork.OnJump();
                    }

                    else
                    {
                        Debug.Log("ServerCharacterAction component is missing on the Player GameObject.");
                    }
                }
                else
                {
                    Debug.Log("Player GameObject not found.");
                }
            });
        }

        private void CrouchButton()
        {
            EventTrigger trigger = crouchButton.AddComponent<EventTrigger>();

            // Menambahkan PointerDown
            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
            pointerDownEntry.eventID = EventTriggerType.PointerDown;
            pointerDownEntry.callback.AddListener((data) => { OnCrouchButtonDown(); });
            trigger.triggers.Add(pointerDownEntry);

            // Menambahkan PointerUp
            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
            pointerUpEntry.eventID = EventTriggerType.PointerUp;
            pointerUpEntry.callback.AddListener((data) => { OnCrouchButtonUp(); });
            trigger.triggers.Add(pointerUpEntry);
        }

        private void OnCrouchButtonDown()
        {
            GameObject playerIF = GameObject.FindGameObjectWithTag("Player");
            if (playerIF != null)
            {
                ServerCharacterMovement playerIFState = playerIF.GetComponent<ServerCharacterMovement>();
                PlayerMovement_2 playerIFStateNonNetwork = playerIF.GetComponent<PlayerMovement_2>();
                if (playerIFState != null)
                {
                    playerIFState.OnCrouchDown();
                }

                if (playerIFStateNonNetwork != null)
                {
                    playerIFStateNonNetwork.OnCrouchDown();
                }

                else
                {
                    Debug.Log("ServerCharacterAction component is missing on the Player GameObject.");
                }
            }
            else
            {
                Debug.Log("Player GameObject not found.");
            }
        }

        private void OnCrouchButtonUp()
        {
            GameObject playerIF = GameObject.FindGameObjectWithTag("Player");
            if (playerIF != null)
            {
                ServerCharacterMovement playerIFState = playerIF.GetComponent<ServerCharacterMovement>();
                PlayerMovement_2 playerIFStateNonNetwork = playerIF.GetComponent<PlayerMovement_2>();
                if (playerIFState != null)
                {
                    playerIFState.OnCrouchUp();
                }

                if (playerIFStateNonNetwork != null)
                {
                    playerIFStateNonNetwork.OnCrouchUp();
                }

                else
                {
                    Debug.Log("ServerCharacterAction component is missing on the Player GameObject.");
                }
            }
            else
            {
                Debug.Log("Player GameObject not found.");
            }
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
                PlayerMovement_2 playerIFStateNonNetwork = playerIF.GetComponent<PlayerMovement_2>();
                if (playerIFState != null)
                {
                    playerIFState.OnMoveLeft();
                }

                else if (playerIFStateNonNetwork != null)
                {
                    playerIFStateNonNetwork.OnMoveLeft();
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
                PlayerMovement_2 playerIFStateNonNetwork = playerIF.GetComponent<PlayerMovement_2>();
                if (playerIFState != null)
                {
                    playerIFState.OnStopMoving();
                }

                else if (playerIFStateNonNetwork != null)
                {
                    playerIFStateNonNetwork.OnStopMoving();
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
                PlayerMovement_2 playerIFStateNonNetwork = playerIF.GetComponent<PlayerMovement_2>();
                if (playerIFState != null)
                {
                    playerIFState.OnMoveRight();
                }

                else if (playerIFStateNonNetwork != null)
                {
                    playerIFStateNonNetwork.OnMoveRight();
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
                PlayerMovement_2 playerIFStateNonNetwork = playerIF.GetComponent<PlayerMovement_2>();
                if (playerIFState != null)
                {
                    playerIFState.OnStopMoving();
                }

                else if (playerIFStateNonNetwork != null)
                {
                    playerIFStateNonNetwork.OnStopMoving();
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
