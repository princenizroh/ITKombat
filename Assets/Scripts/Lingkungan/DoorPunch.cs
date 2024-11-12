using UnityEngine;

namespace ITKombat
{
    public class DoorPunch : MonoBehaviour
    {
        public string targetSceneName; // Nama scene tujuan yang ingin dituju saat memukul

        public GameObject outline;
        [HideInInspector] public bool playerInRange = false; // Dapat diakses dari PlayerCombat

        private Vector2 playerPosition;

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

        public void TransitionToScene()
        {
            // Simpan posisi pemain sebelum pindah scene
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector2 playerPosition = player.transform.position;
                PlayerPrefs.SetFloat("PlayerPositionX", playerPosition.x);
                PlayerPrefs.SetFloat("PlayerPositionY", playerPosition.y);
            }

            // Pindah ke scene tujuan
            SceneController.instance.LoadSceneByName(targetSceneName);
            Debug.Log("Beralih ke scene: " + targetSceneName);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
                outline.SetActive(true);
                other.GetComponent<PlayerCombat>().SetPunchCollider(this); // Mengatur referensi collider
                Debug.Log("Player memasuki area bangunan");
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
                outline.SetActive(false);
                Debug.Log("Player meninggalkan area bangunan");
            }
        }

        public bool CanActivateScene()
        {
            return playerInRange; // Mengembalikan true jika player berada di area
        }
    }
}
