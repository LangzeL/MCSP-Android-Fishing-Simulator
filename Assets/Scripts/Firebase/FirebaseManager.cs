using System;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;
    public FirebaseAuth auth;
    public DatabaseReference databaseRef;
    public bool isFirebaseInitialized = false;
    public event Action OnFirebaseInitialized;
    private bool isInitializing = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[FIREBASE_INIT] Starting Firebase initialization on platform: " + Application.platform);
            StartFirebaseInit();
        }
        else
        {
            Debug.Log("[FIREBASE_INIT] Destroying duplicate FirebaseManager instance");
            Destroy(gameObject);
        }
    }

    private void StartFirebaseInit()
    {
        if (isInitializing)
        {
            Debug.Log("[FIREBASE_INIT] Initialization already in progress");
            return;
        }

        isInitializing = true;
        Debug.Log("[FIREBASE_INIT] Starting initialization with explicit config");

        try
        {
            // Create Firebase app with explicit options
            FirebaseApp.Create(new AppOptions()
            {
                ApiKey = "AIzaSyDMy8dZd8vlgOlvG2ZfYPqbiz89od0UvWA",
                ProjectId = "mcsp-android-fishing-simulator",
                DatabaseUrl = new Uri("https://mcsp-android-fishing-simulator-default-rtdb.firebaseio.com"),
                StorageBucket = "mcsp-android-fishing-simulator.appspot.com",
                AppId = "1:1075733345226:android:17b13d9484cf423e859533"
            });

            // Continue with initialization
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                Debug.Log("[FIREBASE_INIT] Dependencies check completed with status: " + task.Result);
                if (task.Result == DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError($"[FIREBASE_ERROR] Could not resolve dependencies: {task.Result}");
                    isInitializing = false;
                }
            });
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FIREBASE_ERROR] Failed to create Firebase app: {ex.Message}\n{ex.StackTrace}");
            isInitializing = false;
        }
    }

    void InitializeFirebase()
    {
        try
        {
            Debug.Log("[FIREBASE_INIT] Starting Firebase service initialization");

            // Get Firebase app instance
            FirebaseApp app = FirebaseApp.DefaultInstance;
            if (app == null)
            {
                Debug.LogError("[FIREBASE_ERROR] DefaultInstance is null");
                return;
            }
            Debug.Log("[FIREBASE_INIT] Firebase App instance initialized");

            // Initialize Auth
            auth = FirebaseAuth.DefaultInstance;
            Debug.Log("[FIREBASE_INIT] Auth instance created: " + (auth != null));

            // Initialize Database
            databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("[FIREBASE_INIT] Database reference created: " + (databaseRef != null));

            isFirebaseInitialized = true;
            isInitializing = false;
            Debug.Log("[FIREBASE_INIT] Firebase initialization completed successfully");

            OnFirebaseInitialized?.Invoke();
            Debug.Log("[FIREBASE_INIT] Firebase initialization event invoked");
        }
        catch (Exception ex)
        {
            isInitializing = false;
            Debug.LogError($"[FIREBASE_ERROR] Initialization failed: {ex.Message}\n{ex.StackTrace}");
        }
    }

    // Add this method
    public void ReinitializeFirebase()
    {
        Debug.Log("[FIREBASE_INIT] Attempting to reinitialize Firebase");

        // Clean up existing instance if any
        if (FirebaseApp.DefaultInstance != null)
        {
            FirebaseApp.DefaultInstance.Dispose();
            Debug.Log("[FIREBASE_INIT] Disposed existing Firebase instance");
        }

        isFirebaseInitialized = false;
        isInitializing = false;
        auth = null;
        databaseRef = null;

        // Start fresh initialization
        StartFirebaseInit();
    }

    void OnDestroy()
    {
        if (FirebaseApp.DefaultInstance != null)
        {
            FirebaseApp.DefaultInstance.Dispose();
            Debug.Log("[FIREBASE_INIT] Firebase App disposed");
        }
    }
}