using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Unity.Netcode;
using System;
using UnityEngine.UI;
namespace ITKombat
{
    public class BattleAreaUI : NetworkBehaviour
    {
        public EventTrigger leftButtonTrigger;  
        public EventTrigger rightButtonTrigger; 

        public EventTrigger blockButtonTrigger; 

        public EventTrigger crouchButtonTrigger; 

         public Button attackButton;  // Assign dari Editor
        public Button skill1Button; // Assign dari Editor

        public Button skill2Button; // Assign dari Editor

        public Button skill3Button; // Assign dari Editor
        public Button jumpButton;   // Assign dari Editor
        public Button dashButton;
        private ulong clientId;

        [Obsolete]
        private void Start()
        {
            clientId = NetworkManager.Singleton.LocalClientId;
            StartCoroutine(SetupEventTrigger());
            StartCoroutine(SetupButton());
            StartCoroutine(SetupEnemyEventTrigger());
            
        }

        [Obsolete]

        private IEnumerator SetupEventTrigger()
        {

            NetworkObject playerNetworkObject = null;

            while (playerNetworkObject == null)
            {
                foreach (var networkObject in FindObjectsOfType<NetworkObject>())
                {
                    Debug.Log($"Checking object: {networkObject.name}, OwnerClientId: {networkObject.OwnerClientId}, IsOwner: {networkObject.IsOwner}");

                    if (networkObject.OwnerClientId == clientId && networkObject.CompareTag("Player"))
                    {
                        playerNetworkObject = networkObject;
                        break;
                    }
                }
                yield return null; // Tunggu satu frame
            }
            

            GameObject player = playerNetworkObject.gameObject;
            Debug.Log($"Player Prefab: {player.name}");
            ServerCharacterMovement playerMovement = player.GetComponent<ServerCharacterMovement>();
            if (playerMovement != null)
            {
                SetupLeftButtonEvents(playerMovement);
                SetupRightButtonEvents(playerMovement);
                SetupBlockButtonEvents(playerMovement);
                SetupCrouchButtonEvents(playerMovement);
            }
            else
            {
                Debug.LogError("ServerCharacterMovement script not found on Player prefab.");
            }
        }

        [Obsolete]
        private IEnumerator SetupEnemyEventTrigger()
        {
            NetworkObject enemyNetworkObject = null;

            while (enemyNetworkObject == null)
            {
                foreach (var networkObject in FindObjectsOfType<NetworkObject>())
                {
                    if (networkObject.OwnerClientId != clientId && networkObject.CompareTag("Enemy"))
                    {
                        enemyNetworkObject = networkObject;
                        break;
                    }
                }
                yield return null; // Tunggu satu frame
            }

            GameObject enemy = enemyNetworkObject.gameObject;

            // Jika ada logika khusus untuk Enemy, tambahkan di sini.
            Debug.Log($"Enemy Prefab: {enemy.name}");
        }


