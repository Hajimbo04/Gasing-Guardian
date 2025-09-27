// H2_GrappleHook.cs

using UnityEngine;

public class H2_GrappleHook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private DistanceJoint2D _distanceJoint;
    private Rigidbody2D _rb;
    private PlayerMovement _playerMovement;

    [Header("Settings")]
    [SerializeField] private float _maxGrappleDistance = 10f;
    [SerializeField] private LayerMask _grappleableLayer;

    private Vector2 _grapplePoint;
    public bool IsGrappling { get; private set; }

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerMovement = GetComponent<PlayerMovement>();
        _distanceJoint.enabled = false;
        _lineRenderer.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // --- DEBUG ---: Check if the right-click input is detected at all.
            Debug.Log("Right mouse button pressed.");
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StopGrapple();
        }

        if (IsGrappling)
        {
            UpdateRope();
        }
    }

    private void StartGrapple()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)transform.position;

        // --- DEBUG ---: Announce that we are attempting a grapple.
        Debug.Log("Attempting to grapple...");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _maxGrappleDistance, _grappleableLayer);

        if (hit.collider != null)
        {
            // --- DEBUG ---: Log what object the raycast hit.
            Debug.Log("Raycast hit: " + hit.collider.name);

            if (hit.collider.CompareTag("Grappleable"))
            {
                // --- DEBUG ---: Confirmation that everything worked.
                Debug.Log("Grapple successful! Attaching to " + hit.collider.name);
                
                _grapplePoint = hit.point;
                IsGrappling = true;

                _distanceJoint.connectedAnchor = _grapplePoint;
                _distanceJoint.distance = Vector2.Distance(transform.position, _grapplePoint);
                _distanceJoint.enabled = true;

                _lineRenderer.SetPosition(0, transform.position);
                _lineRenderer.SetPosition(1, _grapplePoint);
                _lineRenderer.enabled = true;
            }
            else
            {
                // --- DEBUG ---: The object hit was on the right layer, but had the wrong tag.
                Debug.LogError("Error: Hit object '" + hit.collider.name + "' does not have the 'Grappleable' tag.");
            }
        }
        else
        {
            // --- DEBUG ---: The raycast was fired but didn't hit anything on the specified layer.
            Debug.LogWarning("Grapple failed: No grappleable object found in range/direction.");
        }
    }

    private void StopGrapple()
    {
        if (IsGrappling)
        {
            // --- DEBUG ---: Confirm that we are stopping the grapple.
            Debug.Log("Stopping grapple.");
        }
        
        IsGrappling = false;
        _distanceJoint.enabled = false;
        _lineRenderer.enabled = false;
    }

    private void UpdateRope()
    {
        _lineRenderer.SetPosition(0, transform.position);
    }
}