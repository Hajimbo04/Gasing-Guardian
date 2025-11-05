using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainPanel;
    public GameObject levelSelectPanel;

    void Start()
    {
        mainPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
    }

    public void OnNewGameClicked()
    {
        mainPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    public void OnContinueClicked()
    {
        int highestLevel = GameProgress.GetHighestLevelUnlocked();
        GameProgress.LoadLevel(highestLevel);
    }

    public void OnQuitClicked()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void OnBackToMainMenuClicked()
    {
        mainPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
    }

    public void OnResetProgressClicked()
    {
        GameProgress.ResetProgress();
        LevelSelectMenu levelMenu = UnityEngine.Object.FindFirstObjectByType<LevelSelectMenu>();
        if (levelMenu != null)
        {
            levelMenu.RefreshButtons();
        }
    }
}