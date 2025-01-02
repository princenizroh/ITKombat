using UnityEngine;

namespace ITKombat
{
    public class NewPlayerHelpText : MonoBehaviour
    {
        [HideInInspector] public bool leaderboard = false; // Reference to the leaderboard canvas
        public GameObject bookCanvas;
        public GameObject canvasDead;
        public GameObject newCanvas;
        public GameObject outline;

        private static bool hasActivated = false;


      void Start()
      {
        // Cek dari PlayerPrefs apakah newCanvas sudah pernah diaktifkan
            if (hasActivated)
            {
                // Jika sudah diaktifkan sebelumnya, kedua canvas dimatikan
                canvasDead.SetActive(false);
                newCanvas.SetActive(false);
            }
            else
            {
                // Jika belum pernah diaktifkan, canvasDead tetap aktif
                canvasDead.SetActive(true);
                newCanvas.SetActive(false); // newCanvas dimulai dalam keadaan tidak aktif
            }
      }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                leaderboard = true;
                outline.SetActive(true);
                canvasDead.SetActive(false);
                if(!hasActivated)
                {
                newCanvas.SetActive(true);
                hasActivated = true;
                Debug.Log("New Text Activated for once");
                }
                other.GetComponent<PlayerCombat>().setFirstTextCollider(this);
                Debug.Log("Masuk collider");
            }
        }

        void OnTriggerExit2D(Collider2D other) 
        {
            if (other.CompareTag("Player"))
            {
                leaderboard = false;
                outline.SetActive(false);
                Debug.Log("Keluar collider");
            } 
        }

        public bool CanActivateCanvas()
        {
            return leaderboard;
        }

        public void ActivateCanvas()
        {
            if(bookCanvas != null)
            {
                newCanvas.SetActive(false);
                Debug.Log("CanvasActive");
            }
        }
    }
}
