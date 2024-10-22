using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    DatabaseReference reference;


    // Start is called before the first frame update
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SaveData(string userId, int score)
    {
        reference.Child("users").Child(userId).Child("score").SetValueAsync(score);
    }
    public void LoadData(string userId)
    {
        reference.Child("users").Child(userId).Child("score").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int score = Convert.ToInt32(snapshot.Value);
                Debug.Log("User score: " + score);
            }
        });
    }

}
