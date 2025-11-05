using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Tooltip("The camera this background will follow.")]
    public Transform cameraTransform;

    [Tooltip("How fast this layer moves. (0 = stays still, 1 = moves with camera).")]
    public Vector2 parallaxMultiplier;

    private Vector3 lastCameraPosition;
    private float textureUnitSizeX; 

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        lastCameraPosition = cameraTransform.position;

        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = (texture.width / sprite.pixelsPerUnit) * transform.localScale.x;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        Vector3 moveAmount = new Vector3(deltaMovement.x * parallaxMultiplier.x, deltaMovement.y * parallaxMultiplier.y, 0);
        transform.position += moveAmount;

        lastCameraPosition = cameraTransform.position;

        float distanceToCamera = cameraTransform.position.x - transform.position.x;

        if (distanceToCamera > textureUnitSizeX)
        {
            transform.position = new Vector3(transform.position.x + textureUnitSizeX, transform.position.y, transform.position.z);
        }
        else if (distanceToCamera < -textureUnitSizeX)
        {
            transform.position = new Vector3(transform.position.x - textureUnitSizeX, transform.position.y, transform.position.z);
        }
    }
}