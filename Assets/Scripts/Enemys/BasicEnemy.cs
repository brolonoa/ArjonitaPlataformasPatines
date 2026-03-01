using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float reachDistance = 0.1f;
    [SerializeField] private float waitTime = 1.5f;

    [Header("Vision")]
    [SerializeField] private float visionRange = 5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Attack")]
    [SerializeField] private float aimTime = 1f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Visual")]
    [SerializeField] private Transform visual;
    [SerializeField] private LineRenderer lineRenderer;

    private int currentIndex = 0;
    private float waitCounter;
    private bool waiting;

    private Transform player;
    private float aimCounter;
    private bool aiming;

    void Update()
    {
        CheckForPlayer();

        if (player != null)
            HandleAttack();
        else
            Patrol();
    }

    // =========================
    // PATROL (SOLO EJE X)
    // =========================
    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        if (waiting)
        {
            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0f)
            {
                waiting = false;
                currentIndex++;
                if (currentIndex >= patrolPoints.Length)
                    currentIndex = 0;
            }
            return;
        }

        Transform targetPoint = patrolPoints[currentIndex];

        Vector3 targetPos = new Vector3(
            targetPoint.position.x,
            transform.position.y, // 🔒 bloqueamos Y
            transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        float dirX = targetPos.x - transform.position.x;
        Flip(dirX);

        if (Mathf.Abs(transform.position.x - targetPos.x) <= reachDistance)
        {
            waiting = true;
            waitCounter = waitTime;
        }
    }

    // =========================
    // DETECCIÓN
    // =========================
    void CheckForPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, visionRange, playerLayer);

        if (hit != null)
            player = hit.transform;
        else
        {
            player = null;
            aiming = false;
            lineRenderer.enabled = false;
        }
    }

    // =========================
    // APUNTAR + DISPARAR
    // =========================
    void HandleAttack()
    {
        Vector2 direction = (player.position - firePoint.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(
            firePoint.position,
            direction,
            visionRange,
            playerLayer | obstacleLayer
        );

        if (hit.collider != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, hit.point);

            if (hit.collider.CompareTag("Player"))
            {
                aiming = true;
                aimCounter += Time.deltaTime;

                if (aimCounter >= aimTime)
                {
                    Shoot(direction);
                    aimCounter = 0f;
                }
            }
            else
            {
                aiming = false;
                aimCounter = 0f;
            }
        }

        Flip(direction.x);
    }

    void Shoot(Vector2 dir)
    {
        Instantiate(projectilePrefab, firePoint.position, Quaternion.identity)
            .GetComponent<Rigidbody2D>()
            .linearVelocity = dir * 10f;
    }

    // =========================
    void Flip(float dirX)
    {
        if (visual == null) return;

        if (dirX > 0)
            visual.localScale = new Vector3(1, 1, 1);
        else if (dirX < 0)
            visual.localScale = new Vector3(-1, 1, 1);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}
