using ITKombat;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerButton : MonoBehaviour
{
    
    private PlayerIFAttack playerAttack;
    private PlayerMovement_2 playerMovement;
    private SkillsHolder playerSkill;

    private void Start()
    {
        playerAttack = GetComponent<PlayerIFAttack>();
        playerMovement = GetComponent<PlayerMovement_2>();
        playerSkill = GetComponent<SkillsHolder>();
    }

    public void CrouchButtonDown()
    {
        if (playerMovement != null)
        {
            playerMovement.OnCrouchDown();
        }
    }

    public void CrouchButtonUp()
    {
        if (playerMovement != null)
        {
            playerMovement.OnCrouchUp();
        }
    }

    // Tambahkan metode untuk block
    public void BlockButtonDown()
    {
        if (playerMovement != null)
        {
            playerMovement.OnBlockDown();
        }
    }

    public void BlockButtonUp()
    {
        if (playerMovement != null)
        {
            playerMovement.OnBlockUp();
        }
    }

    public void AttackButton()
    {
        if (playerMovement != null && playerMovement.IsCrouching) // Akses dengan properti
        {
            playerMovement.OnCrouchAttack(); // Crouch attack when crouching
        }
        else if (playerAttack != null)
        {
            playerAttack.PerformAttack(); // Regular attack
        }
    }

    public void LeftInputButtonDown()
    {
        if (playerMovement != null)
        {
                playerMovement.OnMoveLeft();
        }
    }

    public void RightInputButtonDown()
    {
        if (playerMovement != null)
        {
            playerMovement.OnMoveRight();
        }
    }

    public void LeftInputButtonUp()
    {
        if (playerMovement != null)
        {
            playerMovement.OnStopMoving();
        }
    }

    public void RightInputButtonUp()
    {
        if (playerMovement != null)
        {
            playerMovement.OnStopMoving();
        }
    }

    public void DashButton()
    {
        if (playerMovement != null)
        {
            playerMovement.OnDash(); 
        }
    }

    public void OnJumpInput()
    {
        if (playerMovement != null)
        {
            playerMovement.OnJump();
        }
    }

    public void Skill1Button()
    {
        if (playerSkill != null)
        {
            playerSkill.ActivateSkill1(); 
        }
    }

    public void Skill2Button()
    {
        if (playerSkill != null)
        {
            playerSkill.ActivateSkill2(); 
        }
    }

    public void Skill3Button()
    {
        if (playerSkill != null)
        {
            playerSkill.ActivateSkill3();
        }
    }
}
