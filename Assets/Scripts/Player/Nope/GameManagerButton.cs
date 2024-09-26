using UnityEngine;

public class GameManagerButton : MonoBehaviour
{
    [SerializeField] 
    private PlayerAttackTestNope playerAttack;

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
}
