using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private Transform respawnPoint;

    private void Start()
    {
        // Find the GameObject in the scene with the "RespawnPoint" tag
        // and store a reference to its transform.
        respawnPoint = GameObject.FindGameObjectWithTag("RespawnPoint").transform;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider that entered the trigger has the "Player" tag
        // AND if it has a Rigidbody2D component attached.
        if (other.CompareTag("Player") && other.attachedRigidbody != null)
        {
            // Move the entire player GameObject (the one with the Rigidbody)
            // to the respawn point's position.
            other.attachedRigidbody.transform.position = respawnPoint.position;
        }
    }
}