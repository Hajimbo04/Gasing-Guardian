using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(DistanceJoint2D))]
[RequireComponent(typeof(LineRenderer))]
public class GrapplingHook : MonoBehaviour
{
    [Header("Inspector Settings")]
    public LayerMask grappleLayer;
    public float maxGrappleDistance = 15f;
    public float swingForce = 50f;
    public float launchForce = 10f; // Force added when jumping off

    [Header("Component References")]
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    private LineRenderer lineRenderer;
    private DistanceJoint2D distanceJoint;
    private Camera mainCamera;

    // --- State ---
    private bool isGrappling = false;
    private Vector2 grapplePoint;

    void Awake()
    {
        // Get all required components
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        distanceJoint = GetComponent<DistanceJoint2D>();
        mainCamera = Camera.main;

        // Setup components
        distanceJoint.enabled = false;
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2; // A rope is just two points
    }

    void Update()
    {
        // --- Input ---
        // You can change "Input.GetMouseButtonDown(1)" to your InputManager
        if (Input.GetMouseButtonDown(1)) 
        {
            StartGrapple();
        }

        if (Input.GetMouseButtonUp(1))
        {
            StopGrapple();
        }
        
        // --- Handle Jump Release ---
        // You can change "Input.GetKeyDown(KeyCode.Space)" to "InputManager.JumpWasPressed"
        if (isGrappling && InputManager.JumpWasPressed)
        {
            StopGrapple(true); // Stop with a launch
        }

        // --- Visuals ---
        if (isGrappling)
        {
            DrawRope();
        }
    }

    void FixedUpdate()
    {
        if (isGrappling)
        {
            HandleSwing();
        }
    }

    private void StartGrapple()
    {
        // --- THIS IS THE FIX ---
        mainCamera = Camera.main;
        // --- END OF FIX ---
        // Get mouse position in world space
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;

        // Raycast to find a grapple point
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxGrappleDistance, grappleLayer);

        if (hit.collider != null)
        {
            isGrappling = true;
            grapplePoint = hit.point;

            // 1. Tell PlayerMovement to stop
            playerMovement.SetGrappling(true);

            // 2. Configure the DistanceJoint
            distanceJoint.enabled = true;
            distanceJoint.connectedAnchor = grapplePoint;
            distanceJoint.distance = Vector2.Distance(transform.position, grapplePoint);

            // 3. Enable the LineRenderer
            lineRenderer.enabled = true;
        }
    }

    private void StopGrapple(bool withLaunch = false)
    {
        if (!isGrappling) return;

        isGrappling = false;

        // 1. Disable joint and renderer
        distanceJoint.enabled = false;
        lineRenderer.enabled = false;

        // 2. Tell PlayerMovement to resume (this also syncs its velocity)
        playerMovement.SetGrappling(false);

        // 3. Apply launch force if "Jump" was pressed
        if (withLaunch)
        {
            // Get the current momentum direction and add launch force
            Vector2 launchDir = rb.linearVelocity.normalized;
            rb.AddForce(launchDir * launchForce, ForceMode2D.Impulse);
        }
    }

    private void HandleSwing()
    {
        // Get left/right input
        // You can change "Input.GetAxisRaw("Horizontal")" to "InputManager.Movement.x"
        float swingInput = InputManager.Movement.x;

        // No input, don't apply force
        if (Mathf.Abs(swingInput) < 0.1f) return; 

        // --- New Realistic Swing Logic ---

        // 1. Get the direction from the player *to* the grapple point
        Vector2 ropeDirection = (grapplePoint - (Vector2)transform.position).normalized;

        // 2. Calculate the perpendicular direction (tangent to the swing arc)
        // This gives us a vector that points "sideways" along the arc
        Vector2 swingDirection = new Vector2(ropeDirection.y, -ropeDirection.x);
        // 3. Apply force along this new direction
        // We multiply by swingInput so that pressing "left" (-1) reverses the force
        rb.AddForce(swingDirection * swingInput * swingForce, ForceMode2D.Force);
    }

    private void DrawRope()
    {
        // Draw the line from the player to the grapple point
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }
}