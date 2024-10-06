using ITKombat;
using Unity.VisualScripting;
using UnityEngine;

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
            Debug.Log("crouch");
            playerAttack.StartCrouch();
        }
        else
        {
            Debug.LogWarning("PlayerAttackTestNope belum diassign!");
        }
    }

    // Menghentikan crouch saat tombol dilepas
    public void OnCrouchButtonUp()
    {
        if (playerAttack != null)
        {
            Debug.Log("berhenti crouch");
            playerAttack.StopCrouch();
        }
    }

    // Tombol untuk serangan
    public void AttackButton()
    {
        if (playerAttack != null)
        {
            Debug.Log("GameManagerButton pressed: Memulai serangan");
            playerAttack.PerformAttack();
        }
        else
        {
            Debug.LogWarning("PlayerAttackTestNope belum diassign!");
        }
    }

    // Tombol untuk serangan crouch
    public void CrouchAttackButton()
    {
        if (playerAttack != null)
        {
            Debug.Log("Crouch Attack triggered!");
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
