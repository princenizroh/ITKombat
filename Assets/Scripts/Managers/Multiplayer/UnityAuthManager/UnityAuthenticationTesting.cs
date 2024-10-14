using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using System.Linq;

namespace ITKombat
{
    public class UnityAuthenticationTesting : MonoBehaviour
    {
        public static UnityAuthenticationTesting Instance;
        [Header("Firebase")]
        public FirebaseUser User;
        public FirebaseAuth auth;
        public DependencyStatus dependencyStatus;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // Pindahkan inisialisasi Firebase ke coroutine
            StartCoroutine(InitializeFirebaseCoroutine());
        }

        private IEnumerator InitializeFirebaseCoroutine()
        {
            // Mengecek dan memperbaiki dependencies Firebase
            var checkDependenciesTask = FirebaseApp.CheckAndFixDependenciesAsync();
            yield return new WaitUntil(() => checkDependenciesTask.IsCompleted);

            dependencyStatus = checkDependenciesTask.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Firebase dependencies available, initializing Firebase...");
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        }

        private void InitializeFirebase()
        {
            Debug.Log("Setting up Firebase Auth...");
            auth = FirebaseAuth.DefaultInstance; // Menginisialisasi FirebaseAuth

            if (auth != null)
            {
                Debug.Log("Firebase Auth initialized successfully.");
                // Panggil login setelah Firebase Auth berhasil diinisialisasi
                StartCoroutine(Login("akbar@gmail.com", "admin123"));
            }
            else
            {
                Debug.LogError("Firebase Auth initialization failed.");
            }
        }

        public IEnumerator Login(string _email, string _password)
        {
            Debug.Log("Starting Firebase login process...");

            // Memulai task login
            Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
            Debug.Log("LoginTask created, waiting for completion...");
            Debug.Log("Attempting to login with email: " + _email);

            // Tunggu sampai task login selesai
            yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

            if (LoginTask.IsCanceled)
            {
                Debug.LogError("LoginTask was canceled.");
                yield break;
            }
            if (LoginTask.IsFaulted)
            {
                Debug.LogError("LoginTask encountered an error.");
                if (LoginTask.Exception != null)
                {
                    FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
                    if (firebaseEx != null)
                    {
                        Debug.LogError($"Firebase Exception: {firebaseEx.Message}, ErrorCode: {(AuthError)firebaseEx.ErrorCode}");
                    }
                    else
                    {
                        Debug.LogError($"Non-Firebase Exception: {LoginTask.Exception.Message}");
                    }
                }
                yield break;
            }
            // Jika tidak ada error, lanjutkan dengan sukses login
            User = LoginTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName ?? "No DisplayName", User.Email);

            Debug.Log("Login successful!");
            Debug.Log($"User ID: {User.UserId}, Email: {User.Email}");
        }

        private async void Start()
        {
            await UnityServices.InitializeAsync();
            Debug.Log("Unity Services Initialized");

            SetupEvents();
            await SignInAnonymouslyAsync();
        }

        private void SetupEvents()
        {
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log($"Signed in as {AuthenticationService.Instance.PlayerId}");
                Debug.Log($"Access token: {AuthenticationService.Instance.AccessToken}");
            };
        }

        private async Task SignInAnonymouslyAsync()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Signed in anonymously");
                Debug.Log($"Player ID: {AuthenticationService.Instance.PlayerId}");
            }
            catch (AuthenticationException ex)
            {
                Debug.LogError($"Failed to sign in anonymously: {ex.Message}");
            }
            catch (RequestFailedException ex)
            {
                Debug.LogError($"Failed to request anonymously: {ex.Message}");
            }
        }
    }
}
