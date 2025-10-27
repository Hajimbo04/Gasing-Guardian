using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;

    [Header("AI Detection")]
    public float detectionRadius = 8f; 
    public LayerMask playerLayer;     
    public LayerMask groundLayer;
    public Transform groundCheck;
    public Transform wallCheck;
    public Transform ledgeCheck;    // <-- ADD THIS
    public float checkRadius = 0.1f;

    // --- NEW VARIABLES ---
    [Header("Melee Combat")]
    public float attackRange = 1f;   // How close to get before attacking
    public int attackDamage = 1;     // Damage dealt per attack
    public float attackRate = 1.0f;  // How many seconds between attacks
    private float nextAttackTime = 0f; // Timer for attack rate
    // ----------------------

    // Private variables
    private Rigidbody2D rb;
    private HealthSystem health;
    private bool isFacingRight = true;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isStunned = false;
    private float stunTimer = 0f;
    
    private Transform playerTransform;
    private AIState currentState = AIState.Patrolling;

    private enum AIState
    {
        Patrolling,
        Chasing
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthSystem>();
    }

// This is in EnemyAI.cs
    void FixedUpdate()
    {
        if (isStunned)
        {
            stunTimer -= Time.fixedDeltaTime;
            if (stunTimer <= 0)
            {
                isStunned = false; 
            }
            return; 
        }

        if (health == null || health.CurrentLives <= 0)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // This check is ONLY to stop "air-flipping"
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        if (!isGrounded)
        {
            return; // We are in the air, so do nothing.
        }

        // All logic below this line ONLY runs if we are on the ground
        CheckForPlayer();
        
        switch (currentState)
        {
            case AIState.Patrolling:
                Patrol();
                break;
            
            case AIState.Chasing:
                Chase();
                break;
        }
    }

    private void CheckForPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if (playerCollider != null)
        {
            playerTransform = playerCollider.transform;
            currentState = AIState.Chasing;
        }
        else
        {
            if (currentState == AIState.Chasing) // Only switch back if we *were* chasing
            {
                playerTransform = null;
                currentState = AIState.Patrolling;
            }
        }
    }

// This is in EnemyAI.cs
// This is in EnemyAI.cs
    private void Patrol()
    {
        // Now we do our ledge and wall checks inside Patrol
        bool isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer);
        bool isNearLedge = !Physics2D.OverlapCircle(ledgeCheck.position, checkRadius, groundLayer);

        if (isTouchingWall || isNearLedge)
        {
            Flip();
        }

        float moveDirection = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(patrolSpeed * moveDirection, rb.linearVelocity.y);
    }

    // --- CHASE FUNCTION IS NOW UPDATED ---
    private void Chase()
    {
        if (playerTransform == null)
        {
            currentState = AIState.Patrolling;
            return;
        }

        // --- NEW ATTACK LOGIC ---
        // Check distance to player
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRange)
        {
            // 1. STOP MOVING
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            
            // 2. FACE THE PLAYER
            FacePlayer();

            // 3. TRY TO ATTACK (on cooldown)
            if (Time.time >= nextAttackTime)
            {
                // Update timer
                nextAttackTime = Time.time + attackRate;
                
                // Deal damage
                HealthSystem playerHealth = playerTransform.GetComponent<HealthSystem>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage, transform.position);
                }
            }
            // We are in attack range, so don't run the "move" logic below
            return; 
        }
        // --- END OF NEW ATTACK LOGIC ---


        // If we are NOT in attack range, keep chasing
        float moveDirection = (playerTransform.position.x > transform.position.x) ? 1f : -1f;

        if ((isFacingRight && moveDirection < 0) || (!isFacingRight && moveDirection > 0))
        {
            Flip();
        }

        rb.linearVelocity = new Vector2(chaseSpeed * moveDirection, rb.linearVelocity.y);
    }

    // --- NEW HELPER FUNCTION ---
    private void FacePlayer()
    {
        if (playerTransform == null) return;

        float moveDirection = (playerTransform.position.x > transform.position.x) ? 1f : -1f;
        if ((isFacingRight && moveDirection < 0) || (!isFacingRight && moveDirection > 0))
        {
            Flip();
        }
    }
    // --------------------------

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }
    
    public void Stun(float duration)
    {
        isStunned = true;
        stunTimer = duration;

        // Stop all horizontal movement immediately
        if(rb != null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
}