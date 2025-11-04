using UnityEngine;
using UnityEngine.SceneManagement; 

public class GoalTrigger : MonoBehaviour
{
    [Header("Level Info")]
    public int currentLevelNumber = 1; // <-- ADD THIS. Set this in the Inspector! (e.g., 1 for Level 1)

    [Header("Scene Loading")]
    public string sceneToLoad; // e.g., "Level_2"

    [Header("Progress Saving")]
    public bool unlockNextLevel = true; 
    public int levelToUnlock = 2;       

private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySFX("Reach Goal");
            // 1. Stop the timer
            LevelTimer.Instance.StopTimer();
            float finalTime = LevelTimer.Instance.CurrentTime;

            // --- NEW ---
            // 2. Get the final score
            int finalScore = ScoreManager.Instance.CurrentScore;
            // ---------

            // 3. Save progress (Unlock next level)
            if (unlockNextLevel)
            {
                GameProgress.UnlockLevel(levelToUnlock);
            }
            
            // 4. Save the time
            GameProgress.SaveLevelTime(currentLevelNumber, finalTime);
            
            // --- NEW ---
            // 5. Save the score
            GameProgress.SaveLevelScore(currentLevelNumber, finalScore);
            // ---------

            // 6. Find the LevelClearManager
            LevelClearManager levelClearManager = FindFirstObjectByType<LevelClearManager>();

            if (levelClearManager != null)
            {
                // 7. Tell the manager to show the panel, passing in time AND score
                levelClearManager.ShowLevelClear(sceneToLoad, finalTime, finalScore);
                
                // 8. Disable this goal trigger
                gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("LevelClearManager not found!");
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}