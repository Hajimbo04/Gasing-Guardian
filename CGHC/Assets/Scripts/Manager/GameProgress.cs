using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameProgress
{
    // This is the "key" we will use to save our data
    private const string HighestLevelKey = "HighestLevelUnlocked";
    private const string LevelTimeKey = "LevelTime_"; // Key prefix for saving time
    private const string LevelScoreKey = "LevelScore_"; // Key prefix for saving score

    // Call this function when a player completes a level
    public static void UnlockLevel(int levelNumber)
    {
        // Get the current highest level unlocked
        int currentHighestLevel = GetHighestLevelUnlocked();

        // Only save if this new level number is higher than the one we have
        if (levelNumber > currentHighestLevel)
        {
            PlayerPrefs.SetInt(HighestLevelKey, levelNumber);
            PlayerPrefs.Save(); // Save the changes to the device
            Debug.Log("Saved new highest level: " + levelNumber);
        }
    }

    // Call this to find out what level the player is on
    public static int GetHighestLevelUnlocked()
    {
        // Get the saved level. If no level is saved, return 1 (the default value).
        // This ensures Level 1 is *always* unlocked.
        return PlayerPrefs.GetInt(HighestLevelKey, 1);
    }

    // A helper function to load any level scene by its number
    public static void LoadLevel(int levelNumber)
    {
        // This assumes your scenes are named "Level_1", "Level_2", etc.
        SceneManager.LoadScene("Level_" + levelNumber);
    }

    // A helper function for testing so you can reset your progress
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(HighestLevelKey);
        PlayerPrefs.Save();
        Debug.Log("Game progress has been reset. Highest level is 1.");
    }
    // Call this to save a new time for a level
    public static void SaveLevelTime(int levelNumber, float time)
    {
        string key = LevelTimeKey + levelNumber;
        float bestTime = GetLevelTime(levelNumber);

        // We only save if this is the first time, or if the new time is better (faster)
        if (bestTime == 0f || time < bestTime)
        {
            PlayerPrefs.SetFloat(key, time);
            PlayerPrefs.Save();
            Debug.Log("Saved new best time for Level " + levelNumber + ": " + time);
        }
    }

    // Call this to get the best time for a level
    public static float GetLevelTime(int levelNumber)
    {
        string key = LevelTimeKey + levelNumber;
        // Returns the saved time, or 0f if no time is saved
        return PlayerPrefs.GetFloat(key, 0f);
    }
// Call this to save a new score for a level
    public static void SaveLevelScore(int levelNumber, int score)
    {
        string key = LevelScoreKey + levelNumber;
        int bestScore = GetLevelScore(levelNumber);

        // We only save if the new score is higher than the best
        if (score > bestScore)
        {
            PlayerPrefs.SetInt(key, score);
            PlayerPrefs.Save();
            Debug.Log("Saved new best score for Level " + levelNumber + ": " + score);
        }
    }

    // Call this to get the best score for a level
    public static int GetLevelScore(int levelNumber)
    {
        string key = LevelScoreKey + levelNumber;
        // Returns the saved score, or 0 if no score is saved
        return PlayerPrefs.GetInt(key, 0);
    }
}