using UnityEngine;
using UnityEngine.SceneManagement;

namespace ITKombat
{
    public class NextSceneManager : MonoBehaviour
    {
        public float delay;
        // Update is called once per frame
        public void LoadScene(string sceneName)
        {
            delay -= Time.deltaTime;
            if (delay <= 0)
            {
                SceneManager.LoadScene(sceneName);
            }
        
        }
    }
}
