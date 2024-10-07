using ITKombat;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerButton : MonoBehaviour
{
    [SerializeField] 
    private PlayerAttackTestNope playerAttack; 
    [SerializeField] 
    private PlayerMovementNope playerMovement; 

    private void Update() // Debug
    {
        // Handle attack input directly via the 'P' key for debugging
        if (Input.GetKey(KeyCode.P) && playerAttack != null && !playerAttack.IsCrouching())
        {
            playerAttack.PerformAttack();
            Debug.Log("Attack");
        }
    }

    // Method to toggle crouch
    public void OnCrouchButton()
    {
        if (playerAttack != null)
        {
            Debug.Log("Toggling crouch");
            playerAttack.ToggleCrouch(); // Toggle crouch on button press
        }
    }

    public void AttackButton()
    {
        if (playerAttack != null)
        {
            Debug.Log("Attack");
            playerAttack.PerformAttack();
        }
        else
        {
            Debug.LogWarning("PlayerAttackTestNope belum diassign!");
        }
    }

    public void CrouchAttackButton()
    {
        if (playerAttack != null && playerAttack.IsCrouching())
        {
            Debug.Log("Crouch attack");
            playerAttack.PerformCrouchAttack();
        }
        else
        {
            Debug.LogWarning("Crouch attack not allowed!");
        }
    }

    // Movement methods
    public void RightInputButtonDown()
    {
        if (playerMovement != null)
        {
            Debug.Log("Moving right!");
            playerMovement.MoveRight();
        }
        else
        {
            Debug.LogWarning("PlayerMovement belum diassign!");
        }
    }

    public void RightInputButtonUp()
    {
        if (playerMovement != null)
        {
            Debug.Log("Stop moving right!");
            playerMovement.StopMoveRight();
        }
        else
        {
            Debug.LogWarning("PlayerMovement belum diassign!");
        }
    }

    public void LeftInputButtonDown()
    {
        if (playerMovement != null)
        {
            Debug.Log("Moving left!");
            playerMovement.MoveLeft();
        }
        else
        {
            Debug.LogWarning("PlayerMovement belum diassign!");
        }
    }

    public void LeftInputButtonUp()
    {
        if (playerMovement != null)
        {
            Debug.Log("Stop moving left!");
            playerMovement.StopMoveLeft();
        }
        else
        {
            Debug.LogWarning("PlayerMovement belum diassign!");
        }
    }

    // New Dash method
    public void DashButton()
    {
        if (playerMovement != null)
        {
            playerMovement.Dash(); // Call the Dash method
            Debug.Log("Player dashed!");
        }
        else
        {
            Debug.LogWarning("PlayerMovement belum diassign!");
        }
    }

    public void OnJumpInput()
    {
        if (playerMovement != null)
        {
            Debug.Log("Jumping!");
            playerMovement.JumpInput();
        }
        else
        {
            Debug.LogWarning("PlayerMovement belum diassign!");
        }
    }
}
