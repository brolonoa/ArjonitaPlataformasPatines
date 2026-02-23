using UnityEngine;

public class DronEnemy : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float reachDistance = 0.1f;
    [SerializeField] bool canPatrol;

    private int currentIndex = 0;
    private int direction = 1; 

    private void Update()
    {
        if (canPatrol) Patrol();
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentIndex];


        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);


        if (Vector3.Distance(transform.position, targetPoint.position) <= reachDistance)
        {
            currentIndex += direction;

            if (currentIndex >= patrolPoints.Length)
            {
                direction = -1;
                currentIndex = patrolPoints.Length - 2;
            }
            else if (currentIndex < 0)
            {
                direction = 1;
                currentIndex = 1;
            }
        }
    }
}
