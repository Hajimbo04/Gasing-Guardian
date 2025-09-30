using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Lives")]
    public int maxLives = 3;
    private int currentLives;

    [Header("Respawn")]
    private Transform respawnPoint;

    private void Start()
    {
        currentLives = maxLives;

        // Find the RespawnPoint object created earlier
        GameObject respawnObject = GameObject.FindGameObjectWithTag("RespawnPoint");
        if (respawnObject != null)
        {
            respawnPoint = respawnObject.transform;
        }
        else
        {
            Debug.LogError("RespawnPoint GameObject not found. Make sure it is tagged 'RespawnPoint'!");
        }

        // OPTIONAL: Update a UI text element to show lives
        // You'd need a UI Manager script to handle this properly,
        // but for now, we'll log it to the console.
        Debug.Log("Player starting with " + currentLives + " lives.");
    }

    // This method is called by the DeathZone or EnemyDamage scripts
    public void TakeDamage(bool isInstantKill)
    {
        if (isInstantKill)
        {
            currentLives = 0; // Death zone instant kill
        }
        else
        {
            currentLives--; // Enemy contact damage
        }

        if (currentLives > 0)
        {
            // Respawn: move the player back to the respawn point
            if (respawnPoint != null)
            {
                transform.position = respawnPoint.position;
            }
            Debug.Log("Life lost! Remaining lives: " + currentLives);
        }
        else
        {
            // Player is out of lives
            Debug.Log("Game Over!");
            // In a full game, you'd load a game over screen here.
            // For now, we'll just respawn them at max lives.
            currentLives = maxLives;
            if (respawnPoint != null)
            {
                transform.position = respawnPoint.position;
            }
        }
    }
}