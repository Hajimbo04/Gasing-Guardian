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
    public Transform ledgeCheck;   
    public float checkRadius = 0.1f;

    [Header("Melee Combat")]
    public float attackRange = 1f;   
    public int attackDamage = 1;   
    public float attackRate = 1.0f;  
    private float nextAttackTime = 0f; 

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

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        if (!isGrounded)
        {
            return; 
        }

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
            if (currentState == AIState.Chasing) 
            {
                playerTransform = null;
                currentState = AIState.Patrolling;
            }
        }
    }

    private void Patrol()
    {
        bool isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer);
        bool isNearLedge = !Physics2D.OverlapCircle(ledgeCheck.position, checkRadius, groundLayer);

        if (isTouchingWall || isNearLedge)
        {
            Flip();
        }

        float moveDirection = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(patrolSpeed * moveDirection, rb.linearVelocity.y);
    }

    private void Chase()
    {
        if (playerTransform == null)
        {
            currentState = AIState.Patrolling;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            
            FacePlayer();

            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackRate;
                
                HealthSystem playerHealth = playerTransform.GetComponent<HealthSystem>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage, transform.position);
                }
            }
            return; 
        }

        float moveDirection = (playerTransform.position.x > transform.position.x) ? 1f : -1f;

        if ((isFacingRight && moveDirection < 0) || (!isFacingRight && moveDirection > 0))
        {
            Flip();
        }

        rb.linearVelocity = new Vector2(chaseSpeed * moveDirection, rb.linearVelocity.y);
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
}