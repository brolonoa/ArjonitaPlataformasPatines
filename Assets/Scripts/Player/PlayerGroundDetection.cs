using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGroundDetection : MonoBehaviour
{
    private PlayerController controller;
    private PlayerData playerData;

    public bool IsGrounded { get; private set; }
    public float LastGroundedTime { get; private set; }

    public Vector2 GroundNormal { get; private set; }
    public float GroundAngle { get; private set; }

    private bool wasGrounded;

    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        playerData = controller.playerData;
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void CheckGround()
    {
        wasGrounded = IsGrounded;

        Vector2 origin = (Vector2)transform.position + playerData.groundCheckOffset;

        RaycastHit2D hit = Physics2D.CircleCast(origin, playerData.groundCheckRadius, Vector2.down, playerData.groundCheckDistance, groundLayer);

        if (hit.collider != null)
        {
            Vector2 normal = hit.normal;
            float angle = Vector2.Angle(normal, Vector2.up);

            // 🔹 FILTRO DE ÁNGULO (evita que paredes cuenten como suelo)
            if (angle <= playerData.maxGroundAngle)
            {
                IsGrounded = true;
                GroundNormal = normal;
                GroundAngle = angle;
                LastGroundedTime = Time.time;
            }
            else
            {
                IsGrounded = false;
            }
        }
        else
        {
            IsGrounded = false;
        }

        // Eventos opcionales
        if (IsGrounded && !wasGrounded)
        {
            OnLand();
            Debug.Log("Suelo");
        }
        else if (!IsGrounded && wasGrounded)
        {
            OnLeaveGround();
            Debug.Log("NoSuelo");
        }
    }

    private void OnLand()
    {
        // Aquí puedes disparar animación o evento
    }

    private void OnLeaveGround()
    {
        // Aquí puedes iniciar lógica de caída
    }

    private void OnDrawGizmos()
    {
        if (playerData == null) return;

        Vector2 origin = (Vector2)transform.position + playerData.groundCheckOffset;

        Gizmos.color = IsGrounded ? Color.green : Color.red;

        Gizmos.DrawWireSphere(origin, playerData.groundCheckRadius);
        Gizmos.DrawRay(origin, Vector2.down * playerData.groundCheckDistance);
    }
}