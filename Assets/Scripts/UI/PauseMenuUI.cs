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
        private void Awake()
        {
            resumeButton.onClick.AddListener(() =>
            {
                Debug.Log("Resume Called");
                Hide();
            });

            settingsButton.onClick.AddListener(() =>
            {
                Debug.Log("Settings Called");
            });

            guideButton.onClick.AddListener(() =>
            {
                Debug.Log("Guide Called");
            });

            exitButton.onClick.AddListener(() =>
            {
                Debug.Log("Exit Called");
            });
        }

        private void Show()
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

        private void HideSettings()
        {
            SettingsPanel.SetActive(false);
        }

        private void ShowGuide()
        {
            GuidePanel.SetActive(true);
        }

        private void HideGuide()
        {
            GuidePanel.SetActive(false);
        }

        private void Exit()
        {
            
        }
    }
}
