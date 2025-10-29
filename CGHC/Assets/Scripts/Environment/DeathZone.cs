using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Check for the "Player" tag and a Rigidbody
        if (other.CompareTag("Player") && other.attachedRigidbody != null)
        {
            // 2. Get the new HealthSystem script
            HealthSystem playerHealth = other.attachedRigidbody.GetComponent<HealthSystem>();

            if (playerHealth != null)
            {
                // 3. Call the new instant death method
                playerHealth.InstantKillOrRespawn();
            }
        }
    }
}