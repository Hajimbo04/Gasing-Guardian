// using UnityEngine;

// public class H_GrapplingHook : MonoBehaviour
// {
//     // ===================================
//     // 1. FIELDS (Variables)
//     // ===================================

//     [Header("Settings")]
//     [SerializeField] private float maxGrappleDistance = 15f;
//     [SerializeField] private float swingForce = 15f;

//     [Header("Component References")]
//     // NOTE: It's cleaner to get the LayerMask via script if it's based on a named layer.
//     [SerializeField] private LineRenderer lineRenderer;
    
//     // Private/Cached Components
//     private Rigidbody2D rb;
//     private DistanceJoint2D joint;

//     // Private State Variables
//     private LayerMask whatIsGrappleable; 
//     private Vector2 grapplePoint;
//     private bool isGrappling = false;


//     // ===================================
//     // 2. UNITY LIFECYCLE METHODS
//     // ===================================

//     void Awake()
//     {
//         // Get Rigidbody2D here since it's used in FixedUpdate
//         rb = GetComponent<Rigidbody2D>();
//     }

//     void Start()
//     {
//         // Get the DistanceJoint2D
//         joint = GetComponent<DistanceJoint2D>();
        
//         // Ensure components are off at start
//         if (joint) joint.enabled = false;
//         if (lineRenderer) lineRenderer.enabled = false;

//         // Get the layer mask for "Gasing" by name. This overrides any setting in the Inspector.
//         whatIsGrappleable = LayerMask.GetMask("Gasing");
//     }

//     void Update()
//     {
//         // Check for input (non-physics frame rate)
//         if (Input.GetMouseButtonDown(1)) // Right-click down (1)
//         {
//             TryGrapple();
//         }

//         if (Input.GetMouseButtonUp(1)) // Right-click up (1)
//         {
//             StopGrapple();
//         }

//         if (isGrappling && lineRenderer.enabled)
//         {
//             // Update the visual line when grappling
//             // Set position 1 to the player's current position
//             lineRenderer.SetPosition(1, transform.position);
//         }
//     }

//     void FixedUpdate()
//     {
//         // Apply physics forces here
//         if (isGrappling)
//         {
//             // Get input that's typically used for physics movement
//             float horizontalInput = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows

//             if (horizontalInput != 0)
//             {
//                 // Vector from the player to the grapple point
//                 Vector2 playerToGrapple = grapplePoint - (Vector2)transform.position;
                
//                 // Get the perpendicular vector for swinging (swing direction)
//                 // Using new Vector2(-y, x) gives a vector perpendicular to the original
//                 Vector2 swingDirection = new Vector2(-playerToGrapple.y, playerToGrapple.x).normalized;
                
//                 // Apply force for swinging
//                 rb.AddForce(swingDirection * horizontalInput * swingForce);
//             }
//         }
//     }


//     // ===================================
//     // 3. CUSTOM METHODS
//     // ===================================

//     private void TryGrapple()
//     {
//         // Convert mouse position to world space
//         Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

//         // Calculate direction for the Raycast
//         Vector2 direction = mousePosition - (Vector2)transform.position;

//         // Cast a ray from the player towards the mouse position
//         RaycastHit2D hit = Physics2D.Raycast(
//             transform.position, 
//             direction, 
//             maxGrappleDistance, 
//             whatIsGrappleable
//         );

//         if (hit.collider != null)
//         {
//             // Successfully hit something grappleable
//             isGrappling = true;
//             grapplePoint = hit.point;

//             // **1. Setup the DistanceJoint2D**
//             joint.enabled = true;
//             // The anchor point on the player is (0,0) by default, the connectedAnchor is the grapple point
//             joint.connectedAnchor = grapplePoint; 
//             // Set the distance for the joint to the distance upon hitting the target
//             joint.distance = Vector2.Distance(transform.position, grapplePoint); 

//             // **2. Setup the LineRenderer**
//             lineRenderer.enabled = true;
//             lineRenderer.SetPosition(0, grapplePoint); // End point 1: The fixed grapple point
//             lineRenderer.SetPosition(1, transform.position); // End point 2: The player (updated in Update)
//         }
//     }

//     private void StopGrapple()
//     {
//         // Reset state and disable components
//         isGrappling = false;
//         if (joint) joint.enabled = false;
//         if (lineRenderer) lineRenderer.enabled = false;

//         // NOTE: If you changed gravity or drag in TryGrapple, this is where you'd reset them.
//     }
// }