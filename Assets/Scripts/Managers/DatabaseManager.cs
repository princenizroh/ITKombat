using UnityEngine;
using System.Collections;
using Firebase.Database;
using Firebase;
using TMPro;
using System.Threading.Tasks;
using System;

namespace ITKombat
{
    public class DatabaseManager : MonoBehaviour
    {
        public static DatabaseManager instance;

        public DatabaseReference DBreference;

        [field: SerializeField] public PlayerData PlayerData{ get; private set; }

        [Header("User Data UI")]
        public TMP_Text usernameText;
        public TMP_Text levelText;
        public TMP_Text ktmText;
        public TMP_Text danusText;
        public GameObject scoreElement;
        public Transform scoreboardContent;

        void Awake()
        {
            // Implement singleton pattern
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            InitializeFirebase();
        }

        public void initialize()
        {

        }

        private void InitializeFirebase()
        {
            DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        }

#region Send Data
        // Membuat data player saat registrasi
        public IEnumerator CreatePlayerData(string playerId, string _username, string _email)
        {
            PlayerData playerData = new PlayerData(playerId, _username, _email);

            playerId = AuthManager.instance.User.UserId;

            string json = JsonUtility.ToJson(playerData);
            Task DBTask = DBreference.Child("players").Child(playerId).SetRawJsonValueAsync(json);
            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            { 
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
        }

        // public IEnumerator CreateProfileData(string _profileId, string _playerId)
        // {
        //     ProfileData profileData = new ProfileData
        //     {
        //         profile_id = _profileId,
        //         username = PlayerData.username,
        //         player_exp = 0,
        //         player_level = 1,
        //         player_rank_sk2pm = 0,
        //         sk2pm = 0,
        //         player_id = _playerId
        //     };
        //
        //     string json = JsonUtility.ToJson(profileData);
        //     Task DBTask = DBreference.Child("profiles").Child().SetRawJsonValueAsync(json);
        //     yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        //
        //     if (DBTask.Exception != null)
        //     { 
        //         Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        //     }
        // }

        // public IEnumerator CreateLoginHistoryData(string _loginHistoryId,string _playerId)
        // {
        //     LoginHistoryData loginHistoryData = new LoginHistoryData
        //     {
        //         login_history_id = _loginHistoryId,
        //         login_date = DateTime.UtcNow.ToString("o"),
        //         logout_date = "",
        //         player_id = _playerId
        //     };
        //
        //     string json = JsonUtility.ToJson(loginHistoryData);
        //     Task DBTask = DBreference.Child("login_history").Child(AuthManager.User.UserId).SetRawJsonValueAsync(json);
        //     yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        //
        //     if (DBTask.Exception != null)
        //     { 
        //         Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        //     }
        // }

        public IEnumerator CreateBalanceData(string _playerId, string _balanceId, int _KTM, int _DANUS)
        {
            BalanceData balanceData = new BalanceData(_balanceId, _KTM, _DANUS, _playerId);

            string json = JsonUtility.ToJson(balanceData);
            Task DBTask = DBreference.Child("balance").Child(AuthManager.instance.User.UserId).SetRawJsonValueAsync(json);
            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            { 
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
        }
#endregion

#region Get Data
        public void GetPlayerData()
        {

        }

        public void GetProfileData()
        {

        }

        public void GetLoginHistoryData()
        {

        }

        public void GetBalanceData()
        {

        }
#endregion
    }
}
