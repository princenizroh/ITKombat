using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    // UI Elements
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_Text userIdText;

    // Player Profile
    private PlayerProfile playerProfile;

    public event Action<PlayerProfile> OnSignedIn;
    public event Action<PlayerProfile> OnAvatarUpdate;

    private PlayerInfo playerInfo;

    private async void Awake()
    {
        // Initialize Unity Services
        await UnityServices.InitializeAsync();

        // Subscribe to the SignedIn event
        PlayerAccountService.Instance.SignedIn += SignedIn;

        // Add listener to login button
        loginButton.onClick.AddListener(LoginButtonPressed);
    }

    private void OnDestroy()
    {
        // Unsubscribe from the SignedIn event
        PlayerAccountService.Instance.SignedIn -= SignedIn;

        // Remove listener from login button
        loginButton.onClick.RemoveListener(LoginButtonPressed);
    }

    // Method triggered by login button press
    private async void LoginButtonPressed()
    {
        await InitSignIn();
    }

    // Sign-in process initialization
    public async Task InitSignIn()
    {
        // Check if the user is already signed in
        if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("Player is already signed in. Logging out and re-signing in.");

            // Log out the current session
            AuthenticationService.Instance.SignOut();
        }

        // Start the sign-in process
        await PlayerAccountService.Instance.StartSignInAsync();
    }

    // Method called when the player is signed in
    private async void SignedIn()
    {
        try
        {
            var accessToken = PlayerAccountService.Instance.AccessToken;
            await SignInWithUnityAsync(accessToken);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    // Sign in using Unity's authentication service
    private async Task SignInWithUnityAsync(string accessToken)
    {
        // Check if already signed in (just a double check after the SignOut logic)
        if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("Player is already signed in.");
            return;
        }

        try
        {
            // Sign in using the provided access token
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Debug.Log("Sign-in is successful.");

            // Fetch player info
            playerInfo = AuthenticationService.Instance.PlayerInfo;
            var name = await AuthenticationService.Instance.GetPlayerNameAsync();

            // Populate player profile
            playerProfile.playerInfo = playerInfo;
            playerProfile.Name = name;

            // Invoke the OnSignedIn event
            OnSignedIn?.Invoke(playerProfile);

            // Update UI elements with player information
            DatabaseSync(playerProfile);
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    private void DatabaseSync(PlayerProfile player_profile) {
        // jika id belum ada di database maka profile akan dibuat, jika id sudah ada didalam database maka profile akan dicocokan.
        // jika match dengan database maka akan di push ke ui

        UpdateUI(player_profile);
    }

    // Update UI with player profile details
    private void UpdateUI(PlayerProfile profile)
    {
        userIdText.text = $"id_{profile.playerInfo.Id}";
    }

    // This can be used if there is an avatar update
    private void LoginController_OnAvatarUpdate(PlayerProfile profile)
    {
        playerProfile = profile;
        OnAvatarUpdate?.Invoke(profile);
    }
}

[Serializable]
public struct PlayerProfile
{
    public PlayerInfo playerInfo;
    public string Name;
}