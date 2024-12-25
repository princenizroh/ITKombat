using UnityEngine;
using UnityEngine.TextCore.Text;

namespace ITKombat
{
    public class GameManagerSelect : MonoBehaviour
    {
        public static GameManagerSelect instance;
        public SelectCharFelix[] characters;

        public SelectCharFelix[] aiCharacters;
        public SelectCharFelix currentCharacter; // Player Character
        public SelectCharFelix currentAICharacter; // AI Character

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
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (characters.Length > 0)
            {
                currentCharacter = characters[0];
                currentAICharacter = aiCharacters[0]; // Default AI character
            }
        }

        public void SetCharacter(SelectCharFelix character)
        {
            currentCharacter = character;
        }

        public void SetAICharacter(SelectCharFelix character)
        {
            currentAICharacter = character;
        }
    }
}
