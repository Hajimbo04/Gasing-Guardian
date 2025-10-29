using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement; 

public class HealthSystem : MonoBehaviour
{
    public static HealthSystem Instance { get; private set; }

    [Header("Health Settings")]
    public int maxLives = 5; 
    private int currentLives;
    public int CurrentLives => currentLives;

    public event Action<int> OnLivesChanged;

    [Header("Respawn")]
    public Transform respawnPoint; 

    [Header("Feedback")]
    public float flashDuration = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Knockback")]
    public float knockbackForce = 7f;           // How hard the knockback is
    public float knockbackStunDuration = 0.2f;  // How long to stun the AI

    [Header("UI Panels")] 
    public GameObject gameOverPanel;

    // --- Private references for knockback ---
    private Rigidbody2D rb;
    private EnemyAI enemyAI;
    private EnemyRangedAI rangedAI;
    private PlayerMovement playerMovement; // <-- NEW: Reference to your script

    void Awake()
    {
        if (gameObject.CompareTag("Player"))
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        // --- General setup for BOTH Player and Enemies ---
        currentLives = maxLives;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (gameObject.CompareTag("Player"))
        {
            if (respawnPoint == null)
            {
                Debug.LogWarning("Respawn Point is not assigned to " + gameObject.name + "!");
            }
        }

        // --- Get references for knockback ---
        rb = GetComponent<Rigidbody2D>();
        enemyAI = GetComponent<EnemyAI>();
        rangedAI = GetComponent<EnemyRangedAI>();
        playerMovement = GetComponent<PlayerMovement>(); // <-- NEW: Get the PlayerMovement component
    }

    void Start()
    {
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        if (gameObject.CompareTag("Player"))
        {
            OnLivesChanged?.Invoke(currentLives);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // --- THIS IS THE FIX ---
        
        // 1. Unpause the game
        Time.timeScale = 1f;

        // 2. Hide the Game Over Panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // 3. Reset the player's health
        currentLives = maxLives;
        OnLivesChanged?.Invoke(currentLives);
        
        // --- END OF FIX ---

        // --- THIS IS THE FIX ---
        // 4. Reset Player's Rigidbody and Animator
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // Ensure it's not kinematic
            rb.gravityScale = 1f; // Restore default gravity (PlayerMovement will take over)
        }

        if (playerMovement != null)
        {
            // This calls the function we just made in PlayerMovement.cs
            playerMovement.TriggerRespawn(); 
        }
        // --- END OF FIX ---   
        // This is your existing code to move the player
        GameObject newRespawnObject = GameObject.Find("RespawnPoint");
        if (newRespawnObject != null)
        {
            respawnPoint = newRespawnObject.transform;
            transform.position = respawnPoint.position;
        }
        else
        {
            Debug.LogWarning("No 'RespawnPoint' found in the new scene: " + scene.name);
            transform.position = Vector3.zero;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null; 
        }
    }

    public void TakeDamage(int damageAmount, Vector2 damageSourcePosition)
    {
        // 1. Check for invincibility (dashing)
        if (playerMovement != null && playerMovement.IsDashing)
        {
            return; // Player is invincible, exit the function.
        }

        // 2. Check if already dead. If so, don't do anything.
        // This fixes the bug where hitting a dead player causes knockback.
        if (currentLives <= 0)
        {
            return; 
        }

        // 3. Apply damage
        currentLives -= damageAmount;

        // 4. Update UI
        if (gameObject.CompareTag("Player"))
        {
            OnLivesChanged?.Invoke(currentLives);
        }

        // 5. Check if the player just died
        if (currentLives <= 0)
        {
            Die(); // This will start the death routine
            
            // --- THIS IS THE FIX ---
            // We MUST stop here. Do not apply knockback or hurt anims on the death blow.
            return; 
        }

        // --- This code below will ONLY run if the player is NOT dead ---
        
        // 6. Apply "Hurt" animation
        if (playerMovement != null) playerMovement.TriggerHurtAnimation();

        // 7. Start flash
        if (spriteRenderer != null)
        {
            StopCoroutine("FlashRoutine");
            StartCoroutine(FlashRoutine());
        }

        // 8. Apply Knockback
        if (rb != null)
        {
            // Calculate direction and the final velocity
            Vector2 knockbackDirection = ((Vector2)transform.position - damageSourcePosition).normalized;
            knockbackDirection += Vector2.up * 0.2f; // Add the "pop"
            Vector2 knockbackVelocity = knockbackDirection.normalized * knockbackForce;

            // Stun the correct script
            if (enemyAI != null)
            {
                enemyAI.Stun(knockbackStunDuration);
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(knockbackVelocity, ForceMode2D.Impulse);
            }
            else if (rangedAI != null)
            {
                rangedAI.Stun(knockbackStunDuration);
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(knockbackVelocity, ForceMode2D.Impulse);
            }
            else if (playerMovement != null)
            {
                // We DON'T apply force. We tell the PlayerMovement script to handle it.
                playerMovement.ApplyKnockback(knockbackVelocity, knockbackStunDuration);
            }
        }
    }

public void InstantKillOrRespawn()
{
    // A. Check if the player is already dead to prevent repeated calls
    if (currentLives <= 0)
    {
        return;
    }

    // B. Reduce lives to zero
    currentLives = 0;

    // C. Update UI
    if (gameObject.CompareTag("Player"))
    {
        OnLivesChanged?.Invoke(currentLives);
    }

    // D. Trigger the death routine
    Die();
}

    private void Die()
    {
        StopAllCoroutines();
        if (spriteRenderer != null) { spriteRenderer.color = originalColor; }

        if (gameObject.CompareTag("Player"))
        {
            // --- CHANGE THIS ---
            // Instead of running the code here, start the coroutine
            StartCoroutine(PlayerDeathRoutine());
            // --- END OF CHANGE ---
        }
        else
        {
            // --- THIS IS THE MODIFIED PART ---
            Debug.Log(gameObject.name + " has died.");

            // 1. Check if the thing that died was an Enemy
            if (gameObject.CompareTag("Enemy"))
            {
                // 2. Tell the ScoreManager to add score
                ScoreManager.Instance.AddScore();
            }

            // 3. Destroy the enemy object
            Destroy(gameObject);
            // --- END OF MODIFICATION ---
        }
    }

private IEnumerator PlayerDeathRoutine()
{
    // 1. Tell PlayerMovement script to stop input AND disable colliders
    if (playerMovement != null) 
    {
        // We replace "SetDead(true)" with our new, better function
        playerMovement.SetDeadState(); 
    }

    // 2. Stop horizontal physics, but let gravity work
    if (rb != null)
    {
        // Stop horizontal slide, but allow player to fall
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); 
        
        // --- THIS IS THE FIX ---
        // We REMOVE the line that sets gravityScale to 0.
        // This lets the player's body fall flat.
        // rb.gravityScale = 0f; // <-- DELETED
    }

    // 3. Trigger the animation
    if (playerMovement != null) 
    {
        playerMovement.TriggerDeathAnimation();
    }

    // 4. Wait for the animation to play
    yield return new WaitForSeconds(1.5f);

    // 5. NOW, stop all physics completely before showing the panel
    if (rb != null)
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic; // Lock it in place
    }

    // 6. Show the Game Over panel
    Debug.Log(gameObject.name + " has run out of lives. Game Over.");
    
    if (gameOverPanel != null)
    {
        gameOverPanel.SetActive(true);
    }
    else
    {
        Debug.LogError("Game Over Panel is not assigned in HealthSystem!");
    }
    
    Time.timeScale = 0f;
}

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

}