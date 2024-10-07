using ITKombat;
using Unity.VisualScripting;
using UnityEngine;
using ITKombat;

public class GameManagerButton : MonoBehaviour
{
    [SerializeField] 
    private PlayerAttackTestNope playerAttack;
    private PlayerMovementNope playerMovement;

    private void Update() //debug
    {
        if (Input.GetKey(KeyCode.P) && playerAttack != null && !playerAttack.IsCrouching())
        {
            playerAttack.PerformAttack();
            Debug.Log("Attack");
        }
    }

    // Memulai crouch saat tombol ditekan
    public void OnCrouchButtonDown()
    {
        if (playerAttack != null)
        {
            Debug.Log("Mulai crouch");
            playerAttack.StartCrouch();
        }
    }
    public void OnCrouchButtonUp()
    {
        if (playerAttack != null)
        {
            Debug.Log("coruch berhenti");
            playerAttack.StopCrouch();
        }
    }

    public void AttackButton()
    {
        if (playerAttack != null)
        {
            Debug.Log("Attack");
            playerAttack.PerformAttack();
        }
    }

    public void CrouchAttackButton()
    {
        if (playerAttack != null)
        {
            Debug.Log("Crouh attack");
            playerAttack.PerformCrouchAttack();
        }
        else
        {
            Debug.LogWarning("PlayerAttackTestNope belum diassign!");
        }
    }

    // Button untuk movement
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

    // Button untuk jump
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
