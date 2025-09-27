using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	// ────────────────────────────────
	// References
	// ────────────────────────────────
	[Header("References")]
	public PlayerMovementStats MoveStats;
	[SerializeField] private Collider2D _feetColl;
	[SerializeField] private Collider2D _bodyColl;
	[SerializeField] private Animator _animator;

	private Rigidbody2D _rb;

	// ────────────────────────────────
	// Gizmos
	// ────────────────────────────────
	[Header("Gizmos")]
	public float FacingDirectionGizmoLength = 1.5f;
	[Header("Debug Gizmo")]
	public bool ShowFacingDirectionRay;

	// ────────────────────────────────
	// Dash Variables
	// ────────────────────────────────
	[Header("Dash")]
	public float DashSpeed = 20f;
	public float DashDuration = 0.2f;
	public float DashCooldown = 0.5f;

	private bool _isDashing;
	private float _dashTimer;
	private float _dashCooldownTimer;
	private int _dashDirection; // 1 = right, -1 = left

	// ────────────────────────────────
	// Movement Variables
	// ────────────────────────────────
	private Vector2 _moveVelocity;
	public bool _isFacingRight;

	// ────────────────────────────────
	// Collision Checks
	// ────────────────────────────────
	private RaycastHit2D _groundHit;
	private RaycastHit2D _headHit;
	private bool _isGrounded;
	private bool _bumpedHead;

	// ────────────────────────────────
	// Jump Variables
	// ────────────────────────────────
	public float VerticalVelocity { get; private set; }
	private bool _isJumping;
	private bool _isFastFalling;
	private bool _isFalling;

	private float _fastFallTime;
	private float _fastFallReleaseSpeed;
	private int _numberOfJumpsUsed;

	// Apex
	private float _apexPoint;
	private float _timePastApexThreshold;
	private bool _isPastApexThreshold;

	// Jump buffer
	private float _jumpBufferTimer;
	private bool _jumpReleasedDuringBuffer;

	// Coyote time
	private float _coyoteTimer;

	// ────────────────────────────────
	// Wall Actions
	// ────────────────────────────────
	private RaycastHit2D _wallHit;
	private RaycastHit2D _wallHitBack;
	private bool _isWallSliding;
	private bool _isWallClimbing;
	private float _wallClimbTimer;
	private bool _isWallClinging;
	private float _wallClingTimer;
	private float _wallClimbCooldownTimer;

	// ────────────────────────────────
	// Unity Methods
	// ────────────────────────────────
	private void Awake()
	{
		_isFacingRight = true;
		_rb = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		CountTimers();
		JumpChecks();
		WallCling();
		HandleDashInput();
		HandleAnimations();

		if (ShowFacingDirectionRay)
		{
			Vector3 direction = _isFacingRight ? transform.right : -transform.right;
			Debug.DrawRay(transform.position, direction * FacingDirectionGizmoLength, Color.magenta);
		}
	}

	private void FixedUpdate()
	{
		CollisionChecks();
		Jump();
		WallClimb();
		WallJump();

		if (_isDashing)
		{
			HandleDashMovement();
		}
		else
		{
			if (_isGrounded || _isWallClinging)
				Move(MoveStats.GroundAcceleration, MoveStats.GroundDeceleration, InputManager.Movement);
			else
				Move(MoveStats.AirAcceleration, MoveStats.AirDeceleration, InputManager.Movement);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Vector3 direction = _isFacingRight ? transform.right : -transform.right;
		Gizmos.DrawRay(transform.position, direction * FacingDirectionGizmoLength);
	}

	#region Movement
	// ────────────────────────────────
	// Input Handlers
	// ────────────────────────────────
	private void HandleDashInput()
	{
		if (!_isDashing && _dashCooldownTimer <= 0 && Input.GetKeyDown(KeyCode.F))
		{
			if (InputManager.Movement.x != 0)
			{
				_isDashing = true;
				_dashTimer = DashDuration;
				_dashCooldownTimer = DashCooldown + DashDuration;
				_dashDirection = InputManager.Movement.x > 0 ? 1 : -1;
			}
		}
	}

	private void HandleAnimations()
	{
		// Running
		if (InputManager.Movement.x != 0 && _isGrounded && !_isDashing)
			_animator.SetBool("isWalking", true);
		else
			_animator.SetBool("isWalking", false);
	}

	// ────────────────────────────────
	// Movement
	// ────────────────────────────────
	private void Move(float acceleration, float deceleration, Vector2 moveInput)
	{
		if (moveInput != Vector2.zero)
		{
			TurnCheck(moveInput);
			Vector2 targetVelocity = new Vector2(moveInput.x, 0f) * MoveStats.MaxWalkSpeed;
			_moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
		}
		else
		{
			_moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
		}

		_rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocity.y);
	}

	private void TurnCheck(Vector2 moveInput)
	{
		if (_isFacingRight && moveInput.x < 0) Turn(false);
		else if (!_isFacingRight && moveInput.x > 0) Turn(true);
	}

	private void Turn(bool turnRight)
	{
		_isFacingRight = turnRight;
		Vector2 localScale = transform.localScale;
		localScale.x = _isFacingRight ? 1f : -1f;
		transform.localScale = localScale;
	}

	private void HandleDashMovement()
	{
		_rb.linearVelocity = new Vector2(_dashDirection * DashSpeed, 0f);
		_dashTimer -= Time.fixedDeltaTime;
		if (_dashTimer <= 0) _isDashing = false;
	}

	#endregion

	// ────────────────────────────────
	// Jump
	// ────────────────────────────────
	#region Jump

	// ────────────────────────────────
	// Jump Input & State Checks
	// ────────────────────────────────
	private void JumpChecks()
	{
		// Jump pressed → start buffer
		if (InputManager.JumpWasPressed)
		{
			_jumpBufferTimer = MoveStats.JumpBufferTime;
			_jumpReleasedDuringBuffer = false;
		}

		// Jump released → handle variable height / fast fall
		if (InputManager.JumpWasReleased)
		{
			if (_jumpBufferTimer > 0f)
				_jumpReleasedDuringBuffer = true;

			if (_isJumping && VerticalVelocity > 0f)
			{
				if (_isPastApexThreshold)
				{
					// Cut jump after apex
					_isPastApexThreshold = false;
					_isFastFalling = true;
					_fastFallTime = MoveStats.TimeForUpwardsCancel;
					VerticalVelocity = 0f;
				}
				else
				{
					// Cut jump mid-air
					_isFastFalling = true;
					_fastFallReleaseSpeed = VerticalVelocity;
				}
			}
		}

		// Jump buffer + coyote time
		if (_jumpBufferTimer > 0f && !_isJumping && (_isGrounded || _coyoteTimer > 0f))
		{
			InitiateJump(1);
			if (_jumpReleasedDuringBuffer)
			{
				_isFastFalling = true;
				_fastFallReleaseSpeed = VerticalVelocity;
			}
		}

		// Double jump
		else if (_jumpBufferTimer > 0f && _isJumping && _numberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed)
		{
			_isFastFalling = false;
			InitiateJump(1);
		}

		// Air jump after coyote expired
		else if (_jumpBufferTimer > 0f && _isFalling && _numberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed - 1)
		{
			InitiateJump(2);
			_isFastFalling = false;
		}

		// Landing
		if ((_isJumping || _isFalling) && _isGrounded && VerticalVelocity <= 0f)
		{
			_isJumping = false;
			_isFalling = false;
			_isFastFalling = false;
			_fastFallTime = 0f;
			_isPastApexThreshold = false;
			_numberOfJumpsUsed = 0;

			VerticalVelocity = Physics2D.gravity.y;
		}
	}

	// ────────────────────────────────
	// Apply Jump Forces
	// ────────────────────────────────
	private void InitiateJump(int numberOfJumpsUsed)
	{
		if (!_isJumping) _isJumping = true;

		_jumpBufferTimer = 0f;
		_numberOfJumpsUsed += numberOfJumpsUsed;
		VerticalVelocity = MoveStats.InitialJumpVelocity;
	}

	private void Jump()
	{
		// Jumping upward
		if (_isJumping)
		{
			// Bumped head → force fall
			if (_bumpedHead)
				_isFastFalling = true;

			// Ascending
			if (VerticalVelocity >= 0f)
			{
				HandleApexLogic();
			}
			// Descending
			else if (!_isFastFalling)
			{
				VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
			}
			else if (VerticalVelocity < 0f && !_isFalling)
			{
				_isFalling = true;
			}
		}

		// Fast fall (jump cut)
		if (_isFastFalling)
		{
			if (_fastFallTime >= MoveStats.TimeForUpwardsCancel)
			{
				VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
			}
			else
			{
				VerticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f, _fastFallTime / MoveStats.TimeForUpwardsCancel);
			}

			_fastFallTime += Time.fixedDeltaTime;
		}

		// Apply normal gravity while falling
		if (!_isGrounded && !_isJumping && !_isWallClimbing && !_isWallClinging)
		{
			if (!_isFalling) _isFalling = true;
			VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
		}
		// Cancel gravity while clinging/climbing
		else if (_isWallClimbing || _isWallClinging)
		{
			VerticalVelocity = 0f;
		}

		// Clamp fall speed
		VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MoveStats.MaxFallSpeed, 50f);

		_rb.linearVelocity = new Vector2(_rb.linearVelocity.x, VerticalVelocity);
	}

	// ────────────────────────────────
	// Handle Apex Hang Time
	// ────────────────────────────────
	private void HandleApexLogic()
	{
		_apexPoint = Mathf.InverseLerp(MoveStats.InitialJumpVelocity, 0f, VerticalVelocity);

		if (_apexPoint > MoveStats.ApexThreshold)
		{
			if (!_isPastApexThreshold)
			{
				_isPastApexThreshold = true;
				_timePastApexThreshold = 0f;
			}

			_timePastApexThreshold += Time.fixedDeltaTime;

			if (_timePastApexThreshold < MoveStats.ApexHangTime)
				VerticalVelocity = 0f;
			else
				VerticalVelocity = -0.01f;
		}
		else
		{
			VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
			if (_isPastApexThreshold) _isPastApexThreshold = false;
		}
	}

	#endregion


	#region Wall Actions

	private void WallClimb()
	{
		bool climbingInput = (_isFacingRight && InputManager.Movement.x > 0) || (!_isFacingRight && InputManager.Movement.x < 0);

		if (_isWallSliding && climbingInput && _wallClimbCooldownTimer <= 0)
		{
			if (!_isWallClimbing)
			{
				_isWallClimbing = true;
				_wallClimbTimer = MoveStats.WallClimbTimeLimit;
			}

			if (_wallClimbTimer > 0)
			{
				_rb.linearVelocity = new Vector2(_rb.linearVelocity.x, MoveStats.WallClimbSpeed);
				_wallClimbTimer -= Time.fixedDeltaTime;
			}
			else
			{
				_isWallClimbing = false;
				_wallClimbCooldownTimer = 0.5f; // short cooldown
			}
		}
		else
		{
			_isWallClimbing = false;
		}
	}

	private void WallJump()
	{
		if ((_isWallSliding || _isWallClinging) && InputManager.JumpWasPressed)
		{
			bool jumpingIntoWall = (_isFacingRight && InputManager.Movement.x > 0) || (!_isFacingRight && InputManager.Movement.x < 0);

			if (jumpingIntoWall)
			{
				// Reset states
				_isJumping = true;
				_isFalling = false;
				_isFastFalling = false;
				_numberOfJumpsUsed = 0;
				_wallClimbCooldownTimer = 0f;

				// Jump away from wall
				float jumpDirection = _isFacingRight ? -1f : 1f;
				_rb.linearVelocity = new Vector2(MoveStats.WallJumpForce * jumpDirection, MoveStats.WallJumpVerticalForce);
			}
		}
	}

	private void WallCling()
	{
		bool clingingInput = (_isFacingRight && InputManager.Movement.x > 0) || (!_isFacingRight && InputManager.Movement.x < 0);

		if (_isWallSliding && clingingInput)
		{
			if (!_isWallClinging)
			{
				_isWallClinging = true;
				_wallClingTimer = MoveStats.WallClingTime;
			}

			if (_wallClingTimer > 0)
			{
				_wallClingTimer -= Time.deltaTime;
			}

			if (_wallClingTimer <= 0 || _wallHitBack.collider == null)
			{
				_isWallClinging = false;
			}
		}
		else
		{
			_isWallClinging = false;
		}
	}

	#endregion


	#region Collision Checks

	private void IsGrounded()
	{
		Vector2 origin = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
		Vector2 size = new Vector2(_feetColl.bounds.size.x, MoveStats.GroundDetectionRayLength);

		_groundHit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, MoveStats.GroundDetectionRayLength, MoveStats.GroundLayer);
		_isGrounded = _groundHit.collider != null;
	}

	private void BumpedHead()
	{
		Vector2 origin = new Vector2(_feetColl.bounds.center.x, _bodyColl.bounds.max.y);
		Vector2 size = new Vector2(_feetColl.bounds.size.x * MoveStats.HeadWidth, MoveStats.HeadDetectionRayLength);

		_headHit = Physics2D.BoxCast(origin, size, 0f, Vector2.up, MoveStats.HeadDetectionRayLength, MoveStats.GroundLayer);
		_bumpedHead = _headHit.collider != null;
	}

	private void WallCheck()
	{
		Vector2 forward = _isFacingRight ? Vector2.right : Vector2.left;
		Vector2 back = _isFacingRight ? Vector2.left : Vector2.right;

		_wallHit = Physics2D.Raycast(transform.position, forward, MoveStats.WallDetectionRayLength, MoveStats.WallLayer);
		_wallHitBack = Physics2D.Raycast(transform.position, back, MoveStats.WallDetectionRayLength, MoveStats.WallLayer);

		if (_wallHit.collider != null && !_isGrounded)
		{
			_isWallSliding = true;
			_numberOfJumpsUsed = 0; // reset on wall slide
		}
		else
		{
			_isWallSliding = false;
			_isWallClimbing = false;
			_isWallClinging = false;
		}
	}

	private void CollisionChecks()
	{
		IsGrounded();
		BumpedHead();
		WallCheck();
	}

	#endregion


	#region Timers

	private void CountTimers()
	{
		_jumpBufferTimer -= Time.deltaTime;

		// Coyote time
		if (!_isGrounded)
			_coyoteTimer -= Time.deltaTime;
		else
			_coyoteTimer = MoveStats.JumpCoyoteTime;

		// Wall climb cooldown
		if (_wallClimbCooldownTimer > 0)
			_wallClimbCooldownTimer -= Time.deltaTime;

		// Dash cooldown
		if (_dashCooldownTimer > 0)
			_dashCooldownTimer -= Time.deltaTime;
	}
}
#endregion
