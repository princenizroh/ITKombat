using UnityEngine;
using UnityEngine.UI;

public class FinishPoint : MonoBehaviour
{
    private float timer = 0f;
    private bool playerInZone = false;
    private bool levelLoaded = false;

    [SerializeField] private string targetSceneName; // The name of the target scene
    [SerializeField] private Slider progressBar; // Reference to the UI Slider

    // Method called at the start of the game
    private void Start()
    {
        progressBar.gameObject.SetActive(false); // Hide progress bar at the start
    }

    private void Update()
    {
        if (playerInZone && !levelLoaded)
        {
            timer += Time.deltaTime;
            progressBar.value = timer / 2f; // Fill progress bar over 2 seconds

            if (timer >= 2f)
            {
                levelLoaded = true;
                SceneController.instance.LoadSceneByName(targetSceneName);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = true; // Player has entered the trigger zone
            progressBar.gameObject.SetActive(true); // Show progress bar
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = false; // Player has exited the trigger zone
            timer = 0f; // Reset the timer
            levelLoaded = false; // Reset the level loaded flag
            progressBar.gameObject.SetActive(false); // Hide progress bar
            progressBar.value = 0f; // Reset progress bar value
        }
    }
}
