using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNameDisplay : MonoBehaviour
{
    void Start()
    {
        // Mendapatkan nama scene saat ini
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log(currentSceneName + "adalah scene sekarang");
        if (currentSceneName == "SoundMultiplayer")
        {
            MusicManager.Instance.PlayMusic("Battle_1");
        }
        else if (currentSceneName == "Asrama")
        {
            MusicManager.Instance.PlayMusic("MarsITKombat");
        }
        // Menampilkan nama scene di console
        Debug.Log("Nama scene saat ini: " + currentSceneName);

        //Nanti tambahkan else StopMusic()
    }
}
