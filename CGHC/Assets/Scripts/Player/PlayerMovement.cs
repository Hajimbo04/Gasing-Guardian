using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")] 
    public PlayerMovementStats MoveStats;
    [SerializeField] private Collider2D _feetColl;
    [SerializeField] private Collider2D _bodyColl;

    [Header("Gizmos")]
    public float FacingDirectionGizmoLength = 1.5f;
    [Header("Debug Gizmo")]
    public bool ShowFacingDirectionRay;

    // --- Core Components ---
    private Rigidbody2D _rb;
    private Animator _anim; 

    // --- Animation  ---
    private int _isWalkingHash; 
    private int _isJumpingHash; 
    private int _hurtHash;  
    private int _deathHash;  
    private int _respawnHash;
    
    // --- State Variables ---
    public bool _isFacingRight { get; private set; }
    public float VerticalVelocity { get; private set; }
    
    // --- Movement State ---
    private Vector2 _moveVelocity;
    
    // --- Collision State ---
    private bool _isGrounded;
    private bool _bumpedHead;
    private bool _isWallDetected;
    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    private RaycastHit2D _wallHit;

    // --- Jump State ---
    private bool _isJumping;
    private bool _isFalling;
    private bool _isFastFalling;
    private float _fastFallTime;
    private float _fastFallReleaseSpeed;
    private int _numberOfJumpsUsed;
    
    // --- Wall State ---
    private bool _isWallSliding;

    // --- Apex State ---
    private float _apexPoint;
    private float _timePastApexThreshold;
    private bool _isPastApexThreshold;

    // --- Knockback State ---
    private bool _isKnockedBack;
	private float _knockbackTimer;

	// --- Dash State ---
	private bool _isDashing;
	public bool IsDashing => _isDashing; 
	private bool _dashWasPressed;
	private float _dashTimer;
	private float _dashCooldownTimer;

	// --- Grapple State ---
    private bool _isGrappling;
    public bool IsGrappling => _isGrappling;

    // --- Dead State ---
    private bool _isDead;

    // --- Timers ---
    private float _jumpBufferTimer;
    private bool _jumpReleasedDuringBuffer;
    private float _coyoteTimer;
    private float _wallStickTimer;
    private float _wallJumpCooldownTimer;
    
    private void Awake()
    {
        _isFacingRight = true;
        _rb = GetComponent<Rigidbody2D>();
 
        if (_anim == null) _anim = GetComponentInChildren<Animator>();

        _isWalkingHash = Animator.StringToHash("isWalking");
        _isJumpingHash = Animator.StringToHash("isJumping");
        _hurtHash = Animator.StringToHash("Hurt");
        _deathHash = Animator.StringToHash("Death");
        _respawnHash = Animator.StringToHash("Respawn");
    }

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			_dashWasPressed = true;
		}
		
		CountTimers();

		if (!_isKnockedBack && !_isDead)
		{
			JumpChecks();
			DashChecks();
		}
        HandleAnimations();
		DrawDebugRays();
	}

	private void FixedUpdate()
	{
		CollisionChecks();
        if (!_isDashing && !_isGrappling && !_isDead)
        {
            HandleWallSlide();
            Jump(); 
        }

        else if (_isDead)
        {
            VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
            VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MoveStats.MaxFallSpeed, 50f);
        }

		Vector2 input = _isKnockedBack || _isWallSliding || _isDashing || _isGrappling || _isDead ? Vector2.zero : InputManager.Movement;
		if (!_isKnockedBack && !_isWallSliding && !_isDashing && !_isGrappling && !_isDead)
		{
			if (_isGrounded)
			{
				Move(MoveStats.GroundAcceleration, MoveStats.GroundDeceleration, input);
			}
			else
			{
				Move(MoveStats.AirAcceleration, MoveStats.AirDeceleration, input);
			}
		}
		else if (_isWallSliding)
		{
			_moveVelocity.x = 0;
		}

	
		if (!_isGrappling)
		{
			_rb.linearVelocity = new Vector2(_moveVelocity.x, VerticalVelocity);
		}
	}    
    
    #region Movement

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            TurnCheck(moveInput);
            Vector2 targetVelocity = new Vector2(moveInput.x, 0f) * MoveStats.MaxWalkSpeed;
            _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else if (moveInput == Vector2.zero)
        {
            _moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if (_isFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }
        else if (!_isFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
    }

	private void Turn(bool turnRight)
	{
		_isFacingRight = turnRight;
		Vector2 localScale = transform.localScale;
		float sizeX = Mathf.Abs(localScale.x);

		localScale.x = _isFacingRight ? sizeX : -sizeX;
		transform.localScale = localScale;
	}

    public void SetGrappling(bool isGrappling)
    {
        _isGrappling = isGrappling;

        if (isGrappling)
        {
            _moveVelocity.x = 0;
            VerticalVelocity = 0;
        }
        else
        {
            VerticalVelocity = _rb.linearVelocity.y;
            _moveVelocity.x = _rb.linearVelocity.x;
            _isFalling = true;
            _isJumping = false;
        }
    }

    #endregion

    #region Animation

    private void HandleAnimations()
    {
        if (_anim == null) return; 
        if (_isDead) return; 

        bool isWalking = _isGrounded && Mathf.Abs(InputManager.Movement.x) > 0.1f && !_isDashing && !_isGrappling && !_isKnockedBack;
        _anim.SetBool(_isWalkingHash, isWalking);
        _anim.SetBool(_isJumpingHash, _isJumping);

    }
    public void TriggerHurtAnimation()
    {
        AudioManager.Instance.PlaySFX("Player Hurt");
        if (_anim == null) return;
        _anim.SetTrigger(_hurtHash);
    }

    public void TriggerDeathAnimation()
    {
        AudioManager.Instance.PlaySFX("Player Death");
        if (_anim == null) return;
        _anim.SetTrigger(_deathHash);
    }

    public void TriggerRespawn()
    {
        if (_anim == null) return;
        _isDead = false;
        _anim.SetTrigger(_respawnHash);
        if (_feetColl != null) _feetColl.enabled = true;
        if (_bodyColl != null) _bodyColl.enabled = true;
    }

    public void SetDead(bool isDead)
    {
        _isDead = isDead;
    }

    public void SetDeadState()
    {
        _isDead = true;
        if (_feetColl != null) _feetColl.enabled = false;
        if (_bodyColl != null) _bodyColl.enabled = false;
    }

    #endregion

    #region Jump Logic

    private void JumpChecks()
    {
        // 1. Wall Jump 
        if (_jumpBufferTimer > 0f && _isWallSliding)
        {
            WallJump();
            return; 
        }

        // 2. Jump Input
        if (InputManager.JumpWasPressed)
        {
            _jumpBufferTimer = MoveStats.JumpBufferTime;
            _jumpReleasedDuringBuffer = false;
        }

        if (InputManager.JumpWasReleased)
        {
            if (_jumpBufferTimer > 0f)
            {
                _jumpReleasedDuringBuffer = true;
            }

            // variable jump height
            if (_isJumping && VerticalVelocity > 0f)
            {
                if (_isPastApexThreshold)
                {
                    _isPastApexThreshold = false;
                    _isFastFalling = true;
                    _fastFallTime = MoveStats.TimeForUpwardsCancel;
                    VerticalVelocity = 0f;
                }
                else
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        // 3. Ground Jump (with Coyote Time)
        if (_jumpBufferTimer > 0f && !_isJumping && (_isGrounded || _coyoteTimer > 0f))
        {
            InitiateJump(1);

            if (_jumpReleasedDuringBuffer)
            {
                _isFastFalling = true;
                _fastFallReleaseSpeed = VerticalVelocity;
            }
        }
        // 4. Air Jump
        else if (_jumpBufferTimer > 0f && _isJumping && _numberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed)
        {
            _isFastFalling = false;
            InitiateJump(1);
        }
        // 5. Air Jump (after falling off ledge)
        else if (_jumpBufferTimer > 0f && _isFalling && _numberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed)
        {
            InitiateJump(1);
            _isFastFalling = false;
        }

        // 6. Landing
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

    private void InitiateJump(int numberOfJumpsUsed)
    {
        AudioManager.Instance.PlaySFX("Player Jump");
        if (!_isJumping)
        {
            _isJumping = true;
        }
        _jumpBufferTimer = 0f;
        _coyoteTimer = 0f; 
        _numberOfJumpsUsed += numberOfJumpsUsed;
        VerticalVelocity = MoveStats.InitialJumpVelocity;
    }

    private void Jump()
    {
        if (_isJumping)
        {
            if (_bumpedHead)
            {
                _isFastFalling = true;
            }

            if (VerticalVelocity >= 0f)
            {
                _apexPoint = Mathf.InverseLerp(MoveStats.InitialJumpVelocity, 0f, VerticalVelocity);

                if (_apexPoint > MoveStats.ApexThreshold)
                {
                    if (!_isPastApexThreshold)
                    {
                        _isPastApexThreshold = true;
                        _timePastApexThreshold = 0f;
                    }

                    if (_isPastApexThreshold)
                    {
                        _timePastApexThreshold += Time.fixedDeltaTime;
                        if (_timePastApexThreshold < MoveStats.ApexHangTime)
                        {
                            VerticalVelocity = 0f; 
                        }
                        else
                        {
                            VerticalVelocity = -0.01f; 
                        }
                    }
                }
                else
                {
                    VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
                    if (_isPastApexThreshold)
                    {
                        _isPastApexThreshold = false;
                    }
                }
            }
            else if (!_isFastFalling)
            {
                VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime; 
            }
            else
            {
                if (!_isFalling)
                {
                    _isFalling = true;
                }
            }
        }

        // --- Fast Fall Logic (when jump is released) ---
        if (_isFastFalling)
        {
            if (_fastFallTime >= MoveStats.TimeForUpwardsCancel)
            {
                // Gravity for releasing jump (faster)
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (_fastFallTime < MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f, (_fastFallTime / MoveStats.TimeForUpwardsCancel));
            }

            _fastFallTime += Time.fixedDeltaTime;
        }

        // --- Standard Fall (not jumping, just walked off ledge) ---
        if (!_isGrounded && !_isJumping)
        {
            if (!_isFalling)
            {
                _isFalling = true;
            }
            VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
        }
        
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MoveStats.MaxFallSpeed, 50f);
    }
    
    #endregion
    
    
    #region Wall Movement
    
    private void HandleWallSlide()
    {
        if (_wallJumpCooldownTimer > 0f) return;
        
        bool isPushingIntoWall = (_isFacingRight && InputManager.Movement.x > 0.1f) || (!_isFacingRight && InputManager.Movement.x < -0.1f);
        bool canWallSlide = _isWallDetected && !_isGrounded && isPushingIntoWall && VerticalVelocity < 0f; 

        if (canWallSlide)
        {
            if (!_isWallSliding) 
            {
                _isWallSliding = true;
                _isJumping = false; 
                _isFalling = false;
                _isFastFalling = false;
                _numberOfJumpsUsed = 0; 
                _wallStickTimer = MoveStats.WallStickTime; 
            }

            if (_wallStickTimer > 0)
            {
                VerticalVelocity = 0;
            }
            else
            {
                VerticalVelocity = -MoveStats.WallSlideSpeed;
            }
        }
        else
        {
            if (_isWallSliding)
            {
                _isWallSliding = false;
                _wallStickTimer = 0; 
            }
        }
    }

    private void WallJump()
    {
        _isWallSliding = false;
        _isJumping = true;
        _isFalling = false;
        _isFastFalling = false;
        _numberOfJumpsUsed = 1; 
        _jumpBufferTimer = 0f;
        _coyoteTimer = 0f;
        _wallStickTimer = 0f;
        _wallJumpCooldownTimer = MoveStats.WallJumpCooldown;
        
        Turn(!_isFacingRight);

        float forceX = MoveStats.WallJumpForce.x * (_isFacingRight ? 1 : -1);
        float forceY = MoveStats.WallJumpForce.y;
        
        ApplyKnockback(new Vector2(forceX, forceY), MoveStats.WallJumpKnockbackDuration);
    }

	#endregion

	#region Dash

	private void DashChecks()
	{
		if (_dashWasPressed && _dashCooldownTimer <= 0f)
		{
			InitiateDash();
		}
		_dashWasPressed = false; 
	}

	private void InitiateDash()
	{
        AudioManager.Instance.PlaySFX("Player Dash");
        _isDashing = true;
		_dashTimer = MoveStats.DashDuration;
		_dashCooldownTimer = MoveStats.DashCooldown;
		_moveVelocity.x = MoveStats.DashSpeed * (_isFacingRight ? 1 : -1);
		VerticalVelocity = 0f;
		_isJumping = false;
		_isFalling = false;
		_isWallSliding = false;
		_isKnockedBack = false;
		_isFastFalling = false;
	}

    #endregion

    public void PlayFootstepSFX()
    {
        AudioManager.Instance.PlaySFX("Player Footstep");
    }

    #region Collision & Timers

    private void CollisionChecks()
	{
		IsGrounded();
		BumpedHead();
		WallCheck();
	}
    
    private void IsGrounded()
    {
        Vector2 boxcastOrigin = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x, MoveStats.GroundDetectionRayLength);

        _groundHit = Physics2D.BoxCast(boxcastOrigin, boxCastSize, 0f, Vector2.down, MoveStats.GroundDetectionRayLength, MoveStats.GroundLayer);
        _isGrounded = _groundHit.collider != null;
    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _bodyColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x * MoveStats.HeadWidth, MoveStats.HeadDetectionRayLength);

        _headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, MoveStats.HeadDetectionRayLength, MoveStats.GroundLayer);
        _bumpedHead = _headHit.collider != null;
    }
    
    private void WallCheck()
    {
        Vector2 boxCastDirection = _isFacingRight ? Vector2.right : Vector2.left;
        Vector2 boxCastOrigin = _bodyColl.bounds.center;
        Vector2 boxCastSize = new Vector2(_bodyColl.bounds.size.x, _bodyColl.bounds.size.y * 0.9f); 

        _wallHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, boxCastDirection, MoveStats.WallDetectionRayLength, MoveStats.WallLayer);
        _isWallDetected = _wallHit.collider != null;
    }
    
    private void CountTimers()
    {
        _jumpBufferTimer -= Time.deltaTime;
        _wallStickTimer -= Time.deltaTime;
		_wallJumpCooldownTimer -= Time.deltaTime;
		_dashCooldownTimer -= Time.deltaTime;

		if (_dashTimer > 0) 
		{
			_dashTimer -= Time.deltaTime;
			if (_dashTimer <= 0) 
			{
				_isDashing = false;
				_isFalling = true; 
			}
		}

        if (!_isGrounded)
        {
            _coyoteTimer -= Time.deltaTime;
        }
        else
        {
            _coyoteTimer = MoveStats.JumpCoyoteTime;
        }

        if (_knockbackTimer > 0)
        {
            _knockbackTimer -= Time.deltaTime;
        }
        else if (_isKnockedBack)
        {
            _isKnockedBack = false;
        }
    }
    
    #endregion

    
    #region Knockback
    public void ApplyKnockback(Vector2 knockbackVelocity, float duration)
    {
        _isKnockedBack = true;
        _knockbackTimer = duration;
        _moveVelocity.x = knockbackVelocity.x;
        VerticalVelocity = knockbackVelocity.y;
    }
    
    #endregion
    
    
    #region Gizmos
    
    private void DrawDebugRays()
    {
        if (ShowFacingDirectionRay)
        {
            Vector3 direction = _isFacingRight ? transform.right : -transform.right;
            Debug.DrawRay(transform.position, direction * FacingDirectionGizmoLength, Color.magenta);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 direction = _isFacingRight ? transform.right : -transform.right;
        Gizmos.DrawRay(transform.position, direction * FacingDirectionGizmoLength);
    }
    #endregion
}