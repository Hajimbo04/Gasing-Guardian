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

        // **FIXED LOGIC:**
        // 1. Prioritize loading persistent data. Check if it exists first.
        if (GameData.Instance != null && GameData.Instance.playerLives.HasValue)
        {
            // If data exists, load it.
            currentLives = Mathf.Clamp(GameData.Instance.playerLives.Value, 0, maxLives);
            Debug.Log($"Loaded persistent lives: {currentLives}");
        }
        else
        {
            // 2. If no data exists (e.g., first time playing), THEN initialize to max lives.
            currentLives = maxLives;
            Debug.Log("No persistent lives data found. Starting with max lives.");
        }
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

    // The LoadLives() method is no longer needed as its logic is now in Awake().

    /// <summary>
    /// Called when transitioning to a new level to store current lives.
    /// This is called from the LevelGoal script.
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
            currentLives = 0;

            if (respawnPoint != null)
            {
                transform.position = respawnPoint.position;
            }
        }
        else
        {
            // PATH B: STANDARD DAMAGE / ENEMY HIT
            currentLives--;

            invulnerabilityTimer = invulnerabilityDuration;
            if (playerMovement != null)
            {
                playerMovement.ApplyKnockbackLock(invulnerabilityDuration);
            }
        }

        currentLives = Mathf.Max(0, currentLives);

        OnLivesChanged.Invoke(currentLives);

        // 3. Handle Game Over
        if (currentLives <= 0)
        {
            Debug.Log("!!! GAME OVER, resetting lives to max !!!");

            currentLives = maxLives;

            OnLivesChanged.Invoke(currentLives);

            if (respawnPoint != null)
            {
                transform.position = respawnPoint.position;
            }
        }
    }
}