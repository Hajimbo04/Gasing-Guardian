using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainPanel;
    public GameObject levelSelectPanel;
    // Add references to your Options, Credits panels here

    void Start()
    {
        // When the main menu loads, make sure the main panel is on
        // and the level select panel is off.
        mainPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
    }

    // --- MAIN MENU BUTTONS ---

    public void OnNewGameClicked()
    {
        // Turn off the main panel and turn on the level select panel
        mainPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    public void OnContinueClicked()
    {
        // Get the highest level and load it immediately
        int highestLevel = GameProgress.GetHighestLevelUnlocked();
        GameProgress.LoadLevel(highestLevel);
    }

    public void OnQuitClicked()
    {
        // This only works in a built game, not in the Unity Editor
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    // --- OTHER BUTTONS ---

    public void OnBackToMainMenuClicked()
    {
        // This is for a "Back" button on your level select panel
        mainPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
    }

    // --- DEBUGGING BUTTON ---
    public void OnResetProgressClicked()
    {
        // A helper button to test your system
        GameProgress.ResetProgress();
        
        // Optional: Refresh the level select screen if it's open
        // (You'd need to call the function from LevelSelectMenu.cs)
        LevelSelectMenu levelMenu = UnityEngine.Object.FindFirstObjectByType<LevelSelectMenu>();
        if (levelMenu != null)
        {
            levelMenu.RefreshButtons();
        }
    }
}