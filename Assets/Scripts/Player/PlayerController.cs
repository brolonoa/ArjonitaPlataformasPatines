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
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundDetection = GetComponent<PlayerGroundDetection>();


        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; 
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; 


    }
    private void Update()
    {
      moveInput = Input.GetAxis("Horizontal");
    }
    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyFriction();
        HandleSlopeMovement();
        ApplyGravity();
    }
    void ApplyMovement()
    {
        if (Mathf.Approximately(moveInput, 0f))
            return;

        // Velocidad objetivo
        targetSpeed = moveInput * playerData.maxSpeed;

        // Diferencia entre la velocidad actual y la deseada
        speedDifference = targetSpeed - rb.linearVelocity.x;

        // Aceleración distinta en suelo / aire
        accelerationRate = groundDetection.IsGrounded
            ? playerData.acceleration
            : playerData.acceleration * playerData.airControl;

        // Movimiento suavizado
        float movement = speedDifference * accelerationRate;

        rb.AddForce(Vector2.right * movement, ForceMode2D.Force);
    }

    void ApplyFriction()
    {
        if (!groundDetection.IsGrounded)
            return;

        float velocityX = rb.linearVelocity.x;

        // Si no hay input, frenar progresivamente
        if (Mathf.Abs(moveInput) < 0.1f)
        {
            velocityX = Mathf.MoveTowards(
                velocityX,
                0f,
                playerData.idleFriction * Time.fixedDeltaTime
            );
        }
        // Si hay input contrario, fricción de giro
        else if (Mathf.Sign(moveInput) != Mathf.Sign(velocityX))
        {
            velocityX = Mathf.MoveTowards( velocityX, 0f, playerData.turnFriction * Time.fixedDeltaTime);
        }

        rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y);
    }
    Vector2 GetSlopeTangent()
    {
        Vector2 normal = groundDetection.GroundNormal;
        Vector2 tangent = Vector2.Perpendicular(normal);

        // Asegurar que apunta “hacia delante”
        if (tangent.x * moveInput < 0)
            tangent = -tangent;

        return tangent.normalized;
    }
    void HandleSlopeMovement()
    {
        if (!groundDetection.IsGrounded)
            return;

        // Si no es una rampa, salir
        if (groundDetection.GroundAngle < 5f)
            return;

        Vector2 tangent = GetSlopeTangent();

        // Velocidad actual proyectada en la rampa
        float speedAlongSlope = Vector2.Dot(rb.linearVelocity, tangent);

        // Gravedad proyectada en la rampa
        float gravityForce = Physics2D.gravity.y * playerData.gravityScale;
        float slopeGravity = -gravityForce * Mathf.Sin(
            groundDetection.GroundAngle * Mathf.Deg2Rad
        );

        speedAlongSlope += slopeGravity * Time.fixedDeltaTime;

        rb.linearVelocity = tangent * speedAlongSlope;
    }
    void ClampVelocity()
    {
        Vector2 velocity = rb.linearVelocity;

        if (Mathf.Abs(velocity.x) > playerData.maxSpeed)
        {
            velocity.x = Mathf.Sign(velocity.x) * playerData.maxSpeed;
            rb.linearVelocity = velocity;
        }
    }
    void ApplyGravity()
    {
        if (groundDetection.IsGrounded && groundDetection.GroundAngle > 5f)
            return; // la gravedad ya se aplica en la rampa

        Vector2 gravity = Physics2D.gravity * playerData.gravityScale;
        rb.AddForce(gravity, ForceMode2D.Force);
    }
    //void CheckSpeedChange()
    //{
    //    float currentSpeed = CurrentSpeed;

    //    if (!Mathf.Approximately(currentSpeed, previousSpeed))
    //    {
    //        OnSpeedChanged?.Invoke(currentSpeed);
    //        previousSpeed = currentSpeed;
    //    }
    //}
}
