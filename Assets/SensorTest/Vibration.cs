using UnityEngine;

public static class Vibration
{
    public static void Vibrate(long milliseconds = 500)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrate(milliseconds);
#else
        // For other platforms or in the editor, you can optionally call Handheld.Vibrate()
        Debug.Log("Vibration not supported on this platform or in the editor.");
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private static void AndroidVibrate(long milliseconds)
    {
        try
        {
            // Get the UnityPlayer class
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                // Get the currentActivity
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                // Get the Vibrator service
                AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

                if (vibrator != null)
                {
                    vibrator.Call("vibrate", milliseconds);
                    Debug.Log($"Android vibration triggered for {milliseconds} milliseconds.");
                }
                else
                {
                    Debug.LogWarning("Vibrator service not found.");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Exception during Android vibration: " + e.Message);
        }
    }
#endif
}
