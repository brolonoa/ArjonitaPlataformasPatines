using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    public PlayerData playerData;
    private PlayerGroundDetection groundDetection;

    private Rigidbody2D rb;
    private float moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundDetection = GetComponent<PlayerGroundDetection>();


        rb.gravityScale = 0f;
        //rb.freezeRotation = true; 
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; 
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; 


    }
    private void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
    }
    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        rb.AddForce(Vector2.right * moveInput  /*currentAcceleration*/, ForceMode2D.Force);
    }

}
