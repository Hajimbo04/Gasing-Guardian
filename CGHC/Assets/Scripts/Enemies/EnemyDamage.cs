using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float knockbackForce = 15f;
    public bool dealsInstantKill = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.attachedRigidbody != null)
        {
            // Get necessary components from the player's main GameObject
            PlayerHealth playerHealth = other.attachedRigidbody.GetComponent<PlayerHealth>();
            PlayerMovement playerMovement = other.attachedRigidbody.GetComponent<PlayerMovement>();
            Rigidbody2D playerRB = other.attachedRigidbody;

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(dealsInstantKill);

                // Only apply knockback if it's not an instant kill
                if (!dealsInstantKill)
                {
                    if (playerRB != null && playerMovement != null)
                    {
                        // 1. Apply Knockback Force
                        Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
                        playerRB.linearVelocity = Vector2.zero;
                        playerRB.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

                        // 2. ACTIVATE THE LOCK
                        // Use the Invulnerability duration as the lock time
                        playerMovement.ApplyKnockbackLock(playerHealth.invulnerabilityDuration);

                        Debug.Log("KNOCKBACK APPLIED: Force = " + knockbackForce);
                    }
                    else
                    {
                        Debug.LogError("KNOCKBACK FAILED: PlayerMovement or Rigidbody is missing on Player.");
                    }
                }
            }
        }
         Debug.Log("Trigger entered with: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected!");
        }
    }
}