using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Triggers the level transition, ensuring player data is saved first.
/// This script should be attached to the Goal/Exit trigger object.
/// </summary>
public class LevelGoal : MonoBehaviour
{
    [Tooltip("The name of the next scene to load (must be in Build Settings).")]
    public string nextLevelName = "Level2"; // CHANGE THIS IN THE INSPECTOR

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Check if the object entering the trigger is the player's root object
        // (Uses the same logic established in EnemyDamage.cs to handle child colliders)
        GameObject playerRoot = other.transform.root.gameObject;

        if (playerRoot.CompareTag("Player"))
        {
            Debug.Log("Goal reached! Attempting data save and transition.");

            // 2. Find the PlayerHealth script on the root player object
            PlayerHealth playerHealth = playerRoot.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // **CRITICAL STEP: SAVE THE DATA**
                playerHealth.SaveLives();

                // 3. Instruct the persistent GameData manager to load the next scene
                if (GameData.Instance != null)
                {
                    GameData.Instance.TransitionToNextLevel(nextLevelName);
                }
                else
                {
                    Debug.LogError("GameData instance not found! Cannot save persistence. Loading scene directly.");
                    SceneManager.LoadScene(nextLevelName);
                }
            }
            else
            {
                Debug.LogError("PlayerHealth script not found on player root object. Transition aborted.");
            }
        }
    }
}
