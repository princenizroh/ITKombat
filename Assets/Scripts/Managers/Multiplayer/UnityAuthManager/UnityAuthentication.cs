using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;


namespace ITKombat
{
    public class UnityAuthentication : MonoBehaviour
    {
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
                Debug.LogError($"Failed to Request anonymously: {ex.Message}");
            }
        }
    }
}
