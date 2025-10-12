using UnityEngine;

public class CloudController : MonoBehaviour
{
    // The speed at which the cloud will move. This can be set by the spawner.
    public float moveSpeed = 1f;

    // The x-coordinate at which the cloud will be destroyed.
    // This prevents clouds from accumulating off-screen forever.
    public float destroyXPosition = -15f;

    void Update()
    {
        // Move the cloud to the left based on its moveSpeed and the frame rate.
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

        // Check if the cloud has moved past its destruction point.
        if (transform.position.x < destroyXPosition)
        {
            // Destroy the cloud GameObject to clean it up.
            Destroy(gameObject);
        }
    }
}

