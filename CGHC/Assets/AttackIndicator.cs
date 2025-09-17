using UnityEngine;

public class AttackIndicator : MonoBehaviour
{
    public float lifetime = 0.5f;
    private Transform _playerTransform;

    private Vector2 _offset;
    private PlayerMovement _playerMovement;

    private Vector2 _originalLocalScale;

    void Awake()
    {
        _originalLocalScale = transform.localScale;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Setup(Transform player, Vector2 offset, PlayerMovement playerMovementRef)
    {
        _playerTransform = player;
        _offset = offset;
        _playerMovement = playerMovementRef;
        UpdatePositionAndOrientation(); 
    }

    void Update()
    {
        if (_playerTransform != null)
        {
            UpdatePositionAndOrientation();
        }
    }

    private void UpdatePositionAndOrientation()
    {
        Vector2 newPosition = _playerTransform.position;

        if (_playerMovement._isFacingRight)
        {
            newPosition += _offset;
            transform.localScale = _originalLocalScale;
        }
        else
        {
            newPosition -= _offset;
            transform.localScale = new Vector2(-_originalLocalScale.x, _originalLocalScale.y);
        }

        transform.position = newPosition;
    }
}