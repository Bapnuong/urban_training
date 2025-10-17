using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Bossthongminh : MonoBehaviour
{
    private Transform player; // target hiện tại
    public Transform firePoint;
    public GameObject bulletPrefab;

    [Header("Stats")]
    public float scanRadius = 100f;       // 🔍 Phạm vi quét tìm player
    public float attackRange = 10f;      // Tầm bắn
    public float bulletForce = 20f;
    public float attackCooldown = 0.3f;
    public int magazineSize = 5;
    public float reloadTime = 2f;
    public float speed = 800f;
    public float patrolRadius = 10f;     // 🚶 Bán kính đi lang thang
    public float targetUpdateRate = 1.5f; // Chu kỳ quét lại mục tiêu

    private int currentAmmo;
    private float nextAttackTime = 0f;
    private bool isReloading = false;

    private NavMeshAgent agent;
    private Animator animator;

    private Vector3 patrolTarget; // vị trí ngẫu nhiên để đi lang thang

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        currentAmmo = magazineSize;

        agent.acceleration = 12f;
        agent.angularSpeed = 500f;
        agent.stoppingDistance = 2f;

        StartCoroutine(UpdateTargetContinuously());
    }

    void Update()
    {
        if (isReloading) return;

        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            // Nếu Player vẫn trong phạm vi quét
            if (distance <= scanRadius)
            {
                // Trong tầm tấn công
                if (distance <= attackRange)
                {
                    agent.isStopped = true;
                    Vector3 dir = (player.position - transform.position).normalized;
                    dir.y = 0;
                    transform.rotation = Quaternion.LookRotation(dir);

                    if (Time.time >= nextAttackTime && currentAmmo > 0)
                    {
                        Shoot();
                        currentAmmo--;
                        nextAttackTime = Time.time + attackCooldown;

                        if (currentAmmo <= 0)
                            StartCoroutine(Reload());
                    }
                    else
                    {
                        animator.SetBool("IsShooting", false);
                    }
                }
                else // Player còn trong vùng quét nhưng ngoài tầm bắn
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                    animator.SetBool("Moving", agent.velocity.magnitude > 0.1f);
                }
            }
            else
            {
                // Player rời khỏi vùng quét
                player = null;
            }
        }
        else
        {
            // Không thấy player → đi tuần ngẫu nhiên
            Patrol();
        }

        // Xoay theo hướng di chuyển
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    void Patrol()
    {
        animator.SetBool("IsShooting", false);

        if (!agent.hasPath || agent.remainingDistance < 1f)
        {
            Vector3 randomDir = Random.insideUnitSphere * patrolRadius;
            randomDir += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDir, out hit, patrolRadius, NavMesh.AllAreas))
            {
                patrolTarget = hit.position;
                agent.SetDestination(patrolTarget);
                agent.isStopped = false;
                animator.SetBool("Moving", true);
            }
        }
    }

    void Shoot()
    {
        animator.SetBool("IsShooting", true);

        if (bulletPrefab == null || firePoint == null || player == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 shootDir = (player.position + Vector3.up * 1.2f - firePoint.position).normalized;
            rb.AddForce(shootDir * bulletForce * speed, ForceMode.Impulse);
        }

        Debug.Log("Boss bắn đạn! Còn: " + currentAmmo + " viên");
    }

    IEnumerator Reload()
    {
        isReloading = true;
        animator.SetTrigger("Reload");
        Debug.Log("Boss đang nạp đạn...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        isReloading = false;
        Debug.Log("Boss đã nạp xong!");
    }

    // ✅ Tìm player gần nhất trong phạm vi scanRadius
    Transform FindClosestPlayerInRange()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closest = null;
        float minDist = scanRadius; // chỉ chọn trong phạm vi scanRadius

        foreach (var p in players)
        {
            if (!p.activeInHierarchy) continue;

            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = p.transform;
            }
        }

        return closest;
    }

    IEnumerator UpdateTargetContinuously()
    {
        while (true)
        {
            if (player == null)
                player = FindClosestPlayerInRange();

            yield return new WaitForSeconds(targetUpdateRate);
        }
    }
}
