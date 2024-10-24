using ITKombat;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerButton : MonoBehaviour
{
    [SerializeField]
    private PlayerAttackTestNope playerAttack;
    [SerializeField]
    private PlayerMovement_2 playerMovement;
    [SerializeField]
    private PlayerSkill playerSkill;
    public bool canMove = true; 

    private void Start()
    {
        playerAttack = GetComponent<PlayerAttackTestNope>();
        playerMovement = GetComponent<PlayerMovement_2>();
        playerSkill = GetComponent<PlayerSkill>();
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
            if (canMove)
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
            playerSkill.Skill1(); 
        }
    }

    public void Skill2Button()
    {
        if (playerSkill != null)
        {
            playerSkill.Skill2(); 
        }
    }

    public void Skill3Button()
    {
        if (playerSkill != null)
        {
            playerSkill.Skill3();
        }
    }
}
