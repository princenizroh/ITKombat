using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Google;
using UnityEngine;
using UnityEngine.UI;

namespace ITKombat
{
    public class GoogleAuthLogin : MonoBehaviour
    {
        public string GoogleAPI = "YOUR_API_KEY";

        private GoogleSignInConfiguration configuration;

        Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
        Firebase.Auth.FirebaseAuth auth;
        Firebase.Auth.FirebaseUser user;

        public Text Username, UserEmail;

        public GameObject LoginScreen, ProfileScreen;

        private void Awake() {
            configuration = new GoogleSignInConfiguration{
                WebClientId = GoogleAPI,
                RequestIdToken = true,
            };
        }

        private void Start() {
            InitFirebase();
        }

        void InitFirebase() {
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        }

        public void GoogleSignInClick() {
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;
            GoogleSignIn.Configuration.RequestEmail = true;

            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticatedFinished);
        }

        void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task) {
            if (task.IsFaulted) {
                Debug.LogError("Faulted");
            } else if (task.IsCanceled) {
                Debug.LogError("Cancelled");
            } else {
                Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

                auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(Authtask => {
                    if (Authtask.IsCanceled) {
                        return;
                    }

                    if (Authtask.IsFaulted) {
                        Debug.LogError("SignInWithCredentialAsync encountered an error: " + Authtask.Exception);
                        return;
                    }

                    user = auth.CurrentUser;

                    Username.text = user.DisplayName;
                    UserEmail.text = user.Email;

                    LoginScreen.SetActive(false);
                    ProfileScreen.SetActive(true);

                    // StartCoroutine(LoadImage(CheckImageUrl(user.PhotoUrl.ToString())));
                });
            }
        }

        // private string CheckImageUrl(string url) {
        //     if (!string.IsNullOrEmpty(url)) {
        //         return url;
        //     }
        //     return imageUrl;
        // }

        // IEnumerator LoadImage(string imageUri) {
        //     WWW www = new WWW(imageUri);
        //     yield return www;

        //     UserProfilePic.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
        // }
    }
}