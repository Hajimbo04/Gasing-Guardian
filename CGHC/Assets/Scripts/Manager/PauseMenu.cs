using UnityEngine;
using UnityEngine.SceneManagement; 

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject pauseMenuPanel;

    private bool isPaused = false;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Update()
    {
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

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true); 
        Time.timeScale = 0f;            
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;            
        isPaused = false;
    }
    public void OnContinueClicked()
    {
        ResumeGame();
    }

    public void OnRetryClicked()
    {
        ResumeGame(); 
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMainMenuClicked()
    {
        ResumeGame();

        if (HealthSystem.Instance != null)
        {
            Destroy(HealthSystem.Instance.gameObject);  
        }

        SceneManager.LoadScene("Main_Menu");
    }
}