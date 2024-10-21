using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    [SerializeField] Animator transitionAnim;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Load next level based on build index
    public void NextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    // Load specific scene by name
    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadLevel(sceneName));
    }

    // Coroutine for handling scene transition
    IEnumerator LoadLevel(int buildIndex)
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(buildIndex);
        transitionAnim.SetTrigger("Start");
    }

    IEnumerator LoadLevel(string sceneName)
{
    Debug.Log("Starting scene transition to: " + sceneName);  // Log the scene name
    transitionAnim.SetTrigger("End");
    yield return new WaitForSeconds(1);
    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
    while (!asyncLoad.isDone)
    {
        yield return null;
    }
    transitionAnim.SetTrigger("Start");
    Debug.Log("Scene transition to: " + sceneName + " completed");
}

}
