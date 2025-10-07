using UnityEngine;

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
    private UIManager uiManager; // NEW: Reference to the UI Manager

    private void Start()
    {
        Debug.Log("PlayerHealth Start called");

        currentLives = maxLives;

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

        // NEW: Find the UI Manager component
        GameObject uiObject = GameObject.Find("Canvas"); // Assumes UIManager is on the Canvas
        if (uiObject != null)
        {
            uiManager = uiObject.GetComponent<UIManager>();
        }

        // Initial UI update
        if (uiManager != null)
        {
            uiManager.UpdateLivesDisplay(currentLives);
        }

        Debug.Log("Player starting with " + currentLives + " lives.");
    }

    private void Update()
    {
        if (invulnerabilityTimer > 0)
        {
            invulnerabilityTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(bool isInstantKill)
    {
        // 1. Check for invulnerability
        if (!isInstantKill && invulnerabilityTimer > 0)
        {
            return;
        }

        // 2. Execute Damage Path
        if (isInstantKill)
        {
            // PATH A: INSTANT KILL / DEATH ZONE
            currentLives = 0;

            // UI Update: Lives immediately go to 0
            if (uiManager != null) uiManager.UpdateLivesDisplay(currentLives);

            // Teleport Player
            if (respawnPoint != null)
            {
                transform.position = respawnPoint.position;
            }
        }
        else
        {
            // PATH B: STANDARD DAMAGE / ENEMY HIT
            currentLives--;

            // Apply Invulnerability and Knockback Lock
            invulnerabilityTimer = invulnerabilityDuration;
            if (playerMovement != null)
            {
                playerMovement.ApplyKnockbackLock(invulnerabilityDuration);
            }

            // UI Update: Lives deducted
            if (uiManager != null) uiManager.UpdateLivesDisplay(currentLives);
        }

        // 3. Handle Game Over 
        if (currentLives <= 0)
        {
            Debug.Log("!!! GAME OVER !!!");

            // Reset to max lives 
            currentLives = maxLives;

            // UI Update: Lives reset to maxLives after game over
            if (uiManager != null) uiManager.UpdateLivesDisplay(currentLives);

            // Ensure respawn on Game Over
            if (respawnPoint != null)
            {
                transform.position = respawnPoint.position;
            }
        }
    }
}