using UnityEngine;
using UnityEngine.UI;

namespace ITKombat
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button guideButton;
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
            });

            settingsButton.onClick.AddListener(() =>
            {
                Debug.Log("Settings Called");
                ShowSettings();
            });

            guideButton.onClick.AddListener(() =>
            {
                Debug.Log("Guide Called");
                ShowGuide();
            });

            exitButton.onClick.AddListener(() =>
            {
                Debug.Log("Exit Called");
                Exit();
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

        private void Exit()
        {
            Loader.Load(Loader.Scene.Lingkungan);
        }
    }

}
