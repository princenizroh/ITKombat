using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    [SerializeField] Animator transitionAnim;
    private Vector2 playerPosition;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Load specific scene by name
    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadLevel(sceneName));
    }

    // Coroutine for handling scene transition
    // Metode untuk mengatur posisi pemain
    IEnumerator LoadLevel(string sceneName)
    {
        Debug.Log("Starting scene transition to: " + sceneName);  // Log the scene name
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Wait until the scene is loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        transitionAnim.SetTrigger("Start");
        
        Debug.Log("Scene transition to: " + sceneName + " completed");
        
    }

}
