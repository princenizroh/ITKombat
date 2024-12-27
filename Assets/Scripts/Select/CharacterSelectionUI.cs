using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ITKombat
{
    public class CharacterSelectionUI : MonoBehaviour
    {
        public GameObject optionPrefab;
        public Transform prevCharacter;
        public Transform selectedCharacter;
        public Transform selectedAICharacter; // Untuk menyorot pilihan AI
        public bool isSelectingForAI = false; // Menentukan apakah sedang memilih untuk AI atau pemain

        private void Start()
        {
            LoadCharacterOptions();
        }

        private void Update()
        {
            AnimateSelection(selectedCharacter, new Vector3(1.2f, 1.2f, 1.2f));
            AnimateSelection(selectedAICharacter, new Vector3(1.2f, 1.2f, 1.2f));
            AnimateSelection(prevCharacter, new Vector3(1f, 1f, 1f));
        }

        private void LoadCharacterOptions()
        {
            // Hapus opsi karakter sebelumnya
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // Tentukan daftar karakter berdasarkan mode pemilihan
            SelectCharFelix[] characters = isSelectingForAI ? GameManagerSelect.instance.aiCharacters : GameManagerSelect.instance.characters;

            foreach (SelectCharFelix c in characters)
            {
                GameObject option = Instantiate(optionPrefab, transform);
                Button button = option.GetComponent<Button>();

                // Referensi ke StatusText di prefab
                TextMeshProUGUI statusText = option.transform.Find("StatusText").GetComponent<TextMeshProUGUI>();
                statusText.text = ""; // Awalnya kosong

                button.onClick.AddListener(() => HandleCharacterSelection(c, option.transform, statusText));

                Image image = option.GetComponentInChildren<Image>();
                image.sprite = c.icon;

                TextMeshProUGUI text = option.GetComponentInChildren<TextMeshProUGUI>();
                text.text = c.name;
            }
        }

        private void HandleCharacterSelection(SelectCharFelix character, Transform optionTransform, TextMeshProUGUI statusText)
        {
            NewSoundManager.Instance.PlaySound2D("Button_Click");
            if (isSelectingForAI)
            {
                GameManagerSelect.instance.SetAICharacter(character);

                if (selectedAICharacter != null)
                {
                    ResetStatusText(selectedAICharacter);
                    prevCharacter = selectedAICharacter;
                }

                selectedAICharacter = optionTransform;
                statusText.text = "COM"; // Beri label untuk AI
            }
            else
            {
                GameManagerSelect.instance.SetCharacter(character);

                if (selectedCharacter != null)
                {
                    ResetStatusText(selectedCharacter);
                    prevCharacter = selectedCharacter;
                }

                selectedCharacter = optionTransform;
                statusText.text = "P1"; // Beri label untuk pemain
            }
        }

        private void ResetStatusText(Transform characterTransform)
        {
            if (characterTransform.Find("StatusText")?.TryGetComponent(out TextMeshProUGUI statusText) == true)
            {
                statusText.text = ""; // Kosongkan status sebelumnya
            }
        }

        private void AnimateSelection(Transform target, Vector3 targetScale)
        {
            if (target != null)
            {
                target.localScale = Vector3.Lerp(target.localScale, targetScale, Time.deltaTime * 10);
            }
        }

        public void ToggleSelectionForAI(bool isForAI)
        {
            isSelectingForAI = isForAI;
            LoadCharacterOptions(); // Muat ulang daftar karakter
        }
    }
}
