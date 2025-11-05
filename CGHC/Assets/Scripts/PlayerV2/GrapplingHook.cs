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
    public float launchForce = 10f;

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
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        distanceJoint = GetComponent<DistanceJoint2D>();
        mainCamera = Camera.main;

        distanceJoint.enabled = false;
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2; 
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            StartGrapple();
        }

        if (Input.GetMouseButtonUp(1))
        {
            StopGrapple();
        }
        
        if (isGrappling && InputManager.JumpWasPressed)
        {
            StopGrapple(true); 
        }

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
        mainCamera = Camera.main;
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxGrappleDistance, grappleLayer);

        if (hit.collider != null)
        {
            isGrappling = true;
            grapplePoint = hit.point;
            playerMovement.SetGrappling(true);
            distanceJoint.enabled = true;
            distanceJoint.connectedAnchor = grapplePoint;
            distanceJoint.distance = Vector2.Distance(transform.position, grapplePoint);
            lineRenderer.enabled = true;
        }
    }

    private void StopGrapple(bool withLaunch = false)
    {
        if (!isGrappling) return;
        isGrappling = false;
        distanceJoint.enabled = false;
        lineRenderer.enabled = false;
        playerMovement.SetGrappling(false);

        if (withLaunch)
        {
            Vector2 launchDir = rb.linearVelocity.normalized;
            rb.AddForce(launchDir * launchForce, ForceMode2D.Impulse);
        }
    }

    private void HandleSwing()
    {
        float swingInput = InputManager.Movement.x;
        if (Mathf.Abs(swingInput) < 0.1f) return; 
        Vector2 ropeDirection = (grapplePoint - (Vector2)transform.position).normalized;
        Vector2 swingDirection = new Vector2(ropeDirection.y, -ropeDirection.x);
        rb.AddForce(swingDirection * swingInput * swingForce, ForceMode2D.Force);
    }

    private void DrawRope()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }
}