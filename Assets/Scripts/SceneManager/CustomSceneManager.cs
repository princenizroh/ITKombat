using UnityEngine;
using UnityEngine.SceneManagement;

namespace ITKombat
{
    public class CustomSceneManager : MonoBehaviour
    {

        public void LoadSceneByName(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    
    }
}
