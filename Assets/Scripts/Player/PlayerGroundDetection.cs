using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGroundDetection : MonoBehaviour
{
    [Header("Detección de Suelo")]
    [SerializeField] private float groundCheckRadius = 0.5f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int maxGroundHits = 5;

    [Header("Visualización")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color groundedColor = Color.green;
    [SerializeField] private Color airColor = Color.red;
    [SerializeField] private Color normalColor = Color.blue;
    [SerializeField] private Color castColor = Color.yellow;

    private Animator playerAnim;
    private PlayerController playerController;

    // Resultados de la detección
    private bool isGrounded;
    private Vector2 groundNormal;
    private float groundAngle;
    private Collider2D groundCollider;
    private Vector2 groundPoint;
    private RaycastHit2D[] groundHits;
    private ContactFilter2D contactFilter;

    // Propiedades públicas
    public bool IsGrounded => isGrounded;
    public Vector2 GroundNormal => groundNormal;
    public float GroundAngle => groundAngle;
    public Collider2D GroundCollider => groundCollider;
    public Vector2 GroundPoint => groundPoint;

    private void Awake()
    {
        groundHits = new RaycastHit2D[maxGroundHits];

        // Configurar ContactFilter
        contactFilter = new ContactFilter2D();
        contactFilter.layerMask = groundLayer;
        contactFilter.useLayerMask = true;
        contactFilter.useTriggers = false;
    }

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerAnim = playerController.playerAnim;
    }
    private void Update()
    {
        if(!isGrounded)
        {
            playerAnim.SetBool("isOnAir", true);
        }
       else playerAnim.SetBool("isOnAir", false);
    }
    private void FixedUpdate()
    {
        DetectGround();
    }

    private void DetectGround()
    {
        // Posición inicial del spherecast (desde el centro del personaje)
        Vector2 origin = transform.position;

        // Dirección de detección (siempre hacia abajo en world space)
        Vector2 direction = Vector2.down;

        // CORRECCIÓN: Usar ContactFilter configurado
        int hitCount = Physics2D.CircleCast(
            origin,
            groundCheckRadius,
            direction,
            contactFilter,
            groundHits,
            groundCheckDistance
        );

        // Resetear estado
        isGrounded = false;
        groundNormal = Vector2.up;
        groundAngle = 0f;
        groundCollider = null;

        // Procesar hits
        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit2D hit = groundHits[i];

            // Ignorar el collider del propio jugador
            if (hit.collider.gameObject == gameObject)
                continue;

            // Este es el suelo válido
            isGrounded = true;
            groundNormal = hit.normal;
            groundCollider = hit.collider;
            groundPoint = hit.point;

            // Calcular el ángulo del suelo (0 = horizontal, 90 = vertical)
            groundAngle = Vector2.Angle(Vector2.up, groundNormal);

            // Salir del bucle, nos quedamos con el primer hit válido
            break;
        }
    }

    // Método para verificar si está en una rampa pronunciada
    public bool IsOnSteepSlope(float maxWalkableAngle = 60f)
    {
        return isGrounded && groundAngle > maxWalkableAngle;
    }

    // Método para obtener la dirección de la pendiente (para movimiento a lo largo de la rampa)
    public Vector2 GetSlopeDirection()
    {
        if (!isGrounded || groundAngle < 5f)
            return Vector2.right;

        // Calcular la dirección a lo largo de la pendiente
        // Perpendicular a la normal, apuntando en dirección horizontal
        Vector2 slopeDir = new Vector2(groundNormal.y, -groundNormal.x).normalized;

        // Asegurar que la dirección apunte en la dirección correcta (derecha/izquierda)
        if (slopeDir.x < 0)
            slopeDir = -slopeDir;

        return slopeDir;
    }

    // Método para verificar si la pendiente es descendente en la dirección actual
    public bool IsDownSlope(Vector2 moveDirection)
    {
        if (!isGrounded || groundAngle < 5f)
            return false;

        Vector2 slopeDir = GetSlopeDirection();

        // Si la dirección de movimiento tiene el mismo signo que la pendiente
        return Mathf.Sign(moveDirection.x) == Mathf.Sign(slopeDir.x);
    }

    // Método para verificar si hay suelo delante
    public bool HasGroundAhead(float distance = 0.5f)
    {
        float directionX = Mathf.Sign(transform.localScale.x);
        Vector2 origin = (Vector2)transform.position + Vector2.right * directionX * 0.3f;
        Vector2 direction = Vector2.down;

        RaycastHit2D hit = Physics2D.CircleCast(
            origin,
            groundCheckRadius * 0.7f,
            direction,
            distance,
            groundLayer
        );

        return hit.collider != null && !hit.collider.isTrigger;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos)
            return;

        Vector2 origin = Application.isPlaying ? (Vector2)transform.position : (Vector2)transform.position;
        Vector2 endPoint = origin + Vector2.down * groundCheckDistance;

        // Color del cast según si está grounded o no
        Gizmos.color = isGrounded ? groundedColor : castColor;

        // Dibujar el círculo en el ORIGEN (centro del personaje)
        Gizmos.DrawWireSphere(origin, groundCheckRadius);

        // Dibujar el círculo en el DESTINO (final del raycast)
        // ESTE es el círculo que realmente detecta el suelo
        Gizmos.color = isGrounded ? groundedColor : airColor;
        Gizmos.DrawWireSphere(endPoint, groundCheckRadius);

        // Dibujar líneas que conectan ambos círculos para visualizar el "volumen" del cast
        Gizmos.color = castColor;
        Gizmos.DrawLine(origin + Vector2.left * groundCheckRadius, endPoint + Vector2.left * groundCheckRadius);
        Gizmos.DrawLine(origin + Vector2.right * groundCheckRadius, endPoint + Vector2.right * groundCheckRadius);

        // Dibujar la distancia de detección como línea central
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origin, Vector3.down * groundCheckDistance);

        // Dibujar información del suelo si está grounded
        if (isGrounded)
        {
            Gizmos.color = normalColor;
            Gizmos.DrawRay(groundPoint, groundNormal * 0.5f);

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(groundPoint, 0.05f);

            Gizmos.color = Color.magenta;
            Vector2 slopeDir = GetSlopeDirection();
            Gizmos.DrawRay(groundPoint, slopeDir * 0.5f);
        }
    }
}