using UnityEngine;

namespace ITKombat
{
    public class ObjectClickHandler : MonoBehaviour
    {
        [HideInInspector] public bool leaderboard = false; // Reference to the leaderboard canvas
        public GameObject leaderboardCanvas;
        public GameObject exitButton;
        public GameObject canvasDead;
        public GameObject outline;


        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                leaderboard = true;
                outline.SetActive(true);
                other.GetComponent<PlayerCombat>().setPialaCollider(this);
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
            if(leaderboardCanvas != null)
            {
                leaderboardCanvas.SetActive(true);
                exitButton.SetActive(true);
                canvasDead.SetActive(false);
                Debug.Log("CanvasActive");
            }
        }
    }
}
