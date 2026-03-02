using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    public PlayerData playerData;
    private PlayerGroundDetection groundDetection;

    private Rigidbody2D rb;
    private float moveInput;

    [SerializeField] Transform visual;

  
    private CapsuleCollider2D playerCollider;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    private float jumpBufferCounter;
    private float coyoteCounter;


    private bool isOnSlope;
    private Vector2 slopeNormal;
    private Vector2 slopeDirection;

    private bool isJumping;
    private float jumpStickDisableTime = 0.15f;
    private float jumpEndTime;

    
    private bool isSliding;
    private bool slideInputHeld;
    private float slideEndTime;
    private float slideCooldownTime;
    [SerializeField] private float slideDuration = 0.5f; // Duración máxima del deslizamiento
    [SerializeField] private float slideCooldown = 0.3f; // Cooldown entre deslizamientos

    
    private Vector2 debugForces;

    [SerializeField] public Animator playerAnim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundDetection = GetComponent<PlayerGroundDetection>();
        playerCollider = GetComponent<CapsuleCollider2D>();

       
        if (playerCollider != null)
        {
            originalColliderSize = playerCollider.size;
            originalColliderOffset = playerCollider.offset;
        }

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

       
        if (Input.GetKeyDown(KeyCode.Z))
        {
            jumpBufferCounter = playerData.jumpBufferTime;
        }

        if (Input.GetKeyUp(KeyCode.Z) && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * playerData.jumpCutMultiplier
            );
        }

     
        //slideInputHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        //if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        //{
        //    TryStartSlide();
        //}

        //if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        //{
        //    if (isSliding)
        //    {
        //        EndSlide();
        //    }
        //}


        jumpBufferCounter -= Time.deltaTime;

        if (slideCooldownTime > 0)
        {
            slideCooldownTime -= Time.deltaTime;
        }

        if (isSliding && Time.time > slideEndTime)
        {
            EndSlide();
        }

        if (isJumping && Time.time > jumpEndTime)
        {
            isJumping = false;
        }
    }

    private void FixedUpdate()
    {
        UpdateSlopeState();

        if (groundDetection.IsGrounded)
        {
            coyoteCounter = playerData.coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.fixedDeltaTime;
        }

        ApplyMovement();
        HandleJump();
        ApplyAdaptiveGravity();
    }

    private void UpdateSlopeState()
    {
        if (groundDetection.IsGrounded && groundDetection.GroundAngle > 5f)
        {
            isOnSlope = true;
            slopeNormal = groundDetection.GroundNormal;
            slopeDirection = new Vector2(slopeNormal.y, -slopeNormal.x).normalized;

            if (slopeDirection.x < 0)
                slopeDirection = -slopeDirection;
        }
        else
        {
            isOnSlope = false;
        }
    }

    private void ApplyMovement()
    {
        float targetSpeed = moveInput * playerData.maxSpeed;

        
        if (isSliding)
        {
            targetSpeed *= playerData.slideSpeedMultiplier; 
        }

        float speedDifference = targetSpeed - rb.linearVelocity.x;

        
        float slopeBonus = (isOnSlope && isSliding) ? playerData.slideSlopeBoost : 1f;

        float currentAccel = groundDetection.IsGrounded
            ? playerData.acceleration * slopeBonus
            : playerData.acceleration * playerData.airControl;

        float movement = speedDifference * currentAccel;
        rb.AddForce(Vector2.right * movement, ForceMode2D.Force);
    }

    private void ApplyAdaptiveGravity()
    {
        rb.AddForce(Physics2D.gravity * playerData.gravityScale, ForceMode2D.Force);

        if (isOnSlope && !isJumping)
        {
            Vector2 stickForce = -slopeNormal * playerData.slopeStickForce;
            rb.AddForce(stickForce, ForceMode2D.Force);
        }
    }

    private void HandleJump()
    {
        
        if (jumpBufferCounter > 0 && coyoteCounter > 0 && !isSliding)
        {
            PerformJump();
            jumpBufferCounter = 0;
            coyoteCounter = 0;
        }
    }

    private void PerformJump()
    {
        isJumping = true;
        jumpEndTime = Time.time + jumpStickDisableTime;

        rb.linearVelocity = new Vector2(0f, 0f);
        rb.AddForce(Vector2.up * playerData.jumpForce, ForceMode2D.Impulse);
    }

    
    private void TryStartSlide()
    {
        
        if (groundDetection.IsGrounded && !isSliding && slideCooldownTime <= 0)
        {
            StartSlide();
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        slideEndTime = Time.time + slideDuration;

       
        if (playerCollider != null)
        {
            playerCollider.size = new Vector2(
                originalColliderSize.x,
                originalColliderSize.y * 0.1f
            );

            playerCollider.offset = new Vector2(
                originalColliderOffset.x,
                originalColliderOffset.y - (originalColliderSize.y * 0.25f)
            );
        }

       
        if (playerAnim != null)
        {
            playerAnim.SetBool("isSliding", true);
        }
    }

    private void EndSlide()
    {
        isSliding = false;
        slideCooldownTime = slideCooldown;

       
        if (playerCollider != null)
        {
            playerCollider.size = originalColliderSize;
            playerCollider.offset = originalColliderOffset;
        }

        
        if (playerAnim != null)
        {
            playerAnim.SetBool("isSliding", false);
        }
    }

    //private void OnDrawGizmos()
    //{
    //    if (!Application.isPlaying) return;

    //    if (isOnSlope)
    //    {
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawRay(transform.position, slopeNormal);
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawRay(transform.position, slopeDirection);
    //    }

    //    if (isJumping)
    //    {
    //        Gizmos.color = Color.cyan;
    //        Gizmos.DrawWireSphere(transform.position, 0.4f);
    //    }

        
    //    if (isSliding)
    //    {
    //        Gizmos.color = Color.magenta;
    //        Gizmos.DrawWireCube(transform.position, playerCollider != null ? playerCollider.size : Vector2.one);
    //    }

    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawRay(transform.position, debugForces * 0.05f);
    //}
}
