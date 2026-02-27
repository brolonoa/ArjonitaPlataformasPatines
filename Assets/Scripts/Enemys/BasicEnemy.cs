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

    [Header("Visual")]
    [SerializeField] private Transform visual;

    private int currentIndex = 0;
    private float waitCounter;
    private bool waiting;
    private bool attacking;

    private Transform player;

    void Update()
    {
        CheckForPlayer();

        if (attacking)
            Attack();
        else
            Patrol();
    }

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
                    currentIndex = 0; // 🔁 bucle infinito
            }
            return;
        }

        Transform targetPoint = patrolPoints[currentIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            speed * Time.deltaTime
        );

        float dirX = targetPoint.position.x - transform.position.x;
        Flip(dirX);

        if (Vector3.Distance(transform.position, targetPoint.position) <= reachDistance)
        {
            waiting = true;
            waitCounter = waitTime;
        }
    }

    void Attack()
    {
        if (player == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            speed * 1.5f * Time.deltaTime
        );

        float dirX = player.position.x - transform.position.x;
        Flip(dirX);
    }

    void CheckForPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, visionRange, playerLayer);

        if (hit != null)
        {
            player = hit.transform;
            attacking = true;
        }
        else
        {
            attacking = false;
        }
    }

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
