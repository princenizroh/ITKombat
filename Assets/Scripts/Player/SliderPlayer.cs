using UnityEngine;
using UnityEngine.UI;


namespace ITKombat{
    public class PlayerSliderController : MonoBehaviour
{
    private FinishPoint finishPoint; // Reference to the FinishPoint script
    [SerializeField] private Slider progressBar; // Reference to the slider UI

    private void Start()
    {
        progressBar.gameObject.SetActive(false); // Hide slider at the start
    }

    private void Update()
    {
        if (finishPoint != null && finishPoint.IsPlayerInZone()) // Check if the player is in the finish zone
        {
            progressBar.gameObject.SetActive(true); // Show the slider
            progressBar.value = finishPoint.GetTimer() / 2f; // Update the slider progress based on timer

            if (finishPoint.GetTimer() >= 2f)
            {
                progressBar.gameObject.SetActive(false); // Hide slider after scene change
            }
        }
        else
        {
            progressBar.gameObject.SetActive(false); // Hide the slider when the player leaves the zone
            progressBar.value = 0f; // Reset the slider progress
        }
    }

    // Method to set the reference to the FinishPoint script when player enters a zone
    public void SetFinishPoint(FinishPoint point)
    {
        finishPoint = point;
    }

    // Method to clear the reference when the player exits the zone
    public void ClearFinishPoint()
    {
        finishPoint = null;
    }
}

}
