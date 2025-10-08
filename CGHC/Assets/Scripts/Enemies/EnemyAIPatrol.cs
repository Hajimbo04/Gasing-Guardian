using UnityEngine;

public class EnemyAIPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Vector3 target;
    private bool movingToB = true;

    [Header("Optional References")]
    public SpriteRenderer spriteRenderer; // assign in Inspector if you use flipX
    public Transform firePoint;

    private void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("EnemyAIPatrol: Missing patrol points! Please assign pointA and pointB.");
            enabled = false;
            return;
        }

        target = pointB.position;
    }

    private void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target) < 0.05f)
        {
            movingToB = !movingToB;
            target = movingToB ? pointB.position : pointA.position;
            Flip();
        }
    }

    private void Flip()
    {
        if (spriteRenderer != null)
            spriteRenderer.flipX = !spriteRenderer.flipX;

        if (firePoint != null)
        {
            Vector3 fpScale = firePoint.localScale;
            fpScale.x *= -1f;
            firePoint.localScale = fpScale;
        }

        // optional: if you flip by changing the root scale instead, uncomment:
        // Vector3 s = transform.localScale; s.x *= -1f; transform.localScale = s;
    }

    // <<< ADD THIS METHOD >>>
    public bool IsFacingRight()
    {
        // If using SpriteRenderer.flipX to mirror the sprite, return the logical facing direction:
        if (spriteRenderer != null)
            return !spriteRenderer.flipX;

        // Fallback: assume positive localScale.x means facing right
        return transform.localScale.x > 0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (pointA != null) Gizmos.DrawSphere(pointA.position, 0.05f);
        if (pointB != null) Gizmos.DrawSphere(pointB.position, 0.05f);
        Gizmos.DrawLine(pointA.position, pointB.position);
    }
}

