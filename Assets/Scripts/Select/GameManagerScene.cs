using System;
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
                int selectedPageID = PlayerPrefs.GetInt("SelectedPageID", 1); // Default ke ID 1 jika tidak ada
                SelectScene selectedScene = Array.Find(scenes, scene => scene.id == selectedPageID);

                if (selectedScene != null)
                {
                    currentScene = selectedScene;
                    Debug.Log($"Scene dengan ID {selectedPageID} dipilih.");
                    Instantiate(currentScene.prefab, transform.position, Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning("Scene tidak ditemukan untuk ID yang dipilih!");
                }
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
        }
    }
    
    [System.Serializable]
    public class SelectScene
    {
        public int id; 
        public GameObject prefab; 
    }

}
