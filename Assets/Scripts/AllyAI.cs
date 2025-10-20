using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AllyAI : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public LayerMask enemyLayer;

    [Header("Stats")]
    public float sightRange = 20f;
    public float attackRange = 10f;
    public float moveSpeed = 4f;
    public float bulletForce = 30f;
    public float attackCooldown = 0.3f;
    public int magazineSize = 5;
    public float reloadTime = 2f;

    [Header("Wander Settings")]
    public float wanderRadius = 25f;
    public float wanderDelay = 1.5f;
    public float stuckCheckTime = 3f; // thời gian kiểm tra bị kẹt

    private int currentAmmo;
    private bool isReloading = false;
    private float nextAttackTime = 0f;
    private Transform currentEnemy;

    private NavMeshAgent agent;
    private Animator animator;

    private Vector3 lastPosition;
    private float stuckTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = 0.5f;
        }

        currentAmmo = magazineSize;
        StartCoroutine(WanderLoop());
    }

    void Update()
    {
        if (agent == null || isReloading) return;

        // --- Nếu enemy hiện tại chết hoặc bị xóa ---
        if (currentEnemy != null && !currentEnemy.gameObject.activeInHierarchy)
        {
            currentEnemy = null;
            agent.isStopped = false; // 🔥 Sửa lỗi đứng yên sau khi enemy chết
            StartCoroutine(ResumePatrolAfterLost());
        }

        // --- Tìm enemy ---
        currentEnemy = FindClosestEnemy();

        if (currentEnemy != null)
        {
            float dist = Vector3.Distance(transform.position, currentEnemy.position);

            if (dist <= attackRange)
            {
                agent.isStopped = true;
                LookAt(currentEnemy.position);
                TryShoot();
            }
            else if (dist <= sightRange)
            {
                agent.isStopped = false;
                agent.SetDestination(currentEnemy.position);
            }
            else
            {
                currentEnemy = null;
                agent.isStopped = false;
            }
        }

        // --- Kiểm tra kẹt ---
        if (Vector3.Distance(transform.position, lastPosition) < 0.1f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > stuckCheckTime)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.isStopped = false;
                agent.SetDestination(newPos);
                stuckTimer = 0;
            }
        }
        else
        {
            stuckTimer = 0;
        }

        lastPosition = transform.position;

        // --- Cập nhật animation ---
        if (animator != null)
            animator.SetBool("isRunning", agent.velocity.magnitude > 0.1f);
    }

    IEnumerator WanderLoop()
    {
        while (true)
        {
            agent.isStopped = false; // ✅ đảm bảo bot luôn được phép di chuyển

            if (currentEnemy == null && !isReloading)
            {
                if (!agent.pathPending && (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance + 0.2f))
                {
                    Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                    agent.SetDestination(newPos);
                }
            }
            yield return new WaitForSeconds(wanderDelay);
        }
    }

    IEnumerator ResumePatrolAfterLost()
    {
        yield return new WaitForSeconds(1f);

        if (currentEnemy == null && !isReloading)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.isStopped = false;
            agent.SetDestination(newPos);
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    void TryShoot()
    {
        if (Time.time >= nextAttackTime && currentAmmo > 0)
        {
            Shoot();
            currentAmmo--;
            nextAttackTime = Time.time + attackCooldown;

            if (currentAmmo <= 0)
                StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);

        if (animator != null)
            animator.SetTrigger("Shoot");
    }

    IEnumerator Reload()
    {
        isReloading = true;
        if (animator != null)
            animator.SetTrigger("Reload");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        isReloading = false;
    }

    Transform FindClosestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, sightRange, enemyLayer);
        float minDist = Mathf.Infinity;
        Transform best = null;

        foreach (var hit in hits)
        {
            if (!hit.gameObject.activeInHierarchy) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                best = hit.transform;
            }
        }
        return best;
    }

    void LookAt(Vector3 point)
    {
        Vector3 dir = (point - transform.position);
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}
