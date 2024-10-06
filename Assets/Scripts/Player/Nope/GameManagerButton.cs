using UnityEngine;
using ITKombat;

public class GameManagerButton : MonoBehaviour
{
    [SerializeField] 
    private PlayerAttackTestNope playerAttack; 
    
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
    }
}
