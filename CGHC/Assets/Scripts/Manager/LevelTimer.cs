using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class LevelTimer : MonoBehaviour
{
    public static LevelTimer Instance { get; private set; }

    [Header("UI Reference")]
    public TextMeshProUGUI inGameTimerText;

    private float currentTime = 0f;
    private bool isRunning = false;

    public float CurrentTime => currentTime;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Level_"))
        {
            StartTimer();
        }
        else
        {
            StopTimer(); 
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
            
            if (inGameTimerText != null)
            {
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