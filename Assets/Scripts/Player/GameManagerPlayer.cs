using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManagerPlayer : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerAttack playerAttack;
    public PlayerDefense playerDefense;

    // Attack
    public void AttackInput()
    {
        playerAttack.OnAttackInput();
    }

    // Defense
    public void DefenseInputButtonDown()
    {
        playerDefense.StartBlocking();
    }

    public void DefenseInputButtonUp()
    {
        playerDefense.EndBlocking();
    }

    // Movement
    public void CrouchInputButtonDown()
    {
        playerMovement.Crouch();
    }

    public void CrouchInputButtonUp()
    {
        playerMovement.StopCrouch();
    }

    public void LeftInputButtonDown()
    {
        playerMovement.MoveLeft();
    }

    public void LeftInputButtonUp()
    {
        playerMovement.StopMoveLeft();
    }

    public void RightInputButtonDown()
    {
        playerMovement.MoveRight();
    }

    public void RightInputButtonUp()
    {
        playerMovement.StopMoveRight();
    }
    
    //jump
    public void OnJumpInput()
    {
        playerMovement.JumpInput();
    }
}