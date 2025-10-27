using UnityEngine;

public class BoneProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public float lifeTime = 3f;
    public int damage = 1;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime); 
    }

    public void Launch(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("PlayerProjectile"))
        {
            return;
        }

        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, transform.position);
            }

            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}