    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// This Singleton class manages game data that needs to persist
    /// across scene transitions (e.g., player lives, score, inventory).
    /// It uses DontDestroyOnLoad to ensure it remains active.
    /// </summary>
    public class GameData : MonoBehaviour
    {
        // --- Singleton Setup ---
        // Static property for easy access from anywhere (e.g., GameData.Instance.playerLives)
        public static GameData Instance { get; private set; }

        [Header("Persistent Data")]
        // The data we want to carry over to the next scene
        // CRITICAL FIX: Changed from 'int' to 'int?' (nullable integer) 
        // This allows us to check if the data has been saved before (using .HasValue).
        public int? playerLives = null; // Initialized to null to indicate no saved data yet.

        private void Awake()
        {
            // Enforce Singleton pattern
            if (Instance != null && Instance != this)
            {
                // If another instance exists (from the new scene), destroy this new one
                Destroy(gameObject);
                return;
            }

            // This is the one true instance
            Instance = this;

            // CRITICAL STEP: Ensure this object survives scene loads
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Handles saving the current state and loading the next scene.
        /// This should be called when the player reaches the goal/transition point.
        /// </summary>
        /// <param name="nextSceneName">The name of the scene to load next (e.g., "Level2").</param>
        public void TransitionToNextLevel(string nextSceneName)
        {
            // 1. Find the player's health component in the current scene
            // We assume the PlayerHealth script is on the player object
            PlayerHealth playerHealthComponent = FindObjectOfType<PlayerHealth>();

            if (playerHealthComponent != null)
            {
                // 2. Instruct the player component to save its current LIVES data into this manager
                // The SaveLives method in PlayerHealth will now update the 'playerLives' property (int?)
                playerHealthComponent.SaveLives();
            }
            else
            {
                Debug.LogError("Could not find PlayerHealth component to save data! Transitioning with old data.");
            }

            // 3. Load the next scene
            SceneManager.LoadScene(nextSceneName);

            // When the new scene loads, the new PlayerHealth script's Awake() method will read the saved data
            // from this GameData manager (which is still active).
        }
    }
