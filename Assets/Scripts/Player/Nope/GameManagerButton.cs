using UnityEngine;
using ITKombat;

public class GameManagerButton : MonoBehaviour
{
    [SerializeField] 
    private PlayerAttackTestNope playerAttack;
    [SerializeField] 
    private PlayerMovementNope playerMovement;
    
    public void OnCrouchButtonDown()
    {
        if (playerAttack != null)
        {
            playerAttack.StartCrouch();
        }
    }

    // Menghentikan crouch saat tombol dilepas
    public void OnCrouchButtonUp()
    {
        if (playerAttack != null)
        {
            playerAttack.StopCrouch();
        }
    }

    // Tombol untuk serangan
    public void AttackButton()
    {
        if (playerAttack != null)
        {
            playerAttack.PerformAttack();
        }
    }

    public void CrouchAttackButton()
    {
        if (playerAttack != null)
        {
            playerAttack.PerformCrouchAttack();
        }
    }
}
