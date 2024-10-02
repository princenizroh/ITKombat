using UnityEngine;
using UnityEngine.SceneManagement;

namespace ITKombat
{
    public class LoginPageUIManager : MonoBehaviour
    {
        public static LoginPageUIManager instance;

        public GameObject loginPanel;
        public GameObject registerPanel;
        public GameObject startPanel;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoginScreen()
        {
            loginPanel.SetActive(true);
            registerPanel.SetActive(false);
            startPanel.SetActive(false);
        }

        public void RegisterScreen()
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(true);
            startPanel.SetActive(false);
        }

        public void StartScreen()
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(false);
            startPanel.SetActive(true);
        }

        public void LobbyScreen()
        {
            SceneManager.LoadScene("Multiplayer");
        }
    }
}
