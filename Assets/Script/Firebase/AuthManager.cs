using UnityEngine;
using UnityEngine.UI;
using TMPro; // If you're using TextMeshPro
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

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
        // Initialize Firebase Auth
        auth = FirebaseAuth.DefaultInstance;

        // Check if the user is already logged in
        user = auth.CurrentUser;
        statusText.text = "";
        if (user != null)
        {
            // User is signed in
            Debug.Log("User is already logged in: " + auth.CurrentUser.Email);
            string displayName = GetDisplayName(user);
            playerNameText.text = "Welcome, " + displayName;
        }
        else
        {
            // User is not signed in
            playerNameText.text = "Please Login";
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
            // Load your game scene here
            SceneManager.LoadScene("Game Scenes");
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
        string email = emailInputField.text;
        string password = passwordInputField.text;

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
