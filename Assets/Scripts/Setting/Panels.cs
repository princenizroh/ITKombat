using UnityEngine;

namespace ITKombat
{
    public class Panels : MonoBehaviour
    {
           // Start is called before the first frame update
        [SerializeField] GameObject openPanel;

        public void PauseMenu()
        {
            openPanel.SetActive(true);
            Time.timeScale = 1f; // Mem-pause game
        }

        public void Lanjut()
        {
            openPanel.SetActive(false);
            Time.timeScale = 1f; // Melanjutkan game
        }

        public void ClosePanel()
        {
            openPanel.SetActive(false); // Hanya menutup panel
        }
    }
}
