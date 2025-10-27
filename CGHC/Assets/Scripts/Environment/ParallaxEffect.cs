using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Tooltip("The camera this background will follow.")]
    public Transform cameraTransform;

    [Tooltip("How fast this layer moves. (0 = stays still, 1 = moves with camera).")]
    public Vector2 parallaxMultiplier;

    // --- Private variables ---
    private Vector3 lastCameraPosition;
    private float textureUnitSizeX; // For tiling

    void Start()
    {
        // If no camera is set, find the main camera
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        lastCameraPosition = cameraTransform.position;

        // Get the width of the sprite for tiling
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = (texture.width / sprite.pixelsPerUnit) * transform.localScale.x;
    }

    void LateUpdate()
    {
        // LateUpdate runs after the camera has finished moving
        
        // 1. Calculate how much the camera has moved
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // 2. Move this background by that amount, but scaled by the multiplier
        Vector3 moveAmount = new Vector3(deltaMovement.x * parallaxMultiplier.x, deltaMovement.y * parallaxMultiplier.y, 0);
        transform.position += moveAmount;

        // 3. Update the last position for the next frame
        lastCameraPosition = cameraTransform.position;

        // 4. This is the "Tiling" logic for the infinite background
        // If the camera has moved past the edge of our texture, we "wrap" the
        // background around to the other side.
        float distanceToCamera = cameraTransform.position.x - transform.position.x;

        if (distanceToCamera > textureUnitSizeX)
        {
            // Moved past the right edge
            transform.position = new Vector3(transform.position.x + textureUnitSizeX, transform.position.y, transform.position.z);
        }
        else if (distanceToCamera < -textureUnitSizeX)
        {
            // Moved past the left edge
            transform.position = new Vector3(transform.position.x - textureUnitSizeX, transform.position.y, transform.position.z);
        }
    }
}