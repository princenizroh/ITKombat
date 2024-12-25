using TMPro;
using UnityEngine;

namespace ITKombat
{
    public class SelectCharacterFirstTime : MonoBehaviour
    {
        public static SelectCharacterFirstTime instance;
        public PlayerScriptableObject playerData;
        [SerializeField] private CustomSceneManager customSceneManager;
        [SerializeField] private GameFirebase gameFirebase;
        public TMP_Text informationShows;
        public TMP_InputField username;
        private string usernamePicked;

        void Start() {

            gameFirebase = GameFirebase.instance;

        }

        void Awake() {
            if (instance == null)
            {
                instance = this;
            }

            // Initialize customSceneManager if null
            if (customSceneManager == null)
            {
                customSceneManager = Object.FindFirstObjectByType<CustomSceneManager>();
                if (customSceneManager == null)
                {
                    Debug.LogError("CustomSceneManager is missing in the scene!");
                }
            }

            // Initialize gameFirebase if null
            if (gameFirebase == null)
            {
                gameFirebase = Object.FindFirstObjectByType<GameFirebase>();
                if (gameFirebase == null)
                {
                    Debug.LogError("GameFirebase is missing in the scene!");
                }
            }

            if (playerData == null)
            {
                Debug.LogError("PlayerScriptableObject is not assigned!");
            }
        }


        public void pushDataIntoDatabase() {
            usernamePicked = username.text;
            playerData.playerName = usernamePicked;
            StartCoroutine(gameFirebase.ChangeValueString(playerData.player_id, "players", "username", usernamePicked));
            customSceneManager.LoadSceneByName("Asrama");
        }
    }
}
