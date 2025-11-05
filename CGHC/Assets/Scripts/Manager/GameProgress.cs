using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameProgress
{
    private const string HighestLevelKey = "HighestLevelUnlocked";
    private const string LevelTimeKey = "LevelTime_"; 
    private const string LevelScoreKey = "LevelScore_"; 

    public static void UnlockLevel(int levelNumber)
    {
        int currentHighestLevel = GetHighestLevelUnlocked();

        if (levelNumber > currentHighestLevel)
        {
            PlayerPrefs.SetInt(HighestLevelKey, levelNumber);
            PlayerPrefs.Save(); 
            Debug.Log("Saved new highest level: " + levelNumber);
        }
    }

    public static int GetHighestLevelUnlocked()
    {
        return PlayerPrefs.GetInt(HighestLevelKey, 1);
    }

    public static void LoadLevel(int levelNumber)
    {
        SceneManager.LoadScene("Level_" + levelNumber);
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(HighestLevelKey);
        PlayerPrefs.Save();
        Debug.Log("Game progress has been reset. Highest level is 1.");
    }
    public static void SaveLevelTime(int levelNumber, float time)
    {
        string key = LevelTimeKey + levelNumber;
        float bestTime = GetLevelTime(levelNumber);

        if (bestTime == 0f || time < bestTime)
        {
            PlayerPrefs.SetFloat(key, time);
            PlayerPrefs.Save();
            Debug.Log("Saved new best time for Level " + levelNumber + ": " + time);
        }
    }

    public static float GetLevelTime(int levelNumber)
    {
        string key = LevelTimeKey + levelNumber;
        return PlayerPrefs.GetFloat(key, 0f);
    }
    public static void SaveLevelScore(int levelNumber, int score)
    {
        string key = LevelScoreKey + levelNumber;
        int bestScore = GetLevelScore(levelNumber);

        if (score > bestScore)
        {
            PlayerPrefs.SetInt(key, score);
            PlayerPrefs.Save();
            Debug.Log("Saved new best score for Level " + levelNumber + ": " + score);
        }
    }

    public static int GetLevelScore(int levelNumber)
    {
        string key = LevelScoreKey + levelNumber;
        return PlayerPrefs.GetInt(key, 0);
    }
}