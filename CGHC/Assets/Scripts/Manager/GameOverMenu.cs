using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    // This function will be hooked to the "Retry" button
    public void OnRetryClicked()
    {
        // 1. Unpause the game
        Time.timeScale = 1f;
        
        // 2. Reload the current level
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // This function will be hooked to the "Main Menu" button
    public void OnMainMenuClicked()
    {
        // 1. Unpause the game
        Time.timeScale = 1f;

        // 2. Load the Main Menu scene
        SceneManager.LoadScene("Main_Menu");
    }
}