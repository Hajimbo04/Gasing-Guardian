using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject pauseMenuPanel;

    private bool isPaused = false;

    void Start()
    {
        // Make sure the game starts unpaused
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Update()
    {
        // Check for the Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // --- Core Pause Functions ---

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true); // Show the panel
        Time.timeScale = 0f;            // This is the magic line that pauses all physics and Update loops
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false); // Hide the panel
        Time.timeScale = 1f;             // This unpauses the game
        isPaused = false;
    }

    // --- Button Functions ---

    // This function will be called by the "Continue" button
    public void OnContinueClicked()
    {
        ResumeGame();
    }

    // This function will be called by the "Retry" button
    public void OnRetryClicked()
    {
        // We MUST unpause the game *before* reloading the scene
        ResumeGame(); 
        
        // Reload the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // This function will be called by the "Back to Main Menu" button
    public void OnMainMenuClicked()
    {
        // We MUST unpause the game *before* changing scenes
        ResumeGame();

        if (HealthSystem.Instance != null)
        {
            Destroy(HealthSystem.Instance.gameObject);  
        }

        // Load your main menu scene (make sure the name is correct!)
        SceneManager.LoadScene("Main_Menu");
    }
}