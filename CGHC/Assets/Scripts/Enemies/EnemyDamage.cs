using UnityEngine;

/// <summary>
/// Handles damage and knockback applied to the player upon contact.
/// Correctly identifies the main Player GameObject even when hit by a child collider.
/// </summary>
public class EnemyDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float knockbackForce = 15f;
    public bool dealsInstantKill = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<AttackIndicator>() != null)
        {
            return;
        }

        // Debugging log to confirm collision is firing
        Debug.Log("Trigger entered with: " + other.name);

        // 1. Get the root GameObject (the main Player) from the colliding child collider.
        GameObject playerRoot = other.transform.root.gameObject;

        // 2. Check if the root object has the "Player" tag AND a Rigidbody2D attached
        if (playerRoot.CompareTag("Player"))
        {
            Debug.Log("Player detected!");

            // Get necessary components from the player's main GameObject (the root object)
            PlayerHealth playerHealth = playerRoot.GetComponent<PlayerHealth>();
            PlayerMovement playerMovement = playerRoot.GetComponent<PlayerMovement>();
            Rigidbody2D playerRB = playerRoot.GetComponent<Rigidbody2D>();

            // Ensure all required components are present on the root object
            if (playerHealth != null && playerRB != null)
            {
                playerHealth.TakeDamage(dealsInstantKill);

                // Only apply knockback if it's not an instant kill
                if (!dealsInstantKill)
                {
                    if (playerMovement != null)
                    {
                        // 1. Apply Knockback Force
                        // Calculate direction from the enemy (this object) to the player
                        Vector2 knockbackDirection = (playerRoot.transform.position - transform.position).normalized;

                        // Clear existing velocity before applying impulse
                        playerRB.linearVelocity = Vector2.zero;

                        // Apply force
                        playerRB.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

                        // 2. ACTIVATE THE LOCK
                        // Use the Invulnerability duration as the lock time
                        playerMovement.ApplyKnockbackLock(playerHealth.invulnerabilityDuration);

                        Debug.Log("KNOCKBACK APPLIED: Force = " + knockbackForce);
                    }
                    else
                    {
                        Debug.LogError("KNOCKBACK FAILED: PlayerMovement component is missing on the Player root object.");
                    }
                }
            }
            else
            {
                Debug.LogError("Player Health or Rigidbody2D is missing on the Player root object.");
            }
        }
    }
}
