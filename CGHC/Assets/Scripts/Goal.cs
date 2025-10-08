using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script is placed on the goal object (e.g., a door or finish line).
/// It detects the player and initiates the scene transition using the persistent GameData manager.
/// </summary>
public class LevelGoal : MonoBehaviour
{
    [Tooltip("The name of the next scene to load (must match the name in Build Settings).")]
    public string nextLevelName = "Level2"; // IMPORTANT: Change this in the Inspector!

    // This handles 2D physics triggers
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Get the root GameObject (The main Player object) from the colliding child collider.
        GameObject rootObject = other.transform.root.gameObject;

        // 2. Check if the root object has the "Player" tag
        if (rootObject.CompareTag("Player"))
        {
            Debug.Log($"Goal reached! Attempting transition to {nextLevelName}.");

            if (GameData.Instance != null)
            {
                // CRITICAL CALL: This function first saves the player data, then loads the scene.
                GameData.Instance.TransitionToNextLevel(nextLevelName);
            }
            else
            {
                // Fallback for testing a level directly
                Debug.LogWarning("GameData instance not found! Loading scene directly without persistence.");
                SceneManager.LoadScene(nextLevelName);
            }
        }
    }
}
