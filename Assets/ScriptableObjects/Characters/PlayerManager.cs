using ITKombat;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerScriptableObject playerData;

    [System.Obsolete]
    private void Start()
    {
        // Contoh pengaturan berdasarkan data ScriptableObject
        if (playerData != null)
        {
            // Mengakses data player
            Debug.Log("Player Name: " + playerData.playerName);
            Debug.Log("Max Health: " + playerData.maxHealth);
            Debug.Log("Move Speed: " + playerData.moveSpeed);

            // Menginisialisasi komponen player dengan script yang berbeda
            playerData.audioPlayerTest.PlayBackgroundMusic();
            playerData.characterController.Move(playerData.moveSpeed, false, false);
            playerData.playerState.UpdateHealth();
            playerData.playerAttack.PerformAttack();
        }
    }
}
