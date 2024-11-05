using UnityEngine;

public class ObjectClickHandler : MonoBehaviour
{
    [SerializeField] private GameObject leaderboardCanvas; // Reference to the leaderboard canvas

    private void Start()
    {
        leaderboardCanvas.SetActive(false); // Hide leaderboard at the start
    }

    private void OnMouseDown()
    {
        // Check if the leaderboard is not already active
        if (!leaderboardCanvas.activeSelf)
        {
            leaderboardCanvas.SetActive(true); // Show the leaderboard canvas when the trophy is clicked
        }
    }
}
