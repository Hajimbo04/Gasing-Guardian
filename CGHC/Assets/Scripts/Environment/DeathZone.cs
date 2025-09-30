using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.attachedRigidbody != null)
        {
            // Get the PlayerHealth script attached to the player's Rigidbody/main GameObject
            PlayerHealth playerHealth = other.attachedRigidbody.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Call TakeDamage(true) for an instant kill/respawn
                playerHealth.TakeDamage(true);
            }
        }
    }
}