using UnityEngine;

namespace ITKombat
{
    public class Exit : MonoBehaviour
    {
        public void QuitGame()
        {
            Debug.Log("Anda Keluar");
            Application.Quit();
        }
    }
}