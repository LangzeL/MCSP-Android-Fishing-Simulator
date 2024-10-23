using System;
using System.Collections;
using System.Collections.Generic;
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

    void Awake()
    {
        // Ensure there is only one instance of FirebaseManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("FirebaseManager: Starting Firebase initialization.");

            // Initialize Firebase
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");
                }
            });
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseApp app = FirebaseApp.DefaultInstance;
        Debug.Log("FirebaseManager: Firebase initialized successfully.");

        isFirebaseInitialized = true;
        OnFirebaseInitialized?.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
