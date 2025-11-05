using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI Reference")]
    public TextMeshProUGUI inGameScoreText;

    [Header("Scoring")]
    public int scorePerEnemy = 100;

    private int currentScore = 0;
    
    public int CurrentScore => currentScore;

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
            ResetScore();
        }
        else
        {
            if (inGameScoreText != null)
            {
                inGameScoreText.gameObject.SetActive(false);
            }
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreText();

        if (inGameScoreText != null)
        {
            inGameScoreText.gameObject.SetActive(true);
        }
    }

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