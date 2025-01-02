using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

namespace ITKombat
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button guideButton;
        [SerializeField] private Button guideExitButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private GameObject SettingsPanel;
        [SerializeField] private GameObject GuidePanel;

        private static PauseMenuUI Instance;
        private void Awake()
        {
            Instance = this;
            
            resumeButton.onClick.AddListener(() =>
            {
                Debug.Log("Resume Called");
                Hide();
                NewSoundManager.Instance.PlaySound2D("Button_Click");
            });

            settingsButton.onClick.AddListener(() =>
            {
                Debug.Log("Settings Called");
                ShowSettings();
                NewSoundManager.Instance.PlaySound2D("Button_Click");
            });

            guideButton.onClick.AddListener(() =>
            {
                Debug.Log("Guide Called");
                ShowGuide();
                NewSoundManager.Instance.PlaySound2D("Button_Click");
            });

            guideExitButton.onClick.AddListener(() =>
            {
                Debug.Log("Guide Exit Called");
                ExitGuide();
                NewSoundManager.Instance.PlaySound2D("Button_Click");
            });

            exitButton.onClick.AddListener(() =>
            {
                Debug.Log("Exit Called");
                Exit();
                NewSoundManager.Instance.PlaySound2D("Button_Click");
            });
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void ShowSettings()
        {
            SettingsPanel.SetActive(true);
        }

        private void ShowGuide()
        {
            GuidePanel.SetActive(true);
        }

        private void ExitGuide()
        {
            GuidePanel.SetActive(false);
        }

        private void Exit()
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.Lingkungan);
        }
    }

}
