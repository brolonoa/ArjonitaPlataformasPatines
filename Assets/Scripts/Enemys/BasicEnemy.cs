using System.Collections;
using UnityEngine;

public class BasicEnemy : MonoBehaviour, IParryable
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
    [SerializeField] private float shootCooldown = 1.5f;
    private float shootCooldownCounter = 0f;

    [Header("Visual")]
    [SerializeField] private Transform visual;
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] float stunTime;
    [SerializeField] Animator anim;

    private int currentIndex = 0;
    private float waitCounter;
    private bool waiting;

    private Transform player;
    private float aimCounter;
    private bool aiming;

    [SerializeField] private bool canBeParried;
    public bool CanBeParried => canBeParried;

    void Update()
    {
        if (shootCooldownCounter > 0)
            shootCooldownCounter -= Time.deltaTime;

        CheckForPlayer();

        if (player != null)
            HandleAttack();
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
                    currentIndex = 0;
            }
            return;
        }

        Transform targetPoint = patrolPoints[currentIndex];

        Vector3 targetPos = new Vector3(
            targetPoint.position.x,
            transform.position.y, 
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

 
    void HandleAttack()
    {
        if (shootCooldownCounter > 0f)
        {
            lineRenderer.enabled = false;
            return;
        }

        Vector2 direction = (player.position - firePoint.position).normalized;

      
        RaycastHit2D playerHit = Physics2D.Raycast(
            firePoint.position,
            direction,
            visionRange,
            playerLayer
        );

       
        RaycastHit2D visualHit = Physics2D.Raycast(
            firePoint.position,
            direction,
            visionRange,
            obstacleLayer
        );

        
        if (aiming)
            lineRenderer.enabled = true;
        else
            lineRenderer.enabled = false;
        lineRenderer.SetPosition(0, firePoint.position);

        if (visualHit.collider != null)
            lineRenderer.SetPosition(1, visualHit.point);
        else
            lineRenderer.SetPosition(1, firePoint.position + (Vector3)direction * visionRange);

        
        if (playerHit.collider != null)
        {
            aiming = true;
            aimCounter += Time.deltaTime;

            if (aimCounter >= aimTime)
            {
                Shoot(direction);

                aimCounter = 0f;
                shootCooldownCounter = shootCooldown;
                aiming = false;
                lineRenderer.enabled = false; 
            }
        }
        else
        {
            aiming = false;
            aimCounter = 0f;
        }
        Flip(direction.x);
    }

    void Shoot(Vector2 dir)
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.GetComponent<EnemyProjectile>().Init(dir);
    }

    
    void Flip(float dirX)
    {
        if (visual == null) return;

        if (dirX > 0)
            visual.localScale = new Vector3(1, 1, 1);
        else if (dirX < 0)
            visual.localScale = new Vector3(-1, 1, 1);
    }
    public void OnParry()
    {
        if (canBeParried)
        {
            
        }
    }
    IEnumerator StunTime()
    {
        anim.SetTrigger("stuned");
        canBeParried = false;
        yield return new WaitForSeconds(stunTime);
        canBeParried = true;
        anim.SetTrigger("recoberStun");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}
