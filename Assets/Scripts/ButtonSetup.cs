using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ITKombat
{
    public class ButtonSetup : MonoBehaviour
{
    public Button attackButton;  // Assign dari Editor
    public Button skill1Button; // Assign dari Editor

    public Button skill2Button; // Assign dari Editor

    public Button skill3Button; // Assign dari Editor
    public Button jumpButton;   // Assign dari Editor
    public Button dashButton;




    private void Start()
    {
        StartCoroutine(SetupButton());
    }

    private IEnumerator SetupButton()
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
            PlayerIFAttack playerAttack = player.GetComponent<PlayerIFAttack>();
            PlayerSkill playerSkill = player.GetComponent<PlayerSkill>();
            if (playerMovement != null && playerAttack != null && playerSkill != null)
            {
                
                SetupMoveClickButtons(playerMovement);
                SetupAttackClickButtons(playerAttack);
                SetupSkillClickButtons(playerSkill);

            }
            else
            {
                Debug.LogError("PlayerMovement_2 script not found on Player prefab.");
            }

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

// // Cari instance player aktif di scene
//         GameObject player = GameObject.FindWithTag("Player");
//         if (player != null)
//         {
//             playerMovement = player.GetComponent<PlayerMovement_2>();

//             if (playerMovement != null)
//             {
//                 // Bind tombol ke metode di PlayerMovement_2
//                 attackButton.onClick.AddListener(playerIFAttack.PerformAttack);
//                 skill1Button.onClick.AddListener(playerSkill.Skill1);
//                 skill2Button.onClick.AddListener(playerSkill.Skill2);
//                 skill3Button.onClick.AddListener(playerSkill.Skill3);
//                 jumpButton.onClick.AddListener(playerMovement.OnJump);
//                 dashButton.onClick.AddListener(playerMovement.OnDash);
//             }
//             else
//             {
//                 Debug.LogError("PlayerMovement_2 component not found on Player.");
//             }
//         }
//         else
//         {
//             Debug.LogError("Player not found in the scene.");
//         }
// 
            // if (skill1Button != null)
            //     skill1Button.onClick.AddListener(playerSkill.Skill1);

            // if (skill2Button != null)
            //     skill2Button.onClick.AddListener(playerSkill.Skill2);

            // if (skill3Button != null)
            //     skill3Button.onClick.AddListener(playerSkill.Skill3);

            // if (dashButton != null)
            //     dashButton.onClick.AddListener(playerMovement.OnDash);

            // // Debug.Log("OnClick button events set up successfully.");
            //             if (attackButton != null)
            //     attackButton.onClick.AddListener(playerIFAttack.PerformAttack);