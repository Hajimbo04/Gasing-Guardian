using UnityEngine;

// This script requires the GameObject to have a Rigidbody2D and a Collider2D component.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    // --- Movement Variables ---
    [Header("Movement")]
    [Tooltip("The speed at which the player moves left and right.")]
    [SerializeField] private float moveSpeed = 5f;

    [Tooltip("The force applied when the player jumps.")]
    [SerializeField] private float jumpForce = 10f;

    // --- Fall & Jump Control ---
    [Header("Fall & Jump Control")]
    [Tooltip("Multiplier for gravity when the player is falling. Higher values mean a faster fall.")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [Tooltip("Multiplier for gravity when the jump button is released early. Creates a variable jump height.")]
    [SerializeField] private float lowJumpMultiplier = 2f;

    // --- Ground Check Variables ---
    [Header("Ground Check")]
    [Tooltip("The transform representing the position to check for ground from.")]
    [SerializeField] private Transform groundCheck;

    [Tooltip("The radius of the circle used to check for ground.")]
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Tooltip("The layer(s) that are considered ground.")]
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    // --- Attack Variables ---
    [Header("Attacks")]
    [Tooltip("The projectile prefab for the short-range throw attack.")]
    [SerializeField] private GameObject throwProjectilePrefab;

    [Tooltip("The projectile prefab for the long-range shoot attack.")]
    [SerializeField] private GameObject shootProjectilePrefab;

    [Tooltip("The point from which projectiles are fired.")]
    [SerializeField] private Transform firePoint;

    [Tooltip("The force applied to the thrown projectile.")]
    [SerializeField] private float throwForce = 8f;

    [Tooltip("The force applied to the shot projectile.")]
    [SerializeField] private float shootForce = 20f;

    [Tooltip("How long in seconds before a projectile is destroyed.")]
    [SerializeField] private float projectileLifetime = 5f;


    // --- Component References ---
    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isFacingRight = true;

    // --- Unity Methods ---

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        // Get the Rigidbody2D component attached to this GameObject.
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame.
    void Update()
    {
        // --- Input Handling ---
        // Get horizontal input (A/D keys or Left/Right arrow keys).
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // --- Flipping Character ---
        // Flip the character sprite based on movement direction.
        Flip();

        // --- Jump Logic ---
        // Check for jump input. The player can only jump if they are on the ground.
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // Check for throw attack input (e.g., left mouse button).
        if (Input.GetButtonDown("Fire1"))
        {
            ThrowAttack();
        }

        // Check for shoot attack input (e.g., right mouse button).
        if (Input.GetButtonDown("Fire2"))
        {
            ShootAttack();
        }
    }

    // FixedUpdate is called at a fixed interval and is used for physics calculations.
    void FixedUpdate()
    {
        // --- Ground Check ---
        // Check if the player is on the ground using a small circle at the player's feet.
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // --- Movement ---
        // Apply horizontal movement to the Rigidbody.
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // --- Enhanced Fall & Jump Logic ---
        if (rb.linearVelocity.y < 0)
        {
            // If the player is falling, apply the fallMultiplier to make them fall faster.
            // We multiply by (fallMultiplier - 1) because the base gravity is already applied.
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            // If the player is moving upwards but has released the jump button,
            // apply the lowJumpMultiplier to cut the jump short.
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    // --- Custom Methods ---

    /// <summary>
    /// Makes the player jump by applying an upward force.
    /// </summary>
    private void Jump()
    {
        // Reset vertical velocity before jumping to ensure a consistent jump height.
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        // We use Impulse to apply the force instantly.
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    }

    /// <summary>
    /// Flips the player's sprite to face the direction of movement.
    /// </summary>
    private void Flip()
    {
        if ((isFacingRight && horizontalInput < 0f) || (!isFacingRight && horizontalInput > 0f))
        {
            isFacingRight = !isFacingRight;
            // Multiply the player's x local scale by -1.
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    /// <summary>
    /// Instantiates and launches the short-range projectile.
    /// </summary>
    private void ThrowAttack()
    {
        if (throwProjectilePrefab && firePoint)
        {
            // Create the projectile at the fire point's position and rotation.
            GameObject projectile = Instantiate(throwProjectilePrefab, firePoint.position, firePoint.rotation);

            Destroy(projectile, projectileLifetime);

            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

            if (projectileRb)
            {
                // Determine the direction based on the isFacingRight flag.
                Vector2 fireDirection = isFacingRight ? Vector2.right : Vector2.left;

                // If the projectile sprite needs to be flipped to match the direction.
                if (!isFacingRight)
                {
                    Vector3 projectileScale = projectile.transform.localScale;
                    projectileScale.x *= -1;
                    projectile.transform.localScale = projectileScale;
                }

                // Apply force in the correct direction.
                projectileRb.AddForce(fireDirection * throwForce, ForceMode2D.Impulse);
            }
        }
    }

    /// <summary>
    /// Instantiates and launches the long-range projectile.
    /// </summary>
    private void ShootAttack()
    {
        if (shootProjectilePrefab && firePoint)
        {
            // Create the projectile at the fire point's position and rotation.
            GameObject projectile = Instantiate(shootProjectilePrefab, firePoint.position, firePoint.rotation);

            Destroy(projectile, projectileLifetime);

            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

            if (projectileRb)
            {
                // Determine the direction based on the isFacingRight flag.
                Vector2 fireDirection = isFacingRight ? Vector2.right : Vector2.left;

                // If the projectile sprite needs to be flipped to match the direction.
                if (!isFacingRight)
                {
                    Vector3 projectileScale = projectile.transform.localScale;
                    projectileScale.x *= -1;
                    projectile.transform.localScale = projectileScale;
                }

                // Apply force in the correct direction.
                projectileRb.AddForce(fireDirection * shootForce, ForceMode2D.Impulse);
            }
        }
    }

    // --- Gizmos ---

    // OnDrawGizmos is used to draw gizmos in the editor for visualization.
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
