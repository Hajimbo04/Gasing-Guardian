using UnityEngine;

public class H_PlayerJetpackJoyride : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The upward force applied when the jetpack is active.")]
    public float jetpackThrust = 10f;

    [Tooltip("The maximum speed the player can reach while moving up.")]
    public float maxAscentSpeed = 5f;

    [Header("Component Reference")]
    private Rigidbody2D rb;

    void Awake()
    {
        // Get the Rigidbody2D component attached to the GameObject
        rb = GetComponent<Rigidbody2D>();

        // Ensure the Rigidbody2D exists and is set up correctly for 2D platforming/flying
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on this GameObject. Please add one!");
            // Disable the script if the required component is missing
            enabled = false;
            return;
        }

        // Set gravity scale to ensure the player falls when not thrusting
        // A typical value is 1, but you can adjust this in the Inspector.
        rb.gravityScale = 3f;
    }

    void FixedUpdate()
    {
        // FixedUpdate is used for physics-based movement for consistency
        
        // Check if the 'U' key is currently being held down
        if (Input.GetKey(KeyCode.U))
        {
            // --- Apply Upward Thrust ---

            // Check if the current upward velocity is less than the maximum ascent speed
            if (rb.linearVelocity.y < maxAscentSpeed)
            {
                // Apply a continuous upward force (impulse)
                // ForceMode.Force is good for continuous acceleration
                rb.AddForce(Vector2.up * jetpackThrust, ForceMode2D.Force);
            }
        }
        
        // When 'U' is released, no force is added, and the player will fall 
        // due to the Rigidbody2D's gravityScale setting.
    }
}