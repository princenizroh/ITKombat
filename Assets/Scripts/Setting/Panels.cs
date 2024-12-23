using UnityEngine;

namespace ITKombat
{
    public class Panels : MonoBehaviour
    {
        public static Panels Instance; // Singleton instance
        [SerializeField] GameObject openPanel; // Panel yang ingin dibuka


        void Awake()
        {
            // Cek apakah Instance sudah ada, jika ada hancurkan objek ini
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Menjaga agar panel tetap ada saat berpindah scene
            }
            // else
            // {
            //     Destroy(gameObject); // Hancurkan objek yang duplikat
            // }

            
        }

        public void PauseMenu()
        {
            openPanel.SetActive(true);
            Time.timeScale = 0f; // Menghentikan waktu
            Debug.Log("Panel Terbuka");
        }

        public void Lanjut()
        {
            openPanel.SetActive(false);
            Time.timeScale = 1f; // Melanjutkan game
        }

        public void QuitGame()
        {
            Debug.Log("Anda Keluar");
            Application.Quit(); // Keluar dari aplikasi
        }
    }
}
