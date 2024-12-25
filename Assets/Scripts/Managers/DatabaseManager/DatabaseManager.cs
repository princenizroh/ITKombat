using UnityEngine;
using System.Collections;
using Firebase.Database;
using Firebase;
using TMPro;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace ITKombat
{
    [Serializable]
    public class PlayerDatas
    {
        public string username;
        public string level;
        public string ktm;
        public string danus; 
        public string email;

        public PlayerDatas(string _username, string _level, string _ktm, string _danus, string _email)
        {
            this.username = _username;
            this.level = _level;
            this.ktm = _ktm;
            this.danus = _danus;
            this.email = _email;
        }
    }
    public class DatabaseManager : MonoBehaviour
    {
        public static DatabaseManager instance;

        public DatabaseReference DBreference;

        [field: SerializeField] public PlayerData PlayerData{ get; private set; }
        [field: SerializeField] public PlayerDatas PlayerDatas { get; private set; }
        [field: SerializeField] public ProfileData ProfileData { get; private set; }
        [field: SerializeField] public BalanceData BalanceData { get; private set; }

        [Header("User Data UI")]
        public TMP_Text usernameText;
        public TMP_Text levelText;
        public TMP_Text ktmText;
        public TMP_Text danusText;

        [Header("Player Data UI")]
        public TMP_Text playerUsernameText;
        public TMP_Text playerPasswordText;
        public TMP_Text playerEmailText;

        [Header("Profile Data UI")]
        public TMP_Text playerExpText;
        public TMP_Text playerLevelText;
        public TMP_Text playerRankText;
        public TMP_Text sk2pmText;

        [Header("Balance Data UI")]
        public TMP_Text ktmBalanceText;
        public TMP_Text danusBalanceText;
        

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

        public void UpdateSaveUIFromInspector()
        {
            if (PlayerDatas != null)
            {
                usernameText.text = PlayerDatas.username;
                // levelText.text = PlayerDatas.level;
                ktmText.text = PlayerDatas.ktm;
                danusText.text = PlayerDatas.danus;
                playerEmailText.text = PlayerDatas.email;
            }

            if (ProfileData != null)
            {
                playerExpText.text = ProfileData.playerExp.ToString();
                playerLevelText.text = ProfileData.playerLevel.ToString();
                playerRankText.text = ProfileData.playerRank;
                sk2pmText.text = ProfileData.sk2pm.ToString();
            }

            if (BalanceData != null)
            {
                ktmBalanceText.text = BalanceData.KTM.ToString();
                danusBalanceText.text = BalanceData.danus.ToString();
            }
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            // UpdateSaveUIFromInspector();
        }
#endif


        public void initialize()
        {

        }

        private void InitializeFirebase()
        {
            DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        }

        public void SaveDataButton()
        {
            PlayerDatas = new PlayerDatas(usernameText.text, levelText.text, ktmText.text, danusText.text, playerEmailText.text);
            // PlayerData = new PlayerData(playerUsernameText.text, playerPasswordText.text, playerEmailText.text);
            ProfileData = new ProfileData(int.Parse(playerExpText.text), int.Parse(playerLevelText.text), playerRankText.text, int.Parse(sk2pmText.text));
            BalanceData = new BalanceData(int.Parse(ktmBalanceText.text), int.Parse(danusBalanceText.text));
            StartCoroutine(UpdateUserData(usernameText.text, levelText.text, ktmText.text, danusText.text, playerEmailText.text));
            // StartCoroutine(CreatePlayerData(playerUsernameText.text, playerPasswordText.text, playerEmailText.text));
            StartCoroutine(CreateProfileData(AuthManager.instance.User.UserId, int.Parse(playerExpText.text), int.Parse(playerLevelText.text), playerRankText.text, int.Parse(sk2pmText.text)));
            StartCoroutine(CreateBalanceData(AuthManager.instance.User.UserId, int.Parse(ktmBalanceText.text), int.Parse(danusBalanceText.text)));
            
        }

#region Send Data
        
        private IEnumerator UpdateUserData(string username, string level, string ktm, string danus, string email)
        {
            // Get the user id
            string userId = AuthManager.instance.User.UserId;
            PlayerDatas playerDatas = new PlayerDatas(username, level, ktm, danus, email);

            string json = JsonUtility.ToJson(playerDatas);
            Task DBTask = DBreference.Child("players").Child(userId).SetRawJsonValueAsync(json);
            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                Debug.Log("User data updated successfully");
            }
        }
        // Membuat data player saat registrasi
        // public IEnumerator CreatePlayerData(string username, string password, string email)
        // {
        //     string playerId = DBreference.Push().Key;
        //
        //     PlayerData playerData = new PlayerData(username, password, email);
        //
        //     string json = JsonUtility.ToJson(playerData);
        //
        //     Task DBTask = DBreference.Child("players").Child(playerId).SetRawJsonValueAsync(json);
        //
        //     yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        //
        //     if (DBTask.Exception != null)
        //     {
        //         Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        //     }
        //     else
        //     {
        //         Debug.Log("Player data created successfully");
        //     }
        // }

        // Membuat data profile saat registrasi
        public IEnumerator CreateProfileData(string playerId, int profileExp, int playerLevel, string playerRank, int sk2pm)
        {

            ProfileData profileData = new ProfileData(profileExp, playerLevel, playerRank, sk2pm);

            string json = JsonUtility.ToJson(profileData);

            Task DBTask = DBreference.Child("profiles").Child(playerId).SetRawJsonValueAsync(json);

            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                Debug.Log("Profile data created successfully");
            }
        }

        public IEnumerator CreateBalanceData(string playerId, int _KTM, int _DANUS)
        {
            BalanceData balanceData = new BalanceData(_KTM, _DANUS);

            string json = JsonUtility.ToJson(balanceData);
            Task DBTask = DBreference.Child("balance").Child(playerId).SetRawJsonValueAsync(json);
            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            { 
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                Debug.Log("Balance data created successfully");
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
        
        public IEnumerator LoadUserData()
        {
            // Get the data from the Database
            Task<DataSnapshot> DBTask = DBreference.Child("players").Child(AuthManager.instance.User.UserId).GetValueAsync();

            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else if (DBTask.Result.Value == null)
            {
                // No data exists
                usernameText.text = AuthManager.instance.User.DisplayName;
                levelText.text = "1";
                ktmText.text = "0";
                danusText.text = "0";
                playerEmailText.text = AuthManager.instance.User.Email;
            }
            else
            {
                // Data has been retrieved
                DataSnapshot snapshot = DBTask.Result;
                usernameText.text = snapshot.Child("username").Value.ToString();
                levelText.text = snapshot.Child("level").Value.ToString();
                ktmText.text = snapshot.Child("ktm").Value.ToString();
                danusText.text = snapshot.Child("danus").Value.ToString();
                playerEmailText.text = snapshot.Child("email").Value.ToString();

                Debug.Log("Snapshot" + snapshot.GetRawJsonValue());
            }
        }

        public IEnumerator LoadProfileData()
        {
            // Get the data from the Database
            Task<DataSnapshot> DBTask = DBreference.Child("profiles").Child(AuthManager.instance.User.UserId).GetValueAsync();

            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else if (DBTask.Result.Value == null)
            {
                // No data exists
                playerExpText.text = "0";
                playerLevelText.text = "1";
                playerRankText.text = "Noob";
                sk2pmText.text = "0";
            }
            else
            {
                // Data has been retrieved
                DataSnapshot snapshot = DBTask.Result;
                playerExpText.text = snapshot.Child("player_exp").Value.ToString();
                playerLevelText.text = snapshot.Child("player_level").Value.ToString();
                playerRankText.text = snapshot.Child("player_rank_sk2pm").Value.ToString();
                sk2pmText.text = snapshot.Child("sk2pm").Value.ToString();
                Debug.Log("Snapshot" + snapshot.GetRawJsonValue());
            }
        }
#endregion
    }
}
