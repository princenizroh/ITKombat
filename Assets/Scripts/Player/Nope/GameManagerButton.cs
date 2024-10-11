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

    public void CrouchButtonUp()
    {
        if (playerMovement != null)
        {
            playerMovement.OnCrouchUp();
        }
    }

    public void CrouchButtonDown()
    {
        if (playerMovement != null)
        {
            playerMovement.OnCrouchDown();
        }
    }

    public void AttackButton()
    {
        if (playerAttack != null)
        {
            playerAttack.PerformAttack();
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

    [System.Obsolete]
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
