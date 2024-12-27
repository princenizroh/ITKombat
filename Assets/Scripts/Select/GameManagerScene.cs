using UnityEngine;

namespace ITKombat
{
    public class GameManagerScene : MonoBehaviour
    {
        public static GameManagerScene instance; 
        public SelectScene[] scenes; 
        public SelectScene currentScene; 

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

        private void Start()
        {
            if (scenes != null && scenes.Length > 0)
            {
                currentScene = scenes[0]; // Default to the first scene in the array
            }
            else
            {
                Debug.LogWarning("No scenes available in the scenes array!");
            }
        }

        // Method to set the current scene
        public void SetScene(SelectScene scene)
        {
            if (scene == null)
            {
                Debug.LogWarning("Trying to set a null scene!");
                return;
            }

            currentScene = scene;
            Debug.Log($"Current scene set to: {scene.Id}");
        }
    }

    [System.Serializable]
    public class SelectScene
    {
        public string Id; // Name of the scene
        public GameObject prefab; // Prefab to spawn for the scene
    }
}
