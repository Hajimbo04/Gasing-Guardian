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
    public float knockbackForce = 7f;          
    public float knockbackStunDuration = 0.2f; 

    [Header("UI Panels")] 
    public GameObject gameOverPanel;

    private Rigidbody2D rb;
    private EnemyAI enemyAI;
    private EnemyRangedAI rangedAI;
    private PlayerMovement playerMovement; 

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
        currentLives = maxLives;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (gameObject.CompareTag("Player"))
        {
            if (respawnPoint == null)
            {
                Debug.LogWarning("Respawn Point is not assigned to " + gameObject.name + "!");
            }
        }

        rb = GetComponent<Rigidbody2D>();
        enemyAI = GetComponent<EnemyAI>();
        rangedAI = GetComponent<EnemyRangedAI>();
        playerMovement = GetComponent<PlayerMovement>();
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
        Time.timeScale = 1f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        currentLives = maxLives;
        OnLivesChanged?.Invoke(currentLives);
        
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; 
            rb.gravityScale = 1f;
        }

        if (playerMovement != null)
        {
            playerMovement.TriggerRespawn();
        }
        
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
        // 1. Check for dashing
        if (playerMovement != null && playerMovement.IsDashing)
        {
            return; // Player is invincible, exit the function.
        }

        // 2. Check if already dead.
        if (currentLives <= 0)
        {
            return; 
        }

        // 3. Apply damage
        currentLives -= damageAmount;

        if (gameObject.CompareTag("Enemy"))
        {
            AudioManager.Instance.PlaySFX("Enemy Hurt");
        }

        // 4. Update UI
        if (gameObject.CompareTag("Player"))
        {
            OnLivesChanged?.Invoke(currentLives);
        }

        // 5. Check if the player just died
        if (currentLives <= 0)
        {
            Die(); 
            return; 
        }
        
        // 6. Hurt animation
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
            Vector2 knockbackDirection = ((Vector2)transform.position - damageSourcePosition).normalized;
            knockbackDirection += Vector2.up * 0.2f; 
            Vector2 knockbackVelocity = knockbackDirection.normalized * knockbackForce;

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
                playerMovement.ApplyKnockback(knockbackVelocity, knockbackStunDuration);
            }
        }
    }

public void InstantKillOrRespawn()
{
    if (currentLives <= 0)
    {
        return;
    }

    currentLives = 0;

    if (gameObject.CompareTag("Player"))
    {
        OnLivesChanged?.Invoke(currentLives);
    }
    
    Die();
}

    private void Die()
    {
        StopAllCoroutines();
        if (spriteRenderer != null) { spriteRenderer.color = originalColor; }

        if (gameObject.CompareTag("Player"))
        {
            StartCoroutine(PlayerDeathRoutine());
        }
        else
        {
            Debug.Log(gameObject.name + " has died.");
            AudioManager.Instance.PlaySFX("Enemy Death");

            if (gameObject.CompareTag("Enemy"))
            {
                ScoreManager.Instance.AddScore();
            }

            Destroy(gameObject);
        }
    }

private IEnumerator PlayerDeathRoutine()
{
    if (playerMovement != null) 
    {
        playerMovement.SetDeadState(); 
    }

    if (rb != null)
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); 
    }

    if (playerMovement != null) 
    {
        playerMovement.TriggerDeathAnimation();
    }

    yield return new WaitForSeconds(1.5f);

    if (rb != null)
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic; 
    }

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