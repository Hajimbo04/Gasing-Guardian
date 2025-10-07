using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;
    public GameObject bulletPrefab;

    [Header("Shooting Settings")]
    public float shootRange = 8f;
    public float shootCooldown = 1f;
    private float shootTimer;

    [Header("Layer Masks")]
    public LayerMask playerLayer;

    private EnemyAIPatrol aiPatrol; // For direction
    private bool canSeePlayer;

    void Start()
    {
        if (firePoint == null || bulletPrefab == null)
        {
            Debug.LogError($"[EnemyShooter] Missing bulletPrefab or firePoint on {gameObject.name}");
            return;
        }

        aiPatrol = GetComponent<EnemyAIPatrol>();
        shootTimer = 0f;
    }

    void Update()
    {
        shootTimer -= Time.deltaTime;

        // Check for player
        canSeePlayer = DetectPlayer();

        if (canSeePlayer && shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootCooldown; // Reset cooldown
        }
    }

    bool DetectPlayer()
    {
        if (aiPatrol == null) return false;

        Vector2 direction = aiPatrol.IsFacingRight() ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, shootRange, playerLayer);

        if (hit.collider != null)
        {
            Transform root = hit.collider.transform.root;
            if (root.CompareTag("Player"))
            {
                Debug.Log("Enemy sees player!");
                return true;
            }
        }
        return false;
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Missing bulletPrefab or firePoint on EnemyShooter!");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            Vector2 direction = aiPatrol.IsFacingRight() ? Vector2.right : Vector2.left;
            bulletScript.SetDirection(direction);
        }

        Debug.Log("Enemy fired a bullet!");
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint == null) return;
        Gizmos.color = Color.red;
        Vector3 dir = (aiPatrol != null && aiPatrol.IsFacingRight()) ? Vector3.right : Vector3.left;
        Gizmos.DrawLine(firePoint.position, firePoint.position + dir * shootRange);
    }
}
