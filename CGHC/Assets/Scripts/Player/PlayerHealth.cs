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

    // Reference to PlayerMovement
    private PlayerMovement playerMovement;

    private void Start()
    {
        currentLives = maxLives;

        GameObject respawnObject = GameObject.FindGameObjectWithTag("RespawnPoint");
        if (respawnObject != null)
        {
            respawnPoint = respawnObject.transform;
        }
        else
        {
            Debug.LogError("RespawnPoint GameObject not found. Make sure it is tagged 'RespawnPoint'!");
        }

        // Get the PlayerMovement component
        playerMovement = GetComponent<PlayerMovement>();

        Debug.Log("Player starting with " + currentLives + " lives.");
    }

    private void Update()
    {
        if (invulnerabilityTimer > 0)
        {
            invulnerabilityTimer -= Time.deltaTime;
        }

        // Optional Debug
        // string invulnStatus = invulnerabilityTimer > 0 ? " [INVULNERABLE]" : "";
        // Debug.Log("CURRENT LIVES: " + currentLives + invulnStatus);
    }

    public void TakeDamage(bool isInstantKill)
    {
        // 1. Check for invulnerability (skip damage if timer is active and it's not an instant kill)
        if (!isInstantKill && invulnerabilityTimer > 0)
        {
            Debug.LogWarning("Damage skipped due to Invulnerability!");
            return;
        }

        // 2. Execute Damage Path
        if (isInstantKill)
        {
            // PATH A: INSTANT KILL / DEATH ZONE
            currentLives = 0;

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
        }

        // 3. Handle Game Over (only runs if lives hit 0 after deduction)
        if (currentLives <= 0)
        {
            Debug.Log("!!! GAME OVER !!!");

            // Reset to max lives and ensure respawn on Game Over
            currentLives = maxLives;
            if (respawnPoint != null)
            {
                transform.position = respawnPoint.position;
            }
            else
            {
                Debug.LogError("Cannot perform Game Over respawn: RespawnPoint is missing.");
            }
        }
    }
}