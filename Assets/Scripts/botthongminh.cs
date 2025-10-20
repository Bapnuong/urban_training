using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Bossthongminh : MonoBehaviour
{
    private Transform player;
    public Transform firePoint;
    public GameObject bulletPrefab;

    [Header("Stats")]
    public float scanRadius = 50f;   // phạm vi quét tìm Player
    public float attackRange = 30f;   // tầm bắn
    public float bulletForce = 20f;   // lực bắn
    public float attackCooldown = 0.3f;
    public int magazineSize = 5;
    public float reloadTime = 2f;
    public float speed = 20f;         // tốc độ viên đạn
    public float patrolRadius = 10f;  // bán kính tuần tra
    public float targetUpdateRate = 1.5f;

    private int currentAmmo;
    private float nextAttackTime = 0f;
    private bool isReloading = false;

    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 patrolTarget;

    // 🧠 Bộ nhớ mất dấu Player
    private float lostTargetTimer = 0f;
    private float lostTargetDelay = 2f; // sau 2s không thấy player sẽ hủy mục tiêu

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        currentAmmo = magazineSize;

        agent.acceleration = 12f;
        agent.angularSpeed = 500f;
        agent.stoppingDistance = 0.1f;

        StartCoroutine(UpdateTargetContinuously());
    }

    void Update()
    {
        if (isReloading) return;
        if (CapturePoint.playerInside)
            scanRadius = 500f;
        else
            scanRadius = 50f;

        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            Debug.Log($"👀 Boss đã thấy Player, khoảng cách: {distance}");

            if (distance > scanRadius * 1.2f)
            {
                Debug.Log("🚫 Player đã rời khỏi phạm vi, Boss ngừng truy đuổi.");
                ClearTargetAndStop();
                return;
            }

            if (distance <= attackRange)
            {
                agent.isStopped = true;
                agent.stoppingDistance = 0f;

                // Xoay mặt về hướng Player
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
            else
            {
                // Tiến lại gần Player
                agent.isStopped = false;
                Vector3 dirToPlayer = (player.position - transform.position).normalized;
                Vector3 stopPos = player.position - dirToPlayer * (attackRange - 1f);
                agent.stoppingDistance = 0.2f;
                agent.SetDestination(stopPos);
                animator.SetBool("Moving", agent.velocity.magnitude > 0.1f);
            }
        }
        else
        {
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
        agent.stoppingDistance = 0.1f;

        if (!agent.hasPath || agent.remainingDistance < 0.3f)
        {
            Vector3 randomDir = Random.insideUnitSphere * patrolRadius + transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDir, out hit, patrolRadius, NavMesh.AllAreas))
            {
                patrolTarget = hit.position;
                agent.isStopped = false;
                agent.SetDestination(patrolTarget);
                animator.SetBool("Moving", true);
            }
        }
    }

    void Shoot()
    {
        animator.SetBool("IsShooting", true);

        if (bulletPrefab == null || firePoint == null || player == null)
        {
            Debug.LogWarning("⚠️ Thiếu bulletPrefab hoặc firePoint hoặc player!");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 shootDir = (player.position + Vector3.up * 1.2f - firePoint.position).normalized;
            rb.AddForce(shootDir * bulletForce, ForceMode.Impulse);
        }

        Debug.Log($"🔫 Boss bắn vào {player.name} | Còn {currentAmmo - 1}/{magazineSize} viên");
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

    Transform FindClosestPlayerInRange()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var p in players)
        {
            if (p == null || !p.activeInHierarchy) continue;
            float dist = Vector3.Distance(transform.position, p.transform.position);

            if (dist <= scanRadius && dist < minDist)
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
            Transform found = FindClosestPlayerInRange();

            if (found != null)
            {
                player = found;
                lostTargetTimer = 0f;
            }
            else if (player != null)
            {
                lostTargetTimer += targetUpdateRate;
                if (lostTargetTimer >= lostTargetDelay)
                {
                    Debug.Log("❌ Boss mất mục tiêu hoàn toàn sau " + lostTargetDelay + "s");
                    ClearTargetAndStop();
                    lostTargetTimer = 0f;
                }
            }
            else
            {
                player = null;
            }

            yield return new WaitForSeconds(targetUpdateRate);
        }
    }

    void ClearTargetAndStop()
    {
        player = null;
        agent.isStopped = false;
        animator.SetBool("IsShooting", false);
        Patrol();
    }
}
