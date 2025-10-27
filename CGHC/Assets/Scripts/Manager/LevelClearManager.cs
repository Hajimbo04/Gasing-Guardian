using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for Button
using TMPro; // <-- ADD THIS at the top

public class LevelClearManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject levelClearPanel;
    public Button nextLevelButton; // We need this to disable it on the last level
    public TextMeshProUGUI finalTimeText; // Text to show completion time
    public TextMeshProUGUI finalScoreText; // Text to show final score (optional)

    // This will store the name of the next scene (e.g., "Level_2")
    private string nextSceneToLoad;

    void Start()
    {
        // Make sure the panel is hidden at the start
        levelClearPanel.SetActive(false);
    }

    // This is the public function your GoalTrigger will call
    public void ShowLevelClear(string nextSceneName, float finalTime, int finalScore)
    {
        nextSceneToLoad = nextSceneName;
        levelClearPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game

        // --- NEW LOGIC ---
        // Display the final time
        if (finalTimeText != null)
        {
            finalTimeText.text = "TIME: " + Mathf.FloorToInt(finalTime);
        }
        // --- END NEW LOGIC ---
        // Display the final score
        if (finalScoreText != null)
        {
            finalScoreText.text = "SCORE: " + finalScore;
        }

        if (string.IsNullOrEmpty(nextSceneToLoad))
        {
            nextLevelButton.interactable = false;
        }
        else
        {
            nextLevelButton.interactable = true;
        }
    }

    // --- Button Functions ---

    public void OnNextLevelClicked()
    {
        levelClearPanel.SetActive(false);
        // Unpause and load the next level
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneToLoad);
    }

    public void OnRetryClicked()
    {
        levelClearPanel.SetActive(false);   
        // Unpause and reload the *current* level
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMainMenuClicked()
    {
        levelClearPanel.SetActive(false);   
        // Unpause and load the main menu
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main_Menu");
    }
}