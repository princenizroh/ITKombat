using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

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
        public LoginPageUIManager loginPageUIManager;
        public class InventoryItem
        {
            public int item_id { get; set; }
            public int item_level { get; set; }
            public int item_ascend { get; set; }
            public int item_exp_max { get; set; }
            public int item_current_exp { get; set; }
            public int item_id_type_1 { get; set; }
            public int item_id_type_2 { get; set; }
            public int item_value_type_1 { get; set; }
            public int item_value_type_2 { get; set; }
        }
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
                        Debug.Log("player id successfully tagged");
                        playerData.player_id = playerId;
                        Debug.Log("player id inserted into playerData");

                        var DatabaseCompareName = await DBreference.Child("players").Child(playerId).Child("username").GetValueAsync();
                        var DatabaseUkt = await DBreference.Child("balances").Child(playerId).Child("ukt").GetValueAsync();
                        var DatabaseDanus = await DBreference.Child("balances").Child(playerId).Child("danus").GetValueAsync();

                        if (DatabaseCompareName.Exists) {
                            playerData.playerName = DatabaseCompareName.Value.ToString();
                            Debug.Log("player name successfully tagged");
                        }

                        if (DatabaseUkt.Exists) {
                            playerData.playerUkt = int.Parse(DatabaseUkt.Value.ToString());
                            Debug.Log("player ukt successfully tagged");
                        }

                        if (DatabaseDanus.Exists) {
                            playerData.playerDanus = int.Parse(DatabaseDanus.Value.ToString());
                            Debug.Log("player danus successfully tagged");
                        }
                        
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
                _ = CompareId(id);
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

            var inventoryData = new Dictionary<string, object>
            {
                { "item_id", 0 },
                { "item_level", 0},
                { "item_ascend", 0 },
                { "item_exp_max", 0},
                { "item_current_exp", 0},
                { "item_id_type_1", 0 },
                { "item_id_type_2", 0 },
                { "item_value_type_1", 0 },
                { "item_value_type_2", 0 }
            };

            // Push the data to Firebase under the player's ID
            DBreference.Child("players").Child(player_id).SetValueAsync(playerData); // player_id as the key
            DBreference.Child("profiles").Child(player_id).SetValueAsync(profileData); // player_id as the key for profiles
            DBreference.Child("balances").Child(player_id).SetValueAsync(balanceData); // player_id as the key for balances
            DBreference.Child("inventory").Child(player_id).Child("1").SetValueAsync(inventoryData); // player inventorty data value starter
        }

        public IEnumerator firebaseAuhenticationLogin(string email, string password) {
            var Login = auth.SignInWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(predicate: () => Login.IsCompleted);

            if (Login.Exception != null) {
                Debug.Log("Login sucessfuly");
                loginPageUIManager.StartScreen();
            } else {
                Debug.Log("Login failed");
            }

        }

        // public async Task<int?> GetBalanceData(string player_id, string type_data)
        // {
        //     // Check for null or empty player_id and type_data
        //     if (string.IsNullOrEmpty(player_id) || string.IsNullOrEmpty(type_data))
        //     {
        //         Debug.LogError("player_id or type_data is null or empty.");
        //         return null;
        //     }

        //     if (DBreference == null)
        //     {
        //         Debug.LogError("DBreference is not initialized. Ensure Firebase is set up correctly.");
        //         return null;
        //     }

        //     // Log the path for debugging
        //     string path = $"balances/{player_id}/{type_data}";
        //     Debug.Log($"Accessing path: {path}");

        //     // Try to get the data from the database
        //     var DBTask = await DBreference.Child(path).GetValueAsync();

        //     if (DBTask != null && DBTask.Exists)
        //     {
        //         Debug.Log("data not null");
        //         // Try to convert the value to an integer
        //         if (int.TryParse(DBTask.Value.ToString(), out int balanceValue))
        //         {
        //             return balanceValue; // Return the integer value
        //         }
        //     }
        //     else
        //     {
        //         Debug.LogError($"Data does not exist at path: {path}");
        //     }

        //     return 0;
        // }

        // public async Task<List<InventoryItem>> GetInventoryDataAsync(string player_id)
        // {
        //     var inventoryList = new List<InventoryItem>();
        //     var DBTask = await DBreference.Child("inventory").Child(player_id).GetValueAsync();

        //     if (DBTask.Exists)
        //     {
        //         foreach (var childSnapshot in DBTask.Children)
        //         {
        //             InventoryItem item = new InventoryItem
        //             {
        //                 item_id = int.Parse(childSnapshot.Child("item_id").Value.ToString()),
        //                 item_level = int.Parse(childSnapshot.Child("item_level").Value.ToString()),
        //                 item_ascend = int.Parse(childSnapshot.Child("item_ascend").Value.ToString()),
        //                 item_exp_max = int.Parse(childSnapshot.Child("item_exp_max").Value.ToString()),
        //                 item_current_exp = int.Parse(childSnapshot.Child("item_current_exp").Value.ToString()),
        //                 item_id_type_1 = int.Parse(childSnapshot.Child("item_id_type_1").Value.ToString()),
        //                 item_id_type_2 = int.Parse(childSnapshot.Child("item_id_type_2").Value.ToString()),
        //                 item_value_type_1 = int.Parse(childSnapshot.Child("item_value_type_1").Value.ToString()),
        //                 item_value_type_2 = int.Parse(childSnapshot.Child("item_value_type_2").Value.ToString())
        //             };

        //             inventoryList.Add(item);
        //         }
        //     }

        //     return inventoryList;
        // }

        // public IEnumerator GetInvData(string player_id)
        // {
        //     // Load all gear data from Resources/GearData
        //     GearStat[] gearDataArray = Resources.LoadAll<GearStat>("GearData");

        //     // Create a dictionary for quick access to gear data by gear_stat_id
        //     Dictionary<int, GearStat> gearDataDict = gearDataArray.ToDictionary(gear => gear.gear_stat_id);

        //     // Fetch inventory data for the player
        //     var getInventoryTask = GetInventoryDataAsync(player_id);
        //     yield return new WaitUntil(() => getInventoryTask.IsCompleted);

        //     if (getInventoryTask.Exception == null)
        //     {
        //         List<InventoryItem> inventoryItems = getInventoryTask.Result;
        //         foreach (var item in inventoryItems)
        //         {
        //             Debug.Log($"Item ID: {item.item_id}, Type 1: {item.item_id_type_1}, Type 2: {item.item_id_type_2}, Value Type 1: {item.item_value_type_1}, Value Type 2: {item.item_value_type_2}");

        //             // Match the item_id with the gear_stat_id
        //             if (gearDataDict.TryGetValue(item.item_id, out GearStat matchedGear))
        //             {
        //                 // Print matched gear details
        //                 Debug.Log($"Matched Gear: {matchedGear.gear_name}, Stat ID: {matchedGear.gear_stat_id}, Type ID: {matchedGear.gear_type_id}, Description: {matchedGear.gear_desc}");
        //             }
        //             else
        //             {
        //                 Debug.Log($"No gear found for Item ID: {item.item_id}");
        //             }
        //         }
        //     }
        //     else
        //     {
        //         Debug.LogError("Failed to retrieve inventory data.");
        //     }
        // }

        // public IEnumerator GetRandomInvData(string player_id, Action<List<InventoryItem>> onDataRetrieved)
        // {
        //     // Fetch inventory data for the player
        //     var getInventoryTask = GetInventoryDataAsync(player_id);
        //     yield return new WaitUntil(() => getInventoryTask.IsCompleted);

        //     if (getInventoryTask.Exception == null)
        //     {
        //         List<InventoryItem> inventoryItems = getInventoryTask.Result;
                
        //         // Log details of each inventory item if needed
        //         foreach (var item in inventoryItems)
        //         {
        //             Debug.Log($"Item ID: {item.item_id}, Type 1: {item.item_id_type_1}, Type 2: {item.item_id_type_2}, " +
        //                     $"Value Type 1: {item.item_value_type_1}, Value Type 2: {item.item_value_type_2}");
        //         }

        //         // Return the inventory items list directly through the callback
        //         onDataRetrieved?.Invoke(inventoryItems);
        //     }
        //     else
        //     {
        //         Debug.LogError("Failed to retrieve inventory data.");
        //         onDataRetrieved?.Invoke(null); // Pass null or an empty list if an error occurred
        //     }
        // }


        // public async void AddInventoryData(string player_id, int id, int value_id_1, int value_id_2, int value_1, int value_2)
        // {
        //     var inventoryData = new Dictionary<string, object>
        //     {
        //         { "item_id", 0 },
        //         { "item_level", 0},
        //         { "item_ascend", 0 },
        //         { "item_exp_max", 0},
        //         { "item_current_exp", 0},
        //         { "item_id_type_1", 0 },
        //         { "item_id_type_2", 0 },
        //         { "item_value_type_1", 0 },
        //         { "item_value_type_2", 0 }
        //     };

        //     // Menentukan referensi lokasi di database untuk data baru
        //     var DBTask = DBreference.Child("inventory").Child(player_id).Push();

        //     // Menambahkan data ke database
        //     await DBTask.SetValueAsync(inventoryData);
            
        //     Debug.Log("Inventory data added successfully.");
        // }

        // public async void AddGearRandom(string player_id)
        // {
        //     // Load all objects from Resources/GearData
        //     GearStat[] gearDataArray = Resources.LoadAll<GearStat>("GearData");

        //     if (gearDataArray.Length == 0)
        //     {
        //         Debug.LogWarning("No gear data found in Resources/GearData");
        //         return;
        //     }

        //     // Select a random gear
        //     GearStat randomGear = gearDataArray[UnityEngine.Random.Range(0, gearDataArray.Length)];

        //     Debug.Log($"Selected Gear: {randomGear.gear_name}, Stat ID: {randomGear.gear_stat_id}, Type ID: {randomGear.gear_type_id}, Description: {randomGear.gear_desc}");

        //     // Fetch the existing inventory data to determine the next index
        //     var inventorySnapshot = await DBreference.Child("inventory").Child(player_id).GetValueAsync();

        //     int nextIndex = 1; // Start at 1

        //     if (inventorySnapshot.Exists)
        //     {
        //         // Get the existing keys to find the highest index
        //         foreach (var child in inventorySnapshot.Children)
        //         {
        //             if (int.TryParse(child.Key, out int currentIndex))
        //             {
        //                 // Find the maximum index
        //                 if (currentIndex >= nextIndex)
        //                 {
        //                     nextIndex = currentIndex + 1; // Increment for the next item
        //                 }
        //             }
        //         }
        //     }

        //     // Create a dictionary for the gear data
        //     var gearData = new Dictionary<string, object>
        //     {
        //         { "item_id", randomGear.gear_type_id },
        //         { "item_level", 1 },
        //         { "item_ascend", 0 },
        //         { "item_exp_max", 100 },
        //         { "item_current_exp", 0 },
        //         { "item_id_type_1", 0 },
        //         { "item_id_type_2", 0 },
        //         { "item_value_type_1", 0 },
        //         { "item_value_type_2", 0 }
        //     };

        //     // Define the reference location in the database for new data with the next index
        //     var DBTask = DBreference.Child("inventory").Child(player_id).Child(nextIndex.ToString());

        //     // Add data to the database
        //     await DBTask.SetValueAsync(gearData);

        //     Debug.Log("Random gear added to inventory successfully.");
        // }

        // public IEnumerator UpgradeGear(string player_id, int gear_id, int gear_exp_add) 
        // {
        //     var gearData = DBreference.Child("inventory").Child(player_id).Child(gear_id.ToString()).Child("item_current_exp").GetValueAsync();
        //     yield return new WaitUntil(() => gearData.IsCompleted);

        //     int currentExp = Convert.ToInt32(gearData.Result.Value);
        //     currentExp += gear_exp_add;

        //     // Define the reference path to the specific gear item
        //     var gearRef = DBreference.Child("inventory").Child(player_id).Child(gear_id.ToString()).Child("item_current_exp").SetValueAsync(currentExp);
        //     yield return new WaitUntil(() => gearRef.IsCompleted);
        // }

        // public delegate void GearDataCallback(object gearData);

        // public IEnumerator GetGearData(string player_id, int gear_id, string data_type, GearDataCallback callback)
        // {
        //     var gearDataTask = DBreference.Child("inventory")
        //         .Child(player_id)
        //         .Child(gear_id.ToString())
        //         .Child(data_type)
        //         .GetValueAsync();

        //     // Wait for the task to complete
        //     yield return new WaitUntil(() => gearDataTask.IsCompleted);

        //     if (gearDataTask.IsFaulted)
        //     {
        //         Debug.LogError("Error retrieving gear data: " + gearDataTask.Exception);
        //         callback(null); // Call the callback with null in case of error
        //         yield break; // Exit if there's an error
        //     }

        //     var gearDataSnapshot = gearDataTask.Result;

        //     if (gearDataSnapshot.Exists)
        //     {
        //         var gearData = gearDataSnapshot.Value;
        //         callback(gearData); // Return the gear data via callback
        //     }
        //     else
        //     {
        //         Debug.Log("No gear data found for the specified gear ID.");
        //         callback(null); // Call the callback with null if no data is found
        //     }
        // }



        // public IEnumerator AscendGear(string player_id, int gear_id) 
        // {
        //     var gearData = DBreference.Child("inventory").Child(player_id).Child(gear_id.ToString()).Child("item_ascend").GetValueAsync();
        //     yield return new WaitUntil(() => gearData.IsCompleted);

        //     int currentAscend = Convert.ToInt32(gearData.Result.Value);
        //     currentAscend += 1;

        //     // Define the reference path to the specific gear item
        //     var gearRef = DBreference.Child("inventory").Child(player_id).Child(gear_id.ToString()).Child("item_ascend").SetValueAsync(currentAscend);
        //     yield return new WaitUntil(() => gearRef.IsCompleted);
        // }

        // Documented start here

        public async Task<List<InventoryItem>> GetAllPlayerInventory(string player_id)
        {
            var inventoryList = new List<InventoryItem>();

            try
            {
                // Await the task to get data from Firebase
                var DBTask = await DBreference.Child("inventory").Child(player_id).GetValueAsync();

                // Check if data exists
                if (DBTask.Exists)
                {
                    Debug.Log("Inventory data found for player: " + player_id);

                    // Iterate over the children (inventory items)
                    foreach (var childSnapshot in DBTask.Children)
                    {
                        // Log the child data being read
                        Debug.Log("Found child: " + childSnapshot.Key);

                        InventoryItem item = new InventoryItem
                        {
                            item_id = int.Parse(childSnapshot.Child("item_id").Value.ToString()),
                            item_level = int.Parse(childSnapshot.Child("item_level").Value.ToString()),
                            item_ascend = int.Parse(childSnapshot.Child("item_ascend").Value.ToString()),
                            item_exp_max = int.Parse(childSnapshot.Child("item_exp_max").Value.ToString()),
                            item_current_exp = int.Parse(childSnapshot.Child("item_current_exp").Value.ToString()),
                            item_id_type_1 = int.Parse(childSnapshot.Child("item_id_type_1").Value.ToString()),
                            item_id_type_2 = int.Parse(childSnapshot.Child("item_id_type_2").Value.ToString()),
                            item_value_type_1 = int.Parse(childSnapshot.Child("item_value_type_1").Value.ToString()),
                            item_value_type_2 = int.Parse(childSnapshot.Child("item_value_type_2").Value.ToString())
                        };

                        // Log the values of the item
                        Debug.Log($"Item {item.item_id}: Level {item.item_level}, Ascend {item.item_ascend}");

                        inventoryList.Add(item);
                    }

                    // Log success after data is fetched and mapped
                    Debug.Log("Inventory is fetched successfully!");

                }
                else
                {
                    Debug.LogWarning("No inventory data found for player: " + player_id);
                }
            }
            catch (Exception ex)
            {
                // Handle errors with detailed logs
                Debug.LogError("Error fetching inventory: " + ex.Message);
            }

            return inventoryList;
        }


        public async Task<List<InventoryItem>> GetPlayerGearData(string player_id, int gear_id)
        {
            var playerGearData = new List<InventoryItem>();
            string path = $"inventory/{player_id}/{gear_id}";
            Debug.Log($"Fetching gear data from path: {path}");
            var gearData = await DBreference.Child(path).GetValueAsync();

            if (gearData.Exists)
            {
                // Since gearData is expected to directly contain the item fields, not children.
                InventoryItem item = new InventoryItem
                {
                    item_id = int.Parse(gearData.Child("item_id").Value.ToString()),
                    item_level = int.Parse(gearData.Child("item_level").Value.ToString()),
                    item_ascend = int.Parse(gearData.Child("item_ascend").Value.ToString()),
                    item_exp_max = int.Parse(gearData.Child("item_exp_max").Value.ToString()),
                    item_current_exp = int.Parse(gearData.Child("item_current_exp").Value.ToString()),
                    item_id_type_1 = int.Parse(gearData.Child("item_id_type_1").Value.ToString()),
                    item_id_type_2 = int.Parse(gearData.Child("item_id_type_2").Value.ToString()),
                    item_value_type_1 = int.Parse(gearData.Child("item_value_type_1").Value.ToString()),
                    item_value_type_2 = int.Parse(gearData.Child("item_value_type_2").Value.ToString())
                };

                playerGearData.Add(item);
            }
            else
            {
                Debug.LogWarning("Gear data does not exist.");
            }

            return playerGearData;
        }


        public IEnumerator EditPlayerGearData(string player_id, int gear_id, string selected_parameter, int set_value) {
            var gearData = DBreference.Child("inventory").Child(player_id).Child(gear_id.ToString()).Child(selected_parameter).SetValueAsync(set_value);
            yield return new WaitUntil(() => gearData.IsCompleted);
        }

        public IEnumerator DeletePlayerGear(string player_id, int gear_id) {

            var gearData = DBreference.Child("inventory").Child(player_id).Child(gear_id.ToString()).RemoveValueAsync();
            yield return new WaitUntil(() => gearData.IsCompleted);

        }

        public async void AddGearToPlayer(string player_id, int gear_type_id, int gear_type_1, int gear_type_2, int gear_value_1, int gear_value_2) {
            
            var inventoryData = new Dictionary<string, object>
            {
                { "item_id", gear_type_id },
                { "item_level", 0},
                { "item_ascend", 0 },
                { "item_exp_max", 0},
                { "item_current_exp", 0},
                { "item_id_type_1", gear_type_1 },
                { "item_id_type_2", gear_type_2 },
                { "item_value_type_1", gear_value_1 },
                { "item_value_type_2", gear_value_2 }
            };

            // Menentukan referensi lokasi di database untuk data baru
            var DBTask = DBreference.Child("inventory").Child(player_id).Push();

            // Menambahkan data ke database
            await DBTask.SetValueAsync(inventoryData);
        }

        public async void GiveRandomGear(string player_id) {

            var inventoryData = new Dictionary<string, object>
            {
                { "item_id", UnityEngine.Random.Range(1,3) },
                { "item_level", 0},
                { "item_ascend", 0 },
                { "item_exp_max", 0},
                { "item_current_exp", 0},
                { "item_id_type_1", UnityEngine.Random.Range(1,3) },
                { "item_id_type_2", UnityEngine.Random.Range(1,3) },
                { "item_value_type_1", UnityEngine.Random.Range(20,50) },
                { "item_value_type_2", UnityEngine.Random.Range(20,50) }
            };

            // Menentukan referensi lokasi di database untuk data baru
            var DBTask = DBreference.Child("inventory").Child(player_id).Push();

            // Menambahkan data ke database
            await DBTask.SetValueAsync(inventoryData);
        }

        public string GetPlayerId() {
            return playerData.player_id;
        }

        public string GetPlayerName() {
            return playerData.playerName;
        }

        public async Task<int?> GetBalanceData(string player_id, string type_data) {
            var balanceData = await DBreference.Child("balances").Child(player_id).Child(type_data).GetValueAsync();
            if (balanceData.Exists) {
                int.TryParse(balanceData.Value.ToString(), out int balanceValue);
                return balanceValue;
            } else {
                return 0;
            }
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

        public IEnumerator TopUp(string player_id, string topup_type, int amount)
        {
            Task DBTask = DBreference.Child("balances").Child(player_id).Child(topup_type).SetValueAsync(amount);
            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        }
    }
}
