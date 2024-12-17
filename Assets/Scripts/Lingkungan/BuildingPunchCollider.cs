using UnityEngine;

namespace ITKombat
{
    public class BuildingPunchCollider : MonoBehaviour
    {
        public GameObject targetCanvas;
        public GameObject canvasDead1;
        public GameObject canvasDead2;
        public GameObject outline;
        [HideInInspector] public bool playerInRange = false; // Dapat diakses dari PlayerComba

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
                outline.SetActive(true);
                other.GetComponent<PlayerCombat>().SetBuildingCollider(this); // Mengatur referensi collider
                Debug.Log("Player memasuki area bangunan");
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
                outline.SetActive(false);
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
                canvasDead1.SetActive(false);
                canvasDead2.SetActive(false);
                Debug.Log("Canvas diaktifkan");
                Debug.Log("HUD Dinonaktifkan");
            }
        }
    }
}
