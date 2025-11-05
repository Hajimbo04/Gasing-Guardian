using UnityEngine;

public class CloudController : MonoBehaviour
{
    public float moveSpeed = 1f;

    public float destroyXPosition = -15f;

    void Update()
    {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

        if (transform.position.x < destroyXPosition)
        {
            Destroy(gameObject);
        }
    }
}