        private GameObject FindPlayerObject()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                player = GameObject.FindWithTag("Enemy");
            }
            return player;
        }

         private void SetupLeftButtonEvents(ServerCharacterMovement playerMovement)
        {
            if (leftButtonTrigger == null)
            {
                Debug.LogError("Left button EventTrigger not assigned.");
                return;
            }

            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            pointerDownEntry.callback.AddListener((data) => { playerMovement.OnMoveLeft(); });
            leftButtonTrigger.triggers.Add(pointerDownEntry);

            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            pointerUpEntry.callback.AddListener((data) => { playerMovement.OnStopMoving(); });
            leftButtonTrigger.triggers.Add(pointerUpEntry);
        }

        private void SetupRightButtonEvents(ServerCharacterMovement playerMovement)
        {
            if (rightButtonTrigger == null)
            {
                Debug.LogError("Right button EventTrigger not assigned.");
                return;
            }

            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            pointerDownEntry.callback.AddListener((data) => { playerMovement.OnMoveRight(); });
            rightButtonTrigger.triggers.Add(pointerDownEntry);

            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            pointerUpEntry.callback.AddListener((data) => { playerMovement.OnStopMoving(); });
            rightButtonTrigger.triggers.Add(pointerUpEntry);
        }

        private void SetupBlockButtonEvents(ServerCharacterMovement playerMovement)
        {
            if (blockButtonTrigger == null)
            {
                Debug.LogError("Right button EventTrigger not assigned.");
                return;
            }
            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            pointerDownEntry.callback.AddListener((data) => { playerMovement.OnBlockDown(); });
            blockButtonTrigger.triggers.Add(pointerDownEntry);

            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            pointerUpEntry.callback.AddListener((data) => { playerMovement.OnBlockUp(); });
            blockButtonTrigger.triggers.Add(pointerUpEntry);

        }

        private void SetupCrouchButtonEvents(ServerCharacterMovement playerMovement)
        {
            if (crouchButtonTrigger == null)
            {
                Debug.LogError("Right button EventTrigger not assigned.");
                return;
            }

            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            pointerDownEntry.callback.AddListener((data) => { playerMovement.OnCrouchDown(); });
            crouchButtonTrigger.triggers.Add(pointerDownEntry);
            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            pointerUpEntry.callback.AddListener((data) => { playerMovement.OnCrouchUp(); });
            crouchButtonTrigger.triggers.Add(pointerUpEntry);
        }

        [Obsolete]
        private IEnumerator SetupButton()
        {
            NetworkObject playerNetworkObject = null;

            while (playerNetworkObject == null)
            {
                foreach (var networkObject in FindObjectsOfType<NetworkObject>())
                {
                    if (networkObject.OwnerClientId == clientId && networkObject.CompareTag("Player"))
                    {
                        playerNetworkObject = networkObject;
                        break;
                    }
                }
                yield return null; // Tunggu satu frame
            }

            GameObject player = playerNetworkObject.gameObject;
            ServerCharacterMovement serverCharacterMovement = player.GetComponent<ServerCharacterMovement>();
            ServerCharacterAction serverCharacterAction = player.GetComponent<ServerCharacterAction>();
            PlayerSkill playerSkill = player.GetComponent<PlayerSkill>();
            if (serverCharacterAction != null && serverCharacterMovement != null && playerSkill != null)
            {
                ServerSetupMoveClickButtons(serverCharacterMovement);
                ServerSetupAttackClickButtons(serverCharacterAction);
                SetupSkillClickButtons(playerSkill);
            }
            else
            {
                Debug.LogError("ServerCharacter script not found on Player prefab.");
            }

        }

        private void ServerSetupMoveClickButtons(ServerCharacterMovement serverPlayerMovement)
        {
            if (jumpButton ==  null)
            {
                Debug.LogError("Jump button not assigned.");
                return;
            }

            if (dashButton == null)
            {
                Debug.LogError("Dash button not assigned.");
                return;
            }
            
            dashButton.onClick.AddListener(serverPlayerMovement.OnDash);

            jumpButton.onClick.AddListener(serverPlayerMovement.OnJump);

        }

        private void ServerSetupAttackClickButtons(ServerCharacterAction serverPlayerAttack)
        {
            if (attackButton ==  null)
            {
                Debug.LogError("Jump button not assigned.");
                return;
            }

            attackButton.onClick.AddListener(serverPlayerAttack.OnButtonDown);

        }

        private void SetupMoveClickButtons(PlayerMovement_2 playerMovement)
        {
            if (jumpButton ==  null)
            {
                Debug.LogError("Jump button not assigned.");
                return;
            }

            if (dashButton == null)
            {
                Debug.LogError("Dash button not assigned.");
                return;
            }
            
            dashButton.onClick.AddListener(playerMovement.OnDash);

            jumpButton.onClick.AddListener(playerMovement.OnJump);

        }
        private void SetupSkillClickButtons(PlayerSkill playerSkill)
        {
            if (skill1Button ==  null)
            {
                Debug.LogError("Skill 1 button not assigned.");
                return;
            }
            if (skill2Button ==  null)
            {
                Debug.LogError("Skill 2  button not assigned.");
                return;
            }
            if (skill3Button ==  null)
            {
                Debug.LogError("Skill 3  button not assigned.");
                return;
            }

            skill1Button.onClick.AddListener(playerSkill.Skill1);
            skill2Button.onClick.AddListener(playerSkill.Skill2);
            skill3Button.onClick.AddListener(playerSkill.Skill3);

        }
        private void SetupAttackClickButtons(PlayerIFAttack playerAttack)
        {
            if (attackButton ==  null)
            {
                Debug.LogError("Jump button not assigned.");
                return;
            }

            attackButton.onClick.AddListener(playerAttack.OnButtonDown);

        }

    }
}
