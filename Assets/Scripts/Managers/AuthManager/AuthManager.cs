using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;
using System.Threading.Tasks;

namespace ITKombat 
{
    public class AuthManager : MonoBehaviour
    {
        public static AuthManager instance;
        //Firebase variables
        [Header("Firebase")]
        public FirebaseUser User;
        public FirebaseAuth auth;
        public DependencyStatus dependencyStatus;
        public DatabaseReference DBreference;

        //Login variables
        [Header("Login")]
        public TMP_InputField emailLoginField;
        public TMP_InputField passwordLoginField;
        public TMP_Text warningLoginText;
        public TMP_Text confirmLoginText;

        //Register variables
        [Header("Register")]
        public TMP_InputField usernameRegisterField;
        public TMP_InputField emailRegisterField;
        public TMP_InputField passwordRegisterField;
        public TMP_InputField passwordRegisterVerifyField;
        public TMP_Text warningRegisterText;

        [Header("UserData")]
        public GameObject scoreElement;
        public Transform scoreboardContent;
        

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

        private void InitializeFirebase()
        {
            Debug.Log("Setting up Firebase Auth");
            //Set the authentication instance object
            auth = FirebaseAuth.DefaultInstance;
            DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        }

        public void ClearLoginFields()
        {
            emailLoginField.text = "";
            passwordLoginField.text = "";
        }

        public void ClearRegisterFields()
        {
            emailRegisterField.text = "";
            passwordRegisterField.text = "";
            passwordRegisterVerifyField.text = "";
            usernameRegisterField.text = "";
        }

        //Function for the login button
        public void LoginButton()
        {
            if (string.IsNullOrEmpty(emailLoginField.text))
            {
                warningLoginText.text = "Email cannot be empty!";
                Debug.Log("email: " + emailRegisterField.text);
                return;
            }

            if (string.IsNullOrEmpty(passwordLoginField.text))
            {
                warningLoginText.text = "Password cannot be empty!";
                Debug.Log("password: " + passwordRegisterField.text);
                return;
            }


            //Call the login coroutine passing the email and password
            StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
        }

        //Function for the register button
        public void RegisterButton()
        {
            //Call the register coroutine passing the email, password, and username
            StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
        }

        public void SignOutButton()
        {
            auth.SignOut();
            LoginPageUIManager.instance.LoginScreen();
            ClearLoginFields();
            ClearRegisterFields();
        }


        public void ScoreboardButton()
        {
            StartCoroutine(LoadScoreboardData());
        }

        private IEnumerator Login(string _email, string _password)
        {
            //Call the Firebase auth signin function passing the email and password
            Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
            Debug.Log("LoginTask: " + LoginTask);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

            if (LoginTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
                FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Login Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WrongPassword:
                        message = "Wrong Password";
                        break;
                    case AuthError.InvalidEmail:
                        message = "Invalid Email";
                        break;
                    case AuthError.UserNotFound:
                        message = "Account does not exist";
                        break;
                }
                warningLoginText.text = message;
            }
            else
            {
                //User is now logged in
                //Now get the result
                User = LoginTask.Result.User;
                Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
                warningLoginText.text = "";
                confirmLoginText.text = "Logged In";
                // StartCoroutine(LoadUserData());
                StartCoroutine(DatabaseManager.instance.LoadUserData());
                StartCoroutine(DatabaseManager.instance.LoadProfileData());

                yield return new WaitForSeconds(2);

                DatabaseManager.instance.usernameText.text = User.DisplayName;
                DatabaseManager.instance.playerEmailText.text = User.Email;
                // LoginPageUIManager.instance.UserDataScreen();
                confirmLoginText.text = "";
                ClearLoginFields();
                ClearRegisterFields();
                
            }
        }

        private IEnumerator Register(string _email, string _password, string _username)
        {
            if (_username == "")
            {
                //If the username field is blank show a warning
                warningRegisterText.text = "Missing Username";
            }
            else if(passwordRegisterField.text != passwordRegisterVerifyField.text)
            {
                //If the password does not match show a warning
                warningRegisterText.text = "Password Does Not Match!";
            }
            else 
            {
                //Call the Firebase auth signin function passing the email and password
                Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
                //Wait until the task completes
                yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

                if (RegisterTask.Exception != null)
                {
                    //If there are errors handle them
                    Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                    FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                    string message = "Register Failed!";
                    switch (errorCode)
                    {
                        case AuthError.MissingEmail:
                            message = "Missing Email";
                            break;
                        case AuthError.MissingPassword:
                            message = "Missing Password";
                            break;
                        case AuthError.WeakPassword:
                            message = "Weak Password";
                            break;
                        case AuthError.EmailAlreadyInUse:
                            message = "Email Already In Use";
                            break;
                    }
                    warningRegisterText.text = message;
                }
                else
                {
                    //User has now been created
                    //Now get the result
                    User = RegisterTask.Result.User;

                    if (User != null)
                    {
                        //Create a user profile and set the username
                        UserProfile profile = new UserProfile{DisplayName = _username};

                        //Call the Firebase auth update user profile function passing the profile with the username
                        Task ProfileTask = User.UpdateUserProfileAsync(profile);
                        //Wait until the task completes
                        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                        if (ProfileTask.Exception != null)
                        {
                            //If there are errors handle them
                            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                            FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                            warningRegisterText.text = "Username Set Failed!";
                        }
                        else
                        {
                            //Username is now set
                            //Now return to login screen
                            LoginPageUIManager.instance.LoginScreen();
                            warningRegisterText.text = "";
                        }
                    }
                }
            }

        }

        private IEnumerator UpdateUsernameAuth(string _username)
        {
            // Create a user profile and set the username
            UserProfile profile = new UserProfile{DisplayName = _username};

            // Call the Firebase auth update user profile function passing the profile with the username
            Task ProfileTask = User.UpdateUserProfileAsync(profile);
            yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

            if (ProfileTask.Exception != null)
            {
                // If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                // FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                // AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                // warningRegisterText.text = "Username Set Failed!";
            }
            else
            {
                // Username is now set
                // Now return to login screen
                // LoginPageUIManager.instance.LoginScreen();
                // warningRegisterText.text = "";
            }
        }

        private IEnumerator UpdateUsernameDatabase(string _username)
        {
            Task DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            { 
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                // Username is now set
                // Now return to login screen
                // LoginPageUIManager.instance.LoginScreen();
                // warningRegisterText.text = "";
            }
        }

        
        private IEnumerator LoadScoreboardData()
        {
            Task<DataSnapshot> DBTask = DBreference.Child("users").OrderByChild("level").GetValueAsync();
            
            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                //Data has been retrieved
                DataSnapshot snapshot = DBTask.Result;

                //Destroy any existing scoreboard elements
                foreach (Transform child in scoreboardContent.transform)
                {
                    Destroy(child.gameObject);
                }

                //Loop through every users UID
                foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
                {
                    //Get the username and highscore for each user
                    string _username = childSnapshot.Child("username").Value.ToString();
                    string _level = childSnapshot.Child("level").Value.ToString();
                    string _ktm = childSnapshot.Child("ktm").Value.ToString();
                    string _danus = childSnapshot.Child("danus").Value.ToString();

                    //Create a Scoreboard Entry
                    GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
                    scoreboardElement.GetComponent<ScoreElementManager>().SetScoreElement(_username, int.Parse(_level), int.Parse(_ktm), int.Parse(_danus));
                }
            }
        }
    }
}
