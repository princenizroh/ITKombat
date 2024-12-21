using UnityEngine;
using UnityEngine.SceneManagement;

namespace ITKombat
{
    public class NextSceneManager : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

    }
}
