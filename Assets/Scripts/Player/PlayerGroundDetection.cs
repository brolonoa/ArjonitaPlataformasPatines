using UnityEngine;

public class PlayerGroundDetection : MonoBehaviour
{
   private PlayerController controller;
   private PlayerData playerData;

    public bool IsGrounded { get; private set; }
    public float LastGroundedTime { get; private set; }
    private bool wasGrounded;
    public Vector2 GroundNormal { get; private set; }
    public float GroundAngle { get; private set; }
    [SerializeField] LayerMask goroundLayer;
    private void Start()
    {
        controller = GetComponent<PlayerController>();
        playerData = controller.playerData;
    }

    void Update()
    {
        CheckGround();
        Debug.Log(IsGrounded);
    }

    private void CheckGround()
    {
        wasGrounded = IsGrounded;

        Vector2 rayOrigin = (Vector2)transform.position + playerData.groundCheckOffset;
        RaycastHit2D hit = Physics2D.Raycast(
            rayOrigin,
            Vector2.down,
            playerData.groundCheckDistance,
            goroundLayer
        );

        IsGrounded = hit.collider != null;

        if (IsGrounded)
        {
            LastGroundedTime = Time.time;
            GroundNormal = hit.normal;
            GroundAngle = Vector2.Angle(GroundNormal, Vector2.up);
        }

        if (IsGrounded && !wasGrounded)
        {
            // Land
        }
        else if (!IsGrounded && wasGrounded)
        {
            // Leave ground
        }
    }
    void OnDrawGizmos()
    {
        if (playerData == null) return;

        Vector2 rayOrigin = (Vector2)transform.position + playerData.groundCheckOffset;
        Gizmos.color = IsGrounded ? Color.green : Color.red;

        // Vector2.down en vez de Vector3.down
        Gizmos.DrawRay(rayOrigin, Vector2.down * playerData.groundCheckDistance);
        Gizmos.DrawWireSphere(rayOrigin, 0.1f);
    }
}
