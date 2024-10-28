using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class AuthManager : MonoBehaviour
{
    // UI References
    public GameObject loginPanel;
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI statusText;

    // Firebase Authentication
    private FirebaseAuth auth;
    private FirebaseUser user;

    void Start()
    {
        Debug.Log("[AUTH] AuthManager Start called");

        if (FirebaseManager.Instance == null)
        {
            Debug.LogError("[AUTH_ERROR] FirebaseManager instance is null");
            return;
        }

        Debug.Log("[AUTH] Firebase initialized status: " + FirebaseManager.Instance.isFirebaseInitialized);

        if (FirebaseManager.Instance.isFirebaseInitialized)
        {
            InitializeAuth();
        }
        else
        {
            Debug.Log("[AUTH] Waiting for Firebase initialization, subscribing to event");
            FirebaseManager.Instance.OnFirebaseInitialized += InitializeAuth;
        }
    }
    void InitializeAuth()
    {
        try
        {
            Debug.Log("[AUTH] Initializing Auth");
            auth = FirebaseManager.Instance.auth;
            Debug.Log("[AUTH] Auth instance obtained: " + (auth != null));

            // Check current user
            user = auth.CurrentUser;
            Debug.Log("[AUTH] Current user status: " + (user != null ? "Logged in" : "Not logged in"));

            if (user != null)
            {
                Debug.Log("[AUTH] User is already logged in: " + user.Email);
                string displayName = GetDisplayName(user);
                playerNameText.text = "Welcome, " + displayName;
            }
            else
            {
                playerNameText.text = "Please Login";
            }

            FirebaseManager.Instance.OnFirebaseInitialized -= InitializeAuth;
            Debug.Log("[AUTH] Auth initialization completed");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AUTH_ERROR] InitializeAuth failed: {ex.Message}\n{ex.StackTrace}");
        }
    }

    // Get display name from email or user profile
    private string GetDisplayName(FirebaseUser user)
    {
        if (!string.IsNullOrEmpty(user.DisplayName))
        {
            return user.DisplayName;
        }
        else if (!string.IsNullOrEmpty(user.Email))
        {
            return user.Email.Split('@')[0];
        }
        else
        {
            return "User";
        }
    }

    // Called when Start Game button is clicked
    public void StartGame()
    {
        if (user != null)
        {
            // User is logged in, proceed to game scene
            Debug.Log("Proceeding to game...");

            OnLoginSuccess(user);
            GameManager.Instance.LoadAllFishData();

            // Load your game scene here
            SceneManager.LoadScene("InGameScenes");
        }
        else
        {
            // User is not logged in, show login panel
            loginPanel.SetActive(true);
        }
    }

    // Show login panel
    public void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
    }

    // Hide login panel
    public void HideLoginPanel()
    {
        loginPanel.SetActive(false);
        statusText.text = "";
    }

    // Register a new user
    public void RegisterUser()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {

            if (task.IsCanceled)
            {
                Debug.LogError("Registration was canceled.");
                statusText.text = "Registration canceled.";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("Registration encountered an error: " + task.Exception);
                HandleAuthError(task.Exception, statusText);
                return;
            }

            // Registration successful
            user = task.Result.User;
            Debug.LogFormat("User registered successfully: {0} ({1})", user.Email, user.UserId);

            // Optionally set the user's display name
            UpdateUserProfile(user, email.Split('@')[0]);

            string displayName = GetDisplayName(user);
            playerNameText.text = "Welcome, " + displayName;
            HideLoginPanel();
        });
    }

    // Login an existing user
    public void LoginUser()
    {
        try
        {
            Debug.Log("[AUTH] Starting login process");

            // Check Firebase initialization
            if (!FirebaseManager.Instance.isFirebaseInitialized)
            {
                Debug.LogError("[AUTH_ERROR] Firebase not initialized during login attempt");
                statusText.text = "System initializing, please wait...";
                FirebaseManager.Instance.ReinitializeFirebase();
                return;
            }

            // Check auth instance
            if (auth == null)
            {
                Debug.LogError("[AUTH_ERROR] Auth instance is null, attempting to reinitialize");
                auth = FirebaseManager.Instance.auth;
                if (auth == null)
                {
                    Debug.LogError("[AUTH_ERROR] Failed to get auth instance");
                    statusText.text = "Authentication system error. Please restart the app.";
                    return;
                }
            }

            string email = emailInputField.text;
            string password = passwordInputField.text;

            Debug.Log("[AUTH] Attempting login with email: " + email);

            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Login was canceled.");
                    statusText.text = "Login canceled.";
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("Login encountered an error: " + task.Exception);
                    HandleAuthError(task.Exception, statusText);
                    return;
                }

                // Login successful
                user = task.Result.User;
                Debug.LogFormat("User logged in successfully: {0} ({1})", user.Email, user.UserId);

                string displayName = GetDisplayName(user);
                playerNameText.text = "Welcome, " + displayName;
                HideLoginPanel();
            });
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AUTH_ERROR] Login process failed: {ex.Message}\n{ex.StackTrace}");
            statusText.text = "Login failed. Please try again.";
        }
    }

    // Call this after successful login
    void OnLoginSuccess(FirebaseUser firebaseUser)
    {
        user = firebaseUser;
        string userId = user.UserId;

        // Load user data
        DatabaseManager.Instance.LoadUserData(userId, (userData) =>
        {
            if (userData == null)
            {
                UserData newUserData = new UserData();
                newUserData.username = GetDisplayName(user);
                newUserData.totalScore = 0;
                newUserData.totalAssets = 0;

                DatabaseManager.Instance.SaveUserData(userId, newUserData);

                GameManager.Instance.currentUserData = newUserData;
            }
            else
            {
                GameManager.Instance.currentUserData = userData;
            }
        });
    }

    // Update user profile with display name
    private void UpdateUserProfile(FirebaseUser user, string displayName)
    {
        UserProfile profile = new UserProfile { DisplayName = displayName };

        user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                return;
            }

            Debug.Log("User profile updated successfully.");
        });
    }

    // Logout the current user
    public void Logout()
    {
        auth.SignOut();
        user = null;
        playerNameText.text = "Please Login";
        Debug.Log("User logged out.");
        // Return to authentication scene
        SceneManager.LoadScene("Loading Scenes");
    }

    // Method to handle authentication errors
    private void HandleAuthError(System.AggregateException exception, TextMeshProUGUI statusText)
    {
        FirebaseException firebaseEx = exception.Flatten().InnerExceptions[0] as FirebaseException;
        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

        string message = "Authentication error.";
        switch (errorCode)
        {
            case AuthError.InvalidEmail:
                message = "Invalid email address.";
                break;
            case AuthError.WrongPassword:
                message = "Incorrect password.";
                break;
            case AuthError.UserNotFound:
                message = "User not found.";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "Email already in use.";
                break;
            case AuthError.WeakPassword:
                message = "Weak password.";
                break;
            default:
                message = "Error: " + errorCode.ToString();
                break;
        }
        statusText.text = message;
    }
}
