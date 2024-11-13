using UnityEngine;

namespace ITKombat
{
    public class ObjectClickHandler : MonoBehaviour
    {
        [SerializeField] private GameObject leaderboardCanvas; // Reference to the leaderboard canvas

        private void Start()
        {
            leaderboardCanvas.SetActive(false); // Hide leaderboard at the start
        }

        private void OnMouseDown()
        {
            Debug.Log("Canvas Terklik"); // Tes apakah metode ini terpicu
            if (!leaderboardCanvas.activeSelf)
            {
                leaderboardCanvas.SetActive(true);
            }
        }
    }
}
