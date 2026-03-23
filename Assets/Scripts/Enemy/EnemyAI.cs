using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    public enum BaseState { Idle, Patrol }
    public enum PatrolMode { Loop, OneTime, Reverse }

    [Header("Target")]
    public string targetTag = "Player";
    Transform player;

    [Header("Detection")]
    public float detectionRadius = 10f;
    public float shootDistance = 6f;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Behaviour")]
    public BaseState baseState = BaseState.Patrol;

    [Header("Patrol")]
    public PatrolMode patrolMode = PatrolMode.Loop;
    public Transform[] waypoints;
    public float waypointReachDistance = 1f;

    int currentWaypoint;
    bool movingForward = true;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float bulletSpeed = 12f;

    float nextFireTime;

    Rigidbody rb;

    // ⭐ Movement State
    Vector3 moveTarget;
    bool shouldMove;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        FindTarget();
    }

    void FindTarget()
    {
        GameObject obj = GameObject.FindGameObjectWithTag(targetTag);
        if (obj) player = obj.transform;
    }

    void Update()
    {
        if (!player)
        {
            FindTarget();
            return;
        }

        shouldMove = false;

        float dist = Vector3.Distance(rb.position, player.position);

        if (dist <= detectionRadius)
        {
            RotateTowards(player.position);

            if (dist > shootDistance)
            {
                moveTarget = player.position;
                shouldMove = true;
            }
            else
            {
                ShootPlayer();
            }
        }
        else
        {
            if (baseState == BaseState.Patrol && waypoints.Length > 0)
            {
                moveTarget = waypoints[currentWaypoint].position;
                shouldMove = true;

                CheckWaypointReached();
                RotateTowards(moveTarget);
            }
        }
    }

    void FixedUpdate()
    {
        if (!shouldMove) return;

        Vector3 newPos = Vector3.MoveTowards(
            rb.position,
            moveTarget,
            moveSpeed * Time.fixedDeltaTime);

        rb.MovePosition(newPos);
    }

    void CheckWaypointReached()
    {
        Vector3 flatEnemy = rb.position;
        flatEnemy.y = 0;

        Vector3 flatTarget = moveTarget;
        flatTarget.y = 0;

        float dist = Vector3.Distance(flatEnemy, flatTarget);

        if (dist < waypointReachDistance)
        {
            switch (patrolMode)
            {
                case PatrolMode.Loop:
                    currentWaypoint++;
                    if (currentWaypoint >= waypoints.Length)
                        currentWaypoint = 0;
                    break;

                case PatrolMode.OneTime:
                    if (currentWaypoint < waypoints.Length - 1)
                        currentWaypoint++;
                    break;

                case PatrolMode.Reverse:
                    if (movingForward)
                    {
                        currentWaypoint++;
                        if (currentWaypoint >= waypoints.Length - 1)
                            movingForward = false;
                    }
                    else
                    {
                        currentWaypoint--;
                        if (currentWaypoint <= 0)
                            movingForward = true;
                    }
                    break;
            }
        }
    }

    void RotateTowards(Vector3 pos)
    {
        Vector3 dir = pos - rb.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        rb.MoveRotation(
            Quaternion.Slerp(
                rb.rotation,
                targetRot,
                8f * Time.fixedDeltaTime));
    }

    void ShootPlayer()
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;

        GameObject bullet =
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.linearVelocity = firePoint.forward * bulletSpeed;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootDistance);
    }
}