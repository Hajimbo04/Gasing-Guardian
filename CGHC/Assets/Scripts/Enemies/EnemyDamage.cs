using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float knockbackForce = 10f;
    public bool dealsInstantKill = false; // Set this to false for enemies, true for death zones

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider is the player
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.attachedRigidbody.GetComponent<PlayerHealth>();
            Rigidbody2D playerRB = other.attachedRigidbody;

            if (playerHealth != null)
            {
                // 1. Deduct a life/handle death
                playerHealth.TakeDamage(dealsInstantKill);

                // 2. Knockback the player (only for standard enemies)
                if (playerRB != null && !dealsInstantKill)
                {
                    // Calculate the direction away from the enemy
                    Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;

                    // Apply an impulse force
                    playerRB.linearVelocity = Vector2.zero; // Stop current velocity first
                    playerRB.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}