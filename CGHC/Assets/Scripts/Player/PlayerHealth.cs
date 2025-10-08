using UnityEngine;
using UnityEngine.Events; // Required for UnityEvent

/// <summary>
/// Manages player lives, invulnerability, respawning, and data persistence.
/// Broadcasts lives changes using a UnityEvent for decoupled UI updates.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Lives")]
    public int maxLives = 3;
    private int currentLives;

    [Header("Invulnerability")]
    public float invulnerabilityDuration = 0.5f; // Time player is immune and knocked back
    private float invulnerabilityTimer = 0f;

    [Header("Respawn")]
    private Transform respawnPoint;

    // References to other components
    private PlayerMovement playerMovement;

    // --- UNITY EVENT FOR UI UPDATES ---
    [Header("Events")]
    // This event broadcasts the current number of lives (int) to any listeners (like the UI Manager).
    public UnityEvent<int> OnLivesChanged;

    private void Awake()
    {
        Debug.Log("PlayerHealth Awake called");

        // 1. Initialize lives to max before attempting to load persistent data
        currentLives = maxLives;

        // 2. Load persistent data if available (must happen before Start)
        LoadLives();
    }

    private void Start()
    {
        // Find Respawn Point
        GameObject respawnObject = GameObject.FindGameObjectWithTag("RespawnPoint");
        if (respawnObject != null)
        {
            respawnPoint = respawnObject.transform;
        }
        else
        {
            Debug.LogError("RespawnPoint GameObject not found. Make sure it is tagged 'RespawnPoint'!");
        }

        // Get PlayerMovement component
        playerMovement = GetComponent<PlayerMovement>();

        // Initial UI update via the event.
        // This is called in Start() to ensure UI components are initialized and listening.
        OnLivesChanged.Invoke(currentLives);

        Debug.Log("Player starting with " + currentLives + " lives.");
    }

    private void Update()
    {
        if (invulnerabilityTimer > 0)
        {
            invulnerabilityTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Checks the GameData singleton for previously saved lives and loads them.
    /// </summary>
    public void LoadLives()
    {
        if (GameData.Instance != null && GameData.Instance.playerLives.HasValue)
        {
            // Set current lives from the saved value, clamped to maxLives
            currentLives = Mathf.Clamp(GameData.Instance.playerLives.Value, 0, maxLives);
            Debug.Log($"Loaded persistent lives: {currentLives}");

            // Invoke the event after loading to ensure the UI is correct immediately.
            OnLivesChanged.Invoke(currentLives);
        }
        else
        {
            Debug.Log("No persistent lives data found. Starting with max lives.");
        }
    }

    /// <summary>
    /// Called by GameData when transitioning to a new level to store current lives.
    /// </summary>
    public void SaveLives()
    {
        if (GameData.Instance != null)
        {
            // Store the current lives value in the persistent GameData object
            GameData.Instance.playerLives = currentLives;
            Debug.Log($"Saved player lives: {currentLives}");
        }
        else
        {
            Debug.LogError("GameData instance not found! Cannot save lives.");
        }
    }

    /// <summary>
    /// Deducts a life, handles invulnerability, and checks for game over.
    /// </summary>
    public void TakeDamage(bool isInstantKill)
    {
        // 1. Check for invulnerability
        if (!isInstantKill && invulnerabilityTimer > 0)
        {
            return;
        }

        // Skip damage if already dead
        if (currentLives <= 0) return;

        // --- Core Damage Logic ---
        if (isInstantKill)
        {
            // PATH A: INSTANT KILL / DEATH ZONE
            // Deduct the final life
            currentLives = 0;

            // Teleport Player immediately on death zone hit
            if (respawnPoint != null)
            {
                transform.position = respawnPoint.position;
            }
        }
        else
        {
            // PATH B: STANDARD DAMAGE / ENEMY HIT
            currentLives--;

            // Apply Invulnerability
            invulnerabilityTimer = invulnerabilityDuration;
            if (playerMovement != null)
            {
                // Apply knockback lock, handled by the PlayerMovement script
                playerMovement.ApplyKnockbackLock(invulnerabilityDuration);
            }
        }

        // Clamp lives to prevent negative numbers (though currentLives-- handles most of it)
        currentLives = Mathf.Max(0, currentLives);

        // --- CRITICAL FIX: INVOKE EVENT HERE after lives change ---
        // This ensures the UI updates to 2, 1, 0 before the Game Over reset.
        OnLivesChanged.Invoke(currentLives);

        // 3. Handle Game Over
        if (currentLives <= 0)
        {
            Debug.Log("!!! GAME OVER, resetting lives to max !!!");

            // Reset to max lives (ready for next attempt)
            currentLives = maxLives;

            // --- INVOKE EVENT HERE after reset ---
            // This updates the UI back to 3
            OnLivesChanged.Invoke(currentLives);

            // Ensure player respawns on Game Over
            if (respawnPoint != null)
            {
                transform.position = respawnPoint.position;
            }
        }
    }
}
