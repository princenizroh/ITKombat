using UnityEngine;

public class GameManagerButton : MonoBehaviour
{
    [SerializeField] 
    private PlayerAttackTestNope playerAttack;

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
}
