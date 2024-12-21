using UnityEngine;
using UnityEngine.SceneManagement;

namespace ITKombat{
    public class FinishPoint : MonoBehaviour
{
    private float timer = 0f;
    private bool playerInZone = false;
    private bool levelLoaded = false;
    private Vector2 playerPosition;
    
    [SerializeField] private string targetSceneName; // Nama scene tujuan

    private void Start()
    {
        // Cek jika ada data posisi terakhir pemain
        if (PlayerPrefs.HasKey("PlayerPositionX") && PlayerPrefs.HasKey("PlayerPositionY"))
        {
            float x = PlayerPrefs.GetFloat("PlayerPositionX");
            float y = PlayerPrefs.GetFloat("PlayerPositionY");

            // Set posisi pemain sesuai data terakhir di scene sebelumnya
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = new Vector2(x, y);
            }
        }
    }

    private void Update()
    {
        if (playerInZone && !levelLoaded)
        {
            timer += Time.deltaTime;

            if (timer >= 2f)
            {
                levelLoaded = true;

                // Simpan posisi pemain sebelum pindah scene
                PlayerPrefs.SetFloat("PlayerPositionX", playerPosition.x);
                PlayerPrefs.SetFloat("PlayerPositionY", playerPosition.y);

                // Pindah ke scene tujuan
                SceneController.instance.LoadSceneByName(targetSceneName);
                
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = true; // Pemain masuk zona trigger
            playerPosition = collision.transform.position; // Simpan posisi pemain saat memasuki zona

            PlayerSliderController playerSlider = collision.GetComponent<PlayerSliderController>();
            if (playerSlider != null)
            {
                playerSlider.SetFinishPoint(this); // Referensi ke FinishPoint
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = false; // Pemain keluar dari zona trigger
            timer = 0f; // Reset timer
            levelLoaded = false; // Reset flag level

            PlayerSliderController playerSlider = collision.GetComponent<PlayerSliderController>();
            if (playerSlider != null)
            {
                playerSlider.ClearFinishPoint(); // Bersihkan referensi ke FinishPoint
            }
        }
    }

    // Tambahkan metode untuk mengakses status pemain
    public bool IsPlayerInZone() => playerInZone;
    public float GetTimer() => timer;
}
}

