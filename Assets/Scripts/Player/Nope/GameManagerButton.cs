using ITKombat;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerButton : MonoBehaviour
{
    [SerializeField] 
    private PlayerAttackTestNope playerAttack; 
    [SerializeField] 
    private PlayerMovementNope playerMovement;
    [SerializeField] 
    private PlayerSkill playerSkill; 

    // Track crouch state
    private bool isCrouching = false;

    public void CrouchButtonPressed()
    {
        if (playerAttack != null)
        {
            if (isCrouching)
            {
                Debug.Log("Stopping crouch");
                playerAttack.StopCrouch();
            }
            else
            {
                Debug.Log("Starting crouch");
                playerAttack.StartCrouch();
            }
            // Toggle crouch state
            isCrouching = !isCrouching;
        }
    }

    public void AttackButton()
    {
        if (playerAttack != null)
        {
            Debug.Log("Regular attack");
            playerAttack.PerformAttack();
        }
    }

    public void RightInputButtonDown()
    {
        if (playerMovement != null)
        {
            Debug.Log("Moving right!");
            playerMovement.MoveRight();
        }
    }

    public void RightInputButtonUp()
    {
        if (playerMovement != null)
        {
            Debug.Log("Stop moving right!");
            playerMovement.StopMoveRight();
        }
    }

    public void LeftInputButtonDown()
    {
        if (playerMovement != null)
        {
            Debug.Log("Moving left!");
            playerMovement.MoveLeft();
        }
    }

    public void LeftInputButtonUp()
    {
        if (playerMovement != null)
        {
            Debug.Log("Stop moving left!");
            playerMovement.StopMoveLeft();
        }
    }

/*    public void DashButton()
    {
        if (playerMovement != null)
        {
            playerMovement.Dash(); 
            Debug.Log("Player dashed!");
        }
    }*/

    public void OnJumpInput()
    {
        if (playerMovement != null)
        {
            Debug.Log("Jumping!");
            playerMovement.JumpInput();
        }
    }

    public void Skill1Button()
    {
        if (playerSkill != null)
        {
            Debug.Log("Skill 1 triggered");
            playerSkill.Skill1(); 
        }
    }

    public void Skill2Button()
    {
        if (playerSkill != null)
        {
            Debug.Log("Skill 2 triggered");
            playerSkill.Skill2(); 
        }
    }

    public void Skill3Button()
    {
        if (playerSkill != null)
        {
            Debug.Log("Skill 3 triggered");
            playerSkill.Skill3();
        }
    }
}
