using UnityEngine;
using UnityEngine.SceneManagement;

namespace ITKombat
{
    public class SoundSceneManager : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            Debug.Log(currentSceneName + " adalah scene sekarang");
            if (currentSceneName == "SoundMultiplayer")
            {
                MusicManager.Instance.PlayMusic("Battle_2");
            }
            else if (currentSceneName == "Asrama")
            {
                MusicManager.Instance.PlayMusic("Mars_ITKombat");
            }
            else if (currentSceneName == "(new)LoginRegister")
            {
                MusicManager.Instance.PlayMusic("Mars_ITKombat");
            }
            else if(currentSceneName == "BattleMode")
            {
                MusicManager.Instance.PlayMusic("Battle_1");
            }
            else if(currentSceneName == "Lingkungan")
            {
                MusicManager.Instance.PlayMusic("Mars_ITKombat");
            }
            // else if(currentSceneName == "NewSoundMulti")
            // {
            //     MusicManager.Instance.PlayMusic("Battle_2");
            // }
        }
    };
};
