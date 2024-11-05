using UnityEngine;

public class DoorPunch : MonoBehaviour
{
    public string targetSceneName; // Nama scene tujuan yang ingin dituju saat memukul
    [HideInInspector] public bool playerInRange = false; // Dapat diakses dari PlayerCombat

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            other.GetComponent<PlayerCombat>().SetPunchCollider(this); // Mengatur referensi collider
            Debug.Log("Player memasuki area bangunan");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player meninggalkan area bangunan");
        }
    }

    public bool CanActivateScene()
    {
        return playerInRange; // Mengembalikan true jika player berada di area
    }

    public void ActivateScene()
    {
        if (CanActivateScene() && !string.IsNullOrEmpty(targetSceneName)) // Pastikan player di range dan scene tujuan tidak kosong
        {
            SceneController.instance.LoadSceneByName(targetSceneName); // Pindah ke scene yang dituju
            Debug.Log("Beralih ke scene: " + targetSceneName);
        }
    }
}
