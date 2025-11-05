using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using TMPro; 

public class LevelClearManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject levelClearPanel;
    public Button nextLevelButton; 
    public TextMeshProUGUI finalTimeText; 
    public TextMeshProUGUI finalScoreText; 

    private string nextSceneToLoad;

    void Start()
    {
        levelClearPanel.SetActive(false);
    }

    public void ShowLevelClear(string nextSceneName, float finalTime, int finalScore)
    {
        nextSceneToLoad = nextSceneName;
        levelClearPanel.SetActive(true);
        Time.timeScale = 0f; 

        if (finalTimeText != null)
        {
            finalTimeText.text = "TIME: " + Mathf.FloorToInt(finalTime);
        }
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

    public void OnNextLevelClicked()
    {
        levelClearPanel.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneToLoad);
    }

    public void OnRetryClicked()
    {
        levelClearPanel.SetActive(false);   
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMainMenuClicked()
    {
        levelClearPanel.SetActive(false);   
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main_Menu");
    }
}