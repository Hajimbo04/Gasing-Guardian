using UnityEngine;
using System.Collections; 

public class EnemyFlyingRangedAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float chaseSpeed = 3f;
    public float flyingRadius = 10f; 
    
    [Header("AI Detection")]
    public float detectionRadius = 15f;
    public float attackRange = 10f;
    public LayerMask playerLayer;

    [Header("Combat Setup")]
    public GameObject projectilePrefab; 
    public Transform firePoint;
    public float attackCooldown = 2f;

    private Rigidbody2D rb;
    private HealthSystem health;
    private Transform playerTransform;
    private Vector2 startPosition; 

    private AIState currentState = AIState.Idle;
    private bool isFacingRight = true;
    private float nextFireTime = 0f;

    private bool isStunned = false;
    private float stunTimer = 0f;

    private enum AIState
    {
        Idle,
        Chasing,
        Attacking
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthSystem>();

        rb.gravityScale = 0; 
        startPosition = transform.position; 
    }

    void FixedUpdate()
    {
        // 1. Stun Check (stops all other logic)
        if (isStunned)
        {
            stunTimer -= Time.fixedDeltaTime;
            if (stunTimer <= 0)
            {
                isStunned = false;
            }
            return; 
        }

        // 2. Health Check
        if (health == null || health.CurrentLives <= 0)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 3. Find Player
        CheckForPlayer();

        // 4. Run State Machine
        switch (currentState)
        {
            case AIState.Idle:
                Idle();
                break;
            case AIState.Chasing:
                Chase();
                break;
            case AIState.Attacking:
                Attack();
                break;
        }
    }

    private void CheckForPlayer()
    {
        // Actively look for the player
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if (playerCollider == null)
        {
            // No player in range, go Idle
            playerTransform = null;
            currentState = AIState.Idle;
            return;
        }

        // Player is in range
        playerTransform = playerCollider.transform;

        // Check if player is in attack range
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        
        if (distanceToPlayer <= attackRange)
        {
            currentState = AIState.Attacking;
        }
        else
        {
            currentState = AIState.Chasing;
        }
    }

    // --- STATE LOGIC ---

    private void Idle()
    {
        // When idle, slowly drift back to the start position's X-axis
        float homeDistanceX = startPosition.x - transform.position.x;

        if (Mathf.Abs(homeDistanceX) > 0.1f)
        {
            // Move back towards home at a slow speed
            float moveDirection = Mathf.Sign(homeDistanceX);
            rb.linearVelocity = new Vector2(chaseSpeed * 0.5f * moveDirection, 0);
            
            if ((isFacingRight && moveDirection < 0) || (!isFacingRight && moveDirection > 0))
            {
                Flip();
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Chase()
    {
        if (playerTransform == null) return;

        // 1. Face the player
        FacePlayer();

        // 2. Calculate horizontal distance from its start position
        float homeDistance = Mathf.Abs(transform.position.x - startPosition.x);
        
        // 3. Get direction to player
        float moveDirection = (playerTransform.position.x > transform.position.x) ? 1f : -1f;

        // 4. Check if we are at the edge of our flyingRadius
        if (homeDistance >= flyingRadius)
        {
            bool isMovingAway = (transform.position.x > startPosition.x && moveDirection > 0) || 
                                (transform.position.x < startPosition.x && moveDirection < 0);

            if (isMovingAway)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }
        }
        
        // 5. If inside radius (or moving back towards it), chase the player
        rb.linearVelocity = new Vector2(chaseSpeed * moveDirection, 0);
    }

    private void Attack()
    {
        if (playerTransform == null) return;

        // Stop moving
        rb.linearVelocity = Vector2.zero;

        // Face the player
        FacePlayer();

        // Shoot on cooldown
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + attackCooldown;
            FireProjectile();
        }
    }

    // --- HELPER FUNCTIONS ---

    private void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        BoneProjectile projectileScript = projectile.GetComponent<BoneProjectile>(); 

        if (projectileScript != null && playerTransform != null)
        {
            // Aim at the player
            Vector2 direction = (playerTransform.position - firePoint.position).normalized;
            projectileScript.Launch(direction);
        }
    }

    private void FacePlayer()
    {
        if (playerTransform == null) return;

        float moveDirection = (playerTransform.position.x > transform.position.x) ? 1f : -1f;
        if ((isFacingRight && moveDirection < 0) || (!isFacingRight && moveDirection > 0))
        {
            Flip();
        }
    }

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

        if(rb != null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw Detection Radius (green)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw Attack Radius (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw Flying Radius (blue horizontal lines)
        Gizmos.color = Color.blue;
        Vector2 startPos = Application.isPlaying ? startPosition : (Vector2)transform.position;
        Vector2 leftLimit = startPos + Vector2.left * flyingRadius;
        Vector2 rightLimit = startPos + Vector2.right * flyingRadius;
        Gizmos.DrawLine(leftLimit + Vector2.up, leftLimit + Vector2.down);
        Gizmos.DrawLine(rightLimit + Vector2.up, rightLimit + Vector2.down);
    }
}