using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneAndRemovePersistantManager : MonoBehaviour
{
    public string targetSceneName;
    public string managerName; // Nama Game Manager dari scene Asrama yang akan dihapus

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Cari dan hapus Game Manager yang ada di scene sebelumnya
            GameObject gameManagerToRemove = GameObject.Find(managerName);

            if (gameManagerToRemove != null)
            {
                Destroy(gameManagerToRemove);
            }

            // Pindah ke scene target
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
