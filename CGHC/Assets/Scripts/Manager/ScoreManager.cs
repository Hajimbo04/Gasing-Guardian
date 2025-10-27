using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Required for TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI Reference")]
    public TextMeshProUGUI inGameScoreText;

    [Header("Scoring")]
    public int scorePerEnemy = 100;

    private int currentScore = 0;
    
    // Public property so other scripts can read the score
    public int CurrentScore => currentScore;

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
            ResetScore();
        }
        else
        {
            // Stop and hide score in MainMenu
            if (inGameScoreText != null)
            {
                inGameScoreText.gameObject.SetActive(false);
            }
        }
    }

    // Resets the score to 0 when a level starts/restarts
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreText();

        if (inGameScoreText != null)
        {
            inGameScoreText.gameObject.SetActive(true);
        }
    }

    // This will be called by the HealthSystem
    public void AddScore()
    {
        currentScore += scorePerEnemy;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (inGameScoreText != null)
        {
            inGameScoreText.text = "SCORE: " + currentScore;
        }
    }
}