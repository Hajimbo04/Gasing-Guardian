using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Monobehaviour
{
	[Header)"References")]
	public PlayerMovementStats MoveStats;
	[SerializeField] private Collider2D _feetColl;
	[SerializeField] private Collider2D _bodyColl;

	private Rigidbody2D _rb;

	//movement variables
	private Vector2 _moveVelocity;
	private bool _isFacingRight;

	//collision check variables
	private RaycaseHit2D _groundHit;
	private RaycastHit2D _headHit;
	private bool _isGrounded;
	private bool _bumpedHead;

	private void Awake()
    {
		_isFacingRight = true;

		_rb = GetComponent<Rigidbody2D>();
    }

    #region Movement

	private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
		if (moveInput != Vector2.zero)
        {
			//check if need to turn

			Vector2 targetVelocity = Vector2.zero;
			if (InputManager.RunIsHeld)
            {
				targetVelocity = new Vector2(moveInput.x, 0f) * MoveStats.MaxRunSpeed;
            }
			else
            {
				targetVelocity = new Vector2(_moveVelocity.x, _rb.velocity.y);
            }
        }
    }