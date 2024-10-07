using ITKombat;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerButton : MonoBehaviour
{
     [SerializeField] 
    private PlayerAttackTestNope playerAttack; 
    [SerializeField] 
    private PlayerMovementNope playerMovement; 

    private float lastTapTime = 0f;
    private const float doubleTapDelay = 0.3f;

    private void Update() 
    {
        if (Input.GetKey(KeyCode.P) && playerAttack != null && !playerAttack.IsCrouching())
        {
            playerAttack.PerformAttack();
            Debug.Log("Attack");
        }
    }

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
            Debug.Log("Crouch berhenti");
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
            Debug.Log("Crouch attack");
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
            HandleDash(true); 
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
            HandleDash(false); 
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

    private void HandleDash(bool isRightButton)
    {
        float tapDelay = Time.time - lastTapTime;

        if (tapDelay < doubleTapDelay)
        {
            if (isRightButton)
            {
                playerMovement.Dash(1); 
                Debug.Log("Dash right!");
            }
            else
            {
                playerMovement.Dash(-1); 
                Debug.Log("Dash left!");
            }
        }

        lastTapTime = Time.time;
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
