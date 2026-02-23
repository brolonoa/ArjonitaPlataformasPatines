using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    public PlayerData playerData;
    private PlayerGroundDetection groundDetection;

    private Rigidbody2D rb;
    private float moveInput;

    float targetSpeed;
    float speedDifference;
    float accelerationRate;

    [SerializeField] Transform visual;

    [SerializeField] float rotationSmooth = 10f;
    [SerializeField] float maxSlopeRotation = 45f;

    float targetRotation;
    float currentRotation;
    float jumpBufferCounter;
    float coyoteCounter;
    bool jumpPressed;
    float currentSoftCap;
    bool isCrouching;
    public float CurrentSpeed => Mathf.Abs(rb.linearVelocity.x);
    public bool IsCrouching => isCrouching;


    public float CurrentMaxSpeed => currentSoftCap;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundDetection = GetComponent<PlayerGroundDetection>();


        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; 
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; 


    }
    private bool wasCrouching;
    private bool hasUsedCrouchBoost;

    private void Update()
    {
        // Detectar inicio de crouch
        wasCrouching = isCrouching;
        isCrouching = Input.GetKey(KeyCode.LeftShift);

        // Boost al empezar a agacharse
        if (isCrouching && !wasCrouching && groundDetection.IsGrounded)
        {
            float boostDirection = moveInput != 0 ? Mathf.Sign(moveInput) : Mathf.Sign(rb.linearVelocity.x);
            if (boostDirection == 0) boostDirection = 1f;

            rb.AddForce(Vector2.right * boostDirection * playerData.crouchBoostForce, ForceMode2D.Impulse);
            hasUsedCrouchBoost = true;
        }

        if (!isCrouching && wasCrouching)
        {
            hasUsedCrouchBoost = false;
        }

        moveInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Z)) 
        {
            jumpBufferCounter = playerData.jumpBufferTime;
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    rb.linearVelocity.y * playerData.jumpCutMultiplier
                );
            }
        }
    }
    private void FixedUpdate()
    {
        ApplyMovement();
        UpdateSoftCap();
        ApplySoftCap();
        ApplyFriction();
        HandleSlopeMovement();
        HandleJump();
        ApplyGravity();
    }
    void LateUpdate()
    {
        targetRotation = GetSlopeRotation();

        currentRotation = Mathf.Lerp(
            currentRotation,
            targetRotation,
            rotationSmooth * Time.deltaTime
        );

        visual.localRotation = Quaternion.Euler(0f, 0f, -currentRotation);
    }
    void ApplyMovement()
    {
        if (Mathf.Approximately(moveInput, 0f))
            return;

        targetSpeed = moveInput * playerData.maxSpeed;
        speedDifference = targetSpeed - rb.linearVelocity.x;

        accelerationRate = groundDetection.IsGrounded
            ? playerData.acceleration
            : playerData.acceleration * playerData.airControl;

        // Reducir aceleración si está agachado
        if (isCrouching && groundDetection.IsGrounded)
        {
            accelerationRate *= playerData.crouchAccelerationMultiplier;
        }

        float movement = speedDifference * accelerationRate;
        rb.AddForce(Vector2.right * movement, ForceMode2D.Force);
    }

    void ApplyFriction()
    {
        if (!groundDetection.IsGrounded)
            return;

        float velocityX = rb.linearVelocity.x;
        float speed = Mathf.Abs(velocityX);

        // Si excede soft cap, ApplySoftCap se encarga
        if (speed > currentSoftCap)
            return;

        if (Mathf.Abs(moveInput) < 0.1f)
        {
            velocityX = Mathf.MoveTowards(
                velocityX,
                0f,
                playerData.idleFriction * Time.fixedDeltaTime
            );
        }
        else if (Mathf.Sign(moveInput) != Mathf.Sign(velocityX))
        {
            velocityX = Mathf.MoveTowards(
                velocityX,
                0f,
                playerData.turnFriction * Time.fixedDeltaTime
            );
        }

        rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y);
    }

    float GetSlopeRotation()
    {
        if (!groundDetection.IsGrounded)
            return 0f;

        if (groundDetection.GroundAngle < 5f)
            return 0f;

        Vector2 normal = groundDetection.GroundNormal;

        // Convertimos normal → ángulo visual
        float angle = Mathf.Atan2(normal.x, normal.y) * Mathf.Rad2Deg;

        return Mathf.Clamp(angle, -maxSlopeRotation, maxSlopeRotation);
    }
    void HandleSlopeMovement()
    {
        if (!groundDetection.IsGrounded)
            return;

        if (groundDetection.GroundAngle < 5f)
            return;

        // NO aplicar si está saltando
        if (rb.linearVelocity.y > 0.1f)
            return;

        Vector2 normal = groundDetection.GroundNormal;
        Vector2 tangent = new Vector2(normal.y, -normal.x).normalized;
        float speedAlongSlope = Vector2.Dot(rb.linearVelocity, tangent);

        Vector2 gravity = Physics2D.gravity * playerData.gravityScale;
        float gravityAlongSlope = Vector2.Dot(gravity, tangent);

        speedAlongSlope += gravityAlongSlope * Time.fixedDeltaTime;

        rb.linearVelocity = tangent * speedAlongSlope;
    }

    void ApplyGravity()
    {
        // Solo skip si está en rampa Y tocando suelo Y no saltando
        if (groundDetection.IsGrounded
            && groundDetection.GroundAngle > 5f
            && rb.linearVelocity.y <= 0.1f)
            return;

        Vector2 gravity = Physics2D.gravity * playerData.gravityScale;
        rb.AddForce(gravity, ForceMode2D.Force);
    }
    void HandleJump()
    {
        if (groundDetection.IsGrounded)
            coyoteCounter = playerData.coyoteTime;
        else
            coyoteCounter -= Time.fixedDeltaTime;

        jumpBufferCounter -= Time.fixedDeltaTime;

        if (jumpBufferCounter > 0 && coyoteCounter > 0)
        {
            Jump();
            jumpBufferCounter = 0;
        }
    }
    void Jump()
    {
        Vector2 normal = groundDetection.IsGrounded
            ? groundDetection.GroundNormal
            : Vector2.up;

        // Saltar en dirección de la normal del suelo
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        rb.AddForce(normal * playerData.jumpForce, ForceMode2D.Impulse);
    }
    bool IsOnDownSlope()
    {
        if (!groundDetection.IsGrounded)
            return false;

        if (groundDetection.GroundAngle < 5f)
            return false;

        Vector2 normal = groundDetection.GroundNormal;
        Vector2 tangent = new Vector2(normal.y, -normal.x).normalized;

        float gravityAlongSlope = Vector2.Dot(
            Physics2D.gravity,
            tangent
        );

        return gravityAlongSlope > 0;
    }
    void UpdateSoftCap()
    {
        currentSoftCap = playerData.baseMaxSpeed;

        bool onDownSlope = IsOnDownSlope();

        if (onDownSlope)
        {
            currentSoftCap += playerData.slopeSpeedBonus;

            if (isCrouching)
                currentSoftCap += playerData.crouchSlopeBonus;
        }
        else
        {
            if (isCrouching)
                currentSoftCap += playerData.crouchFlatBonus; // >= 3
        }
    }
    void ApplySoftCap()
    {
        float speed = rb.linearVelocity.magnitude;

        if (speed <= currentSoftCap)
            return;

        float decay = playerData.momentumDecayRate * Time.fixedDeltaTime;

        float newSpeed = Mathf.MoveTowards(
            speed,
            currentSoftCap,
            decay
        );

        rb.linearVelocity = rb.linearVelocity.normalized * newSpeed;
    }
    
}
