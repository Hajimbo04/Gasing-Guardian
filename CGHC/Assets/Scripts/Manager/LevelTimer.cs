using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Required for TextMeshPro

public class LevelTimer : MonoBehaviour
{
    public static LevelTimer Instance { get; private set; }

    [Header("UI Reference")]
    public TextMeshProUGUI inGameTimerText;

    private float currentTime = 0f;
    private bool isRunning = false;

    // Public property so other scripts can read the time
    public float CurrentTime => currentTime;

    void Awake()
    {
        // Setup Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad is already on the parent Player object
        }
    }

    // Subscribe to the sceneLoaded event
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This is called every time a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the new scene is a level (and not the MainMenu)
        if (scene.name.StartsWith("Level_"))
        {
            StartTimer();
        }
        else
        {
            StopTimer(); // Stop and hide timer in MainMenu
            if (inGameTimerText != null)
            {
                inGameTimerText.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (isRunning)
        {
            currentTime += Time.deltaTime;
            
            // Update the in-game UI
            if (inGameTimerText != null)
            {
                // Mathf.FloorToInt rounds down to the nearest whole number
                inGameTimerText.text = "TIME: " + Mathf.FloorToInt(currentTime);
            }
        }
    }

    public void StartTimer()
    {
        currentTime = 0f;
        isRunning = true;
        if (inGameTimerText != null)
        {
            inGameTimerText.gameObject.SetActive(true);
        }
    }

    public void StopTimer()
    {
        isRunning = false;
        if (inGameTimerText != null)
        {
            inGameTimerText.gameObject.SetActive(false);
        }
    }
}