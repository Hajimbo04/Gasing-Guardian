using UnityEngine;

public class GasingProjectile : MonoBehaviour
{
    [Header("Gasing Settings")]
    public float speed = 15f;
    public float lifeTime = 2f;
    
    [Header("Gasing Damage")]
    public int damage = 1;

    [Header("Feedback")] 
    public GameObject impactEffectPrefab;
    public bool causesScreenShake = false; 

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            Vector2 velocity = rb.linearVelocity;
            
            // Handle Horizontal Direction with Scale
            if (velocity.x < -0.1f) // Moving left
            {
                
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                             
                velocity.x *= -1; 
            }
            else 
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }

            //Handle Vertical Angle with Rotation
        
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void Launch(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("PlayerProjectile") || other.CompareTag("Player") || other.isTrigger)
        {
            return;
        }

       
        if (other.CompareTag("Ground"))
        {
            AudioManager.Instance.PlaySFX("Ground Bullet Hit");
        }

        if (impactEffectPrefab != null)
        {
            Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
        }

        if (causesScreenShake) 
            {
                if (CameraShake.Instance != null)
                {
                    Debug.Log("SHAKE CALLED!"); 
                    CameraShake.Instance.StartShake(0.15f, 0.2f);
                }
                else
                {
                    Debug.LogWarning("Tried to shake, but CameraShake.Instance is NULL!"); 
                }
            }

        if (other.CompareTag("Enemy"))
        {
            HealthSystem enemyHealth = other.GetComponent<HealthSystem>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage, transform.position);
            }

            Destroy(gameObject);
            return; // Stop running the rest of this function
        }
        
        Destroy(gameObject);
    }
}