using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 10f;
    public float lifetime = 3f;
    public bool destroyOnHit = true;

    private Vector2 direction;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Automatically destroy after lifetime expires
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
        else
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        // Flip sprite depending on direction
        if (dir.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Transform root = other.transform.root;

        if (root.CompareTag("Player"))
        {
            PlayerHealth playerHealth = root.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(false);
            }
            else
            {
                Debug.LogWarning("PlayerHealth not found on Player root!");
            }
            Destroy(gameObject);
        }
    }
}
