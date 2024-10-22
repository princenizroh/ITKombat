using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    private float timer = 0f;
    private bool playerInZone = false;
    private bool levelLoaded = false;

    [SerializeField] private string targetSceneName; // The name of the target scene

    private void Update()
    {
        if (playerInZone && !levelLoaded)
        {
            timer += Time.deltaTime;

            if (timer >= 2f)
            {
                levelLoaded = true;
                SceneController.instance.LoadSceneByName(targetSceneName); // Load target scene
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Player"))
    {
        playerInZone = true; // Player has entered the trigger zone
        PlayerSliderController playerSlider = collision.GetComponent<PlayerSliderController>();
        if (playerSlider != null)
        {
            playerSlider.SetFinishPoint(this); // Set the reference to this FinishPoint
        }
    }
}

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = false; // Player has exited the trigger zone
            timer = 0f; // Reset the timer
            levelLoaded = false; // Reset the level loaded flag

            PlayerSliderController playerSlider = collision.GetComponent<PlayerSliderController>();
            if (playerSlider != null)
            {
                playerSlider.ClearFinishPoint(); // Clear the reference to this FinishPoint
            }
        }
    }

    // Add helper methods to allow PlayerSliderController to access the timer and player status
    public bool IsPlayerInZone()
    {
        return playerInZone;
    }

    public float GetTimer()
    {
        return timer;
    }

}
