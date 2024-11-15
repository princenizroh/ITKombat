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
            if (!leaderboardCanvas.activeSelf)
            {
                leaderboardCanvas.SetActive(true);
            }
        }
    }
}
