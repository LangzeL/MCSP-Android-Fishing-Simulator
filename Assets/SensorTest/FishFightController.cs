using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class FishFightController : MonoBehaviour
{
    public AudioSource failSource;
    public AudioSource hookedSource;
    public AudioSource PullRodSource;
    private static FishFightController instance;
    public static FishFightController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FishFightController>();
                if (instance == null)
                {
                    GameObject go = new GameObject("FishFightController");
                    instance = go.AddComponent<FishFightController>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUI(); // Add this line
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

    }

    void InitializeUI()
    {
        Debug.Log("Initializing UI elements");

        // Initialize UI references
        fishingPanel = GameObject.Find("FishingPanel");
        statusText = GameObject.Find("StatusText")?.GetComponent<Text>();
        progressBar = GameObject.Find("Slider")?.GetComponent<Slider>();
        progressText = GameObject.Find("ProgressText")?.GetComponent<Text>();

        // Hide all UI elements immediately
        HideAllUI();
    }

    private void HideAllUI()
    {
        Debug.Log("Hiding all UI elements");

        // Hide Fishing Panel
        if (fishingPanel != null)
        {
            fishingPanel.SetActive(false);
        }

        // Hide Status Text
        if (statusText != null && statusText.gameObject != null)
        {
            statusText.gameObject.SetActive(false);
            statusText.text = "";
        }

        // Hide Progress Bar
        if (progressBar != null && progressBar.gameObject != null)
        {
            progressBar.gameObject.SetActive(false);
            progressBar.value = 0f;
        }

        // Hide Progress Text
        if (progressText != null && progressText.gameObject != null)
        {
            progressText.gameObject.SetActive(false);
            progressText.text = "Progress: 0%";
        }
    }
    [Header("Hook Detection Settings")]
    public float hookDistance = 2.0f;
    public float hookTime = 5f;

    [Header("UI Elements")]
    public GameObject fishingPanel;
    public Text statusText;
    public Slider progressBar;
    public Text progressText;

    // Bait and fish state
    private Transform bait;
    private bool isBaitReady = false;
    private FishBehavior hookedFish = null;
    private float timeInHookRange = 0.0f;
    private bool isWaitingForHook = false;

    // Fight state
    private bool isFishing = false;
    private float lastPullTime;
    private int consecutiveTooHardPulls = 0;
    private int consecutiveNoPulls = 0;
    private float progress = 0f;

    // Fighting constants
    private const float minValidPullAcceleration = 2f;
    private const float maxValidPullAcceleration = 4f;
    private const float tooHardPullAcceleration = 4f;
    private const float progressIncrement = 20f;
    private const float progressDropPerWarning = 5f;
    private const int maxWarnings = 3;
    private const float pullDetectionTimeWindow = 0.5f;
    private const float noPullInterval = 1f;
    private const float tooHardPullCooldownDuration = 1f;
    private const float progressUpdateInterval = 0.5f;
    private const string targetSceneName = "TiltTestScene";
    private const float successMessageDuration = 3f;

    // Gesture detection
    private float gestureStartTime = 0f;
    private PullGestureState pullState = PullGestureState.None;
    private float lastProgressUpdateTime = 0f;
    private bool canDetectTooHardPull = true;

    private enum PullGestureState { None, FirstPhase }

    void Start()
    {
        // This will print the scene name if the object is in a scene, or "DontDestroyOnLoad" if it's in the persistent scene
        Debug.Log($"FishFightController is in scene: {gameObject.scene.name}");

        // Print the hierarchy path to help debug parent-child relationships
        Transform current = transform;
        string path = current.name;
        while (current.parent != null)
        {
            current = current.parent;
            path = current.name + "/" + path;
        }
        Debug.Log($"FishFightController hierarchy path: {path}");
        // Hide UI elements initially
        if (fishingPanel != null) fishingPanel.SetActive(false);
        if (statusText != null) statusText.gameObject.SetActive(false);
        if (progressBar != null) progressBar.gameObject.SetActive(false);
        if (progressText != null) progressText.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        if (!isBaitReady) return;

        if (isWaitingForHook)
        {
            CheckForFishHook();
        }
        else if (isFishing)
        {
            UpdateFishing();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isWaitingForHook || hookedFish != null) return;

        FishBehavior fish = other.GetComponent<FishBehavior>();
        if (fish != null && !fish.IsHooked())
        {
            timeInHookRange += Time.deltaTime;
            Debug.Log($"Fish in trigger range for {timeInHookRange} seconds");

            if (timeInHookRange >= hookTime)
            {
                HookFish(fish);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isWaitingForHook || hookedFish != null) return;

        FishBehavior fish = other.GetComponent<FishBehavior>();
        if (fish != null)
        {
            timeInHookRange = 0f;
            Debug.Log("Fish left trigger range - resetting timer");
        }
    }

    void CheckForFishHook()
    {
        if (hookedFish != null || bait == null) return;

        FishBehavior[] allFish = FindObjectsOfType<FishBehavior>();
        bool fishInRange = false;

        foreach (FishBehavior fish in allFish)
        {
            if (fish.IsHooked()) continue;

            float distanceToBait = Vector3.Distance(fish.transform.position, bait.position);

            if (distanceToBait <= hookDistance)
            {
                fishInRange = true;
                timeInHookRange += Time.deltaTime;
                Debug.Log($"Fish in range for {timeInHookRange} seconds");

                if (timeInHookRange >= hookTime)
                {
                    HookFish(fish);
                    break;
                }
            }
        }

        if (!fishInRange)
        {
            timeInHookRange = 0f;
        }
    }

    void HookFish(FishBehavior fish)
    {
        if (!isWaitingForHook) return;

        hookedFish = fish;
        hookedFish.gameObject.tag = "HookedFish";
        hookedFish.OnFishHooked(bait.position);
        RemoveOtherFish(hookedFish);
        isWaitingForHook = false;

        // Start fishing stage (which will show UI)
        hookedSource.Play();
        StartFishingStage();
        Debug.Log("Fish hooked! Starting fight sequence.");
    }

    // Make sure OnBaitReady keeps UI hidden
    public void OnBaitReady(Transform baitTransform)
    {
        bait = baitTransform;
        isBaitReady = true;
        isWaitingForHook = true;
        timeInHookRange = 0f;

        // Ensure UI stays hidden
        HideAllUI();

        Debug.Log("Bait is ready - starting to monitor for fish.");
    }
    void RemoveOtherFish(FishBehavior hookedFish)
    {
        FishBehavior[] allFish = FindObjectsOfType<FishBehavior>();
        foreach (FishBehavior fish in allFish)
        {
            if (fish != hookedFish)
            {
                Destroy(fish.gameObject);
            }
        }
    }

    void StartFishingStage()
    {
        isFishing = true;

        // Re-find UI elements if needed
        if (fishingPanel == null) fishingPanel = GameObject.Find("FishingPanel");
        if (statusText == null) statusText = GameObject.Find("StatusText")?.GetComponent<Text>();
        if (progressBar == null) progressBar = GameObject.Find("Slider")?.GetComponent<Slider>();
        if (progressText == null) progressText = GameObject.Find("ProgressText")?.GetComponent<Text>();

        // Show and reset UI elements
        if (fishingPanel != null)
        {
            fishingPanel.SetActive(true);
        }

        if (statusText != null)
        {
            statusText.gameObject.SetActive(true);
            statusText.text = "Start fishing!";
        }

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.value = 0f;
        }

        if (progressText != null)
        {
            progressText.gameObject.SetActive(true);
            progressText.text = "Progress: 0%";
        }

        // Rest of your existing StartFishingStage code...
        Vibration.Vibrate(1000);
        Debug.Log("Vibration should have started (first time)");
        Invoke(nameof(VibrateAgain), 0.5f);

        lastPullTime = Time.time;
        consecutiveTooHardPulls = 0;
        consecutiveNoPulls = 0;
        progress = 0f;
        lastProgressUpdateTime = Time.time;

        UpdateProgressUI();
    }
    void VibrateAgain()
    {
        Vibration.Vibrate(1000);
        Debug.Log("Vibration should have started (second time)");
    }

    void UpdateFishing()
    {
        DetectPullGesture();

        if (Time.time - lastPullTime >= noPullInterval)
        {
            consecutiveNoPulls++;
            lastPullTime = Time.time;
            progress -= progressDropPerWarning;
            if (progress < 0f) progress = 0f;
            UpdateProgressUI();

            if (consecutiveNoPulls >= maxWarnings)
            {
                isFishing = false;
                ShowStatus("Failed: fish runs away");
                Debug.Log("Fishing failed: fish runs away");
                failSource.Play();
            }
            else
            {
                ShowStatus($"Pull! ({consecutiveNoPulls}/{maxWarnings})");
                Debug.Log($"No pull detected. Warning {consecutiveNoPulls}/{maxWarnings}");
                Vibration.Vibrate(300);
                progress -= progressDropPerWarning;
                if (progress < 0f) progress = 0f;
                UpdateProgressUI();
            }
        }
    }

    void DetectPullGesture()
    {
        Vector3 accel = Input.acceleration;
        float accelY = accel.y;
        float magnitude = accel.magnitude;

        if (pullState == PullGestureState.None && accelY < -0.5f)
        {
            pullState = PullGestureState.FirstPhase;
            gestureStartTime = Time.time;
            Debug.Log("Pull gesture first phase detected");
        }

        if (pullState == PullGestureState.FirstPhase)
        {
            if (Time.time - gestureStartTime > pullDetectionTimeWindow)
            {
                pullState = PullGestureState.None;
                Debug.Log("Pull gesture timed out");
                return;
            }

            if (magnitude >= minValidPullAcceleration && magnitude <= maxValidPullAcceleration)
            {
                lastPullTime = Time.time;
                consecutiveNoPulls = 0;
                pullState = PullGestureState.None;

                if (Time.time - lastProgressUpdateTime >= progressUpdateInterval)
                {
                    progress += progressIncrement;
                    if (progress > 100f) progress = 100f;
                    lastProgressUpdateTime = Time.time;
                    UpdateProgressUI();
                    PullRodSource.Play();
                    Debug.Log($"Valid pull detected. Progress: {progress}%");

                    if (progress >= 100f)
                    {
                        isFishing = false;
                        ShowStatus("Success! You have the fish");
                        Debug.Log("Fishing succeeded");

                        if (hookedFish != null)
                        {
                            // Make sure this object persists
                            DontDestroyOnLoad(this.gameObject);
                            // Make fish persist
                            hookedFish.transform.parent = null;
                            DontDestroyOnLoad(hookedFish.gameObject);
                            Debug.Log("Fish and FishFightController set to persist");
                        }

                        Invoke(nameof(SwitchToTiltTestScene), successMessageDuration);
                    }
                }
            }
            else if (magnitude > tooHardPullAcceleration && canDetectTooHardPull)
            {
                consecutiveTooHardPulls++;
                progress -= progressDropPerWarning;
                if (progress < 0f) progress = 0f;
                UpdateProgressUI();
                Vibration.Vibrate(300);
                ShowStatus($"Too Hard! ({consecutiveTooHardPulls}/{maxWarnings})");

                if (consecutiveTooHardPulls >= maxWarnings)
                {
                    isFishing = false;
                    ShowStatus("Failed: your rod breaks");
                    Debug.Log("Fishing failed: rod breaks");
                    failSource.Play();
                }

                canDetectTooHardPull = false;
                Invoke(nameof(ResetTooHardPullCooldown), tooHardPullCooldownDuration);
            }
        }
    }
    private void PrepareForSceneTransition()
    {
        if (hookedFish != null)
        {
            hookedFish.transform.SetParent(null);
            DontDestroyOnLoad(hookedFish.gameObject);

            // Store original state
            PlayerPrefs.SetFloat("FishScale", 0.3f);
            PlayerPrefs.SetFloat("FishRotationY", 90f);
            PlayerPrefs.Save();
        }
    }
    void SwitchToTiltTestScene()
    {
        PrepareForSceneTransition();
        SceneManager.sceneLoaded += OnTiltSceneLoaded;
        SceneManager.LoadScene(targetSceneName);
    }

    void ResetTooHardPullCooldown()
    {
        canDetectTooHardPull = true;
    }
    void OnTiltSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == targetSceneName)
        {
            if (hookedFish != null)
            {
                hookedFish.PrepareForTiltScene();

                // Restore saved state
                float savedScale = PlayerPrefs.GetFloat("FishScale", 0.3f);
                float savedRotationY = PlayerPrefs.GetFloat("FishRotationY", 90f);

                hookedFish.transform.localScale = new Vector3(savedScale, savedScale, savedScale);
                hookedFish.transform.rotation = Quaternion.Euler(0f, savedRotationY, 0f);
            }
            SceneManager.sceneLoaded -= OnTiltSceneLoaded;
        }
    }
    void UpdateProgressUI()
    {
        if (progressBar != null)
        {
            progressBar.value = progress / 100f;
        }

        if (progressText != null)
        {
            progressText.text = $"Progress: {Mathf.RoundToInt(progress)}%";
        }
    }

    void ShowStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    public FishBehavior GetHookedFish()
    {
        return hookedFish;
    }

    public void ClearHookedFish()
    {
        if (hookedFish != null)
        {
            Destroy(hookedFish.gameObject);
            hookedFish = null;
        }
    }

    public void ResetFishing()
    {
        Debug.Log("Resetting fishing state - Start");

        // Reset all state variables
        isBaitReady = false;
        isWaitingForHook = false;
        if (hookedFish != null)
        {
            Destroy(hookedFish.gameObject);
            hookedFish = null;
        }
        timeInHookRange = 0f;
        isFishing = false;
        progress = 0f;
        lastPullTime = 0f;
        consecutiveTooHardPulls = 0;
        consecutiveNoPulls = 0;
        pullState = PullGestureState.None;

        // Re-find UI elements in case they were destroyed/recreated
        if (fishingPanel == null) fishingPanel = GameObject.Find("FishingPanel");
        if (statusText == null) statusText = GameObject.Find("StatusText")?.GetComponent<Text>();
        if (progressBar == null) progressBar = GameObject.Find("Slider")?.GetComponent<Slider>();
        if (progressText == null) progressText = GameObject.Find("ProgressText")?.GetComponent<Text>();

        // Hide all UI elements
        HideAllUI();

        // Cancel any ongoing invokes
        CancelInvoke();

        Debug.Log("Fishing controller reset complete");
    }
}