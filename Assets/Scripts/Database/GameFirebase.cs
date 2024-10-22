using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine.SceneManagement;

namespace ITKombat
{
    public class GameFirebase : MonoBehaviour
    {
        public static GameFirebase instance;
        public DependencyStatus dependencyStatus;
        public FirebaseUser User;
        public FirebaseAuth auth;
        public PlayerScriptableObject playerData;
        private CustomSceneManager customSceneManager;
        private string player_id;
        public DatabaseReference DBreference;
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            //Check that all of the necessary dependencies for Firebase are present on the system
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    //If they are avalible Initialize Firebase
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }

        void Start()
        {
            // Initialize the CustomSceneManager
            customSceneManager = new CustomSceneManager();
        }

        private void InitializeFirebase()
        {
            Debug.Log("Setting up Firebase Auth");
            //Set the authentication instance object
            auth = FirebaseAuth.DefaultInstance;
            DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        }

        // reff Task DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

        public async Task<bool> CompareId(string id_compared) {
            var DatabaseCompareTask = await DBreference.Child("players").GetValueAsync();

            if (DatabaseCompareTask.Exists) {
                foreach (var childSnapshot in DatabaseCompareTask.Children) {
                    string playerId = childSnapshot.Key;

                    if (id_compared == playerId) {
                        player_id = playerId;
                        Debug.Log("player id successfuly tagged");
                        playerData.player_id = playerId;
                        Debug.Log("player id inserted into playerdata");
                        return true;
                    }
                }
            }

            return false;
        }

        public IEnumerator GoogleRegister(string id) {
            var register = auth.CreateUserWithEmailAndPasswordAsync(id+"@itkombat.net", id);

            yield return new WaitUntil(predicate: () => register.IsCompleted);

            if (register.Exception != null) {
                Debug.Log("Account creation failed");
                
            } else {
                playerData.player_id = id;
                Debug.Log("Account created");
                InitializeNewAccount(id);
                StartCoroutine(firebaseAuhenticationLogin(id, id));
            }
        }

        public void InitializeNewAccount(string player_id)
        {
            // Define the data structure for the player using a dictionary
            var playerData = new Dictionary<string, object>
            {
                { "username", player_id },  // Set initial username
                { "email", player_id + "@itkombat.net" }, // Set initial email
                { "password", player_id }, // You might want to hash this before saving
                { "registration_date", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
            };

            var profileData = new Dictionary<string, object>
            {
                { "player_exp", 0 },  // Initial experience points
                { "player_rank", 0 }, // Initial rank
                { "sk2pm", 0 }        // Skill points
            };

            var balanceData = new Dictionary<string, object>
            {
                { "danus", 0 }, // Default currency balance
                { "ukt", 0 }    // Default UKK balance
            };

            // Push the data to Firebase under the player's ID
            DBreference.Child("players").Child(player_id).SetValueAsync(playerData); // player_id as the key
            DBreference.Child("profiles").Child(player_id).SetValueAsync(profileData); // player_id as the key for profiles
            DBreference.Child("balances").Child(player_id).SetValueAsync(balanceData); // player_id as the key for balances
        }

        public IEnumerator ChangeValueInteger(string player_id, string subject, string sub_subject, int value) {
            Task DBTask = DBreference.Child(subject).Child(player_id).Child(sub_subject).SetValueAsync(value);
            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null) {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            } else {
                Debug.Log("data changed succesfuly");
            }
        }

        public IEnumerator ChangeValueString(string player_id, string subject, string sub_subject, string value) {
            Task DBTask = DBreference.Child(subject).Child(player_id).Child(sub_subject).SetValueAsync(value);
            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
            
            if (DBTask.Exception != null) {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            } else {
                Debug.Log("data changed succesfuly");
            }
        }

        public void TestChange() {
            StartCoroutine(ChangeValueInteger("MgVD2xzBDjabUPE74MchcYV893G2", "players", "danus", 50));
        }

        public IEnumerator firebaseAuhenticationLogin(string email, string password) {

            var Login = auth.SignInWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(predicate: () => Login.IsCompleted);

            if (Login.Exception != null) {
                Debug.Log("Login sucessfuly");
                customSceneManager.LoadSceneByName("TopUp");
            } else {
                Debug.Log("Login failed");
            }

        }

        public IEnumerator TopUp(string player_id, string topup_type, int amount)
        {
            // Check for null or empty player_id and topup_type
            if (string.IsNullOrEmpty(player_id) || string.IsNullOrEmpty(topup_type))
            {
                Debug.LogError("player_id or topup_type is null or empty.");
                yield break; // Exit the coroutine
            }

            if (DBreference == null)
            {
                Debug.LogError("DBreference is not initialized. Ensure Firebase is set up correctly.");
                yield break; // Exit the coroutine
            }

            // Update the correct path
            Task DBTask = DBreference.Child("balances").Child(player_id).Child(topup_type).SetValueAsync(amount);
            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                Debug.Log("Data changed successfully.");
            }
        }

        public async Task<int?> GetBalanceData(string player_id, string type_data)
        {
            // Check for null or empty player_id and type_data
            if (string.IsNullOrEmpty(player_id) || string.IsNullOrEmpty(type_data))
            {
                Debug.LogError("player_id or type_data is null or empty.");
                return null;
            }

            if (DBreference == null)
            {
                Debug.LogError("DBreference is not initialized. Ensure Firebase is set up correctly.");
                return null;
            }

            // Log the path for debugging
            string path = $"balances/{player_id}/{type_data}";
            Debug.Log($"Accessing path: {path}");

            // Try to get the data from the database
            var DBTask = await DBreference.Child(path).GetValueAsync();

            if (DBTask != null && DBTask.Exists)
            {
                Debug.Log("data not null");
                // Try to convert the value to an integer
                if (int.TryParse(DBTask.Value.ToString(), out int balanceValue))
                {
                    return balanceValue; // Return the integer value
                }
            }
            else
            {
                Debug.LogError($"Data does not exist at path: {path}");
            }

            return 0;
        }

        public string GetPlayerId() {
            return player_id;
        }

    }
}
