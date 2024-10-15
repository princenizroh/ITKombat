using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorInteraction : MonoBehaviour
{
    public string sceneName;  // Assign the name of the scene for each door in the Inspector
    private bool playerInFrontOfDoor = false;
    private float timeSpentInFrontOfDoor = 0f;
    public float timeToEnter = 2f;  // Time required to enter the door (2 seconds)

    void Update()
    {
        if (playerInFrontOfDoor)
        {
            timeSpentInFrontOfDoor += Time.deltaTime;

            if (timeSpentInFrontOfDoor >= timeToEnter)
            {
                // Load the scene assigned to this door
                SceneManager.LoadScene(sceneName);
            }
        }
        else
        {
            // Reset the timer if the player leaves the door
            timeSpentInFrontOfDoor = 0f;
        }
    }

    // Detect when the player enters the trigger area in front of the door
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInFrontOfDoor = true;
        }
    }

    // Detect when the player leaves the trigger area in front of the door
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInFrontOfDoor = false;
        }
    }
}