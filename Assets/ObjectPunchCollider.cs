using UnityEngine;

public class BuildingPunchCollider : MonoBehaviour
{
    public GameObject targetCanvas;
    [HideInInspector] public bool playerInRange = false; // Dapat diakses dari PlayerCombat

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            other.GetComponent<PlayerCombat>().SetBuildingCollider(this); // Mengatur referensi collider
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

    public bool CanActivateCanvas()
    {
        return playerInRange; // Mengembalikan true jika player berada di area
    }

    public void ActivateCanvas()
    {
        if (targetCanvas != null)
        {
            targetCanvas.SetActive(true);
            Debug.Log("Canvas diaktifkan");
        }
    }
}