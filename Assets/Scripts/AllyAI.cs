using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AllyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;            // Player để đi theo
    public Transform firePoint;         // Vị trí bắn
    public GameObject bulletPrefab;     // Prefab đạn
    public LayerMask enemyLayer;        // Layer của Enemy

    [Header("Stats")]
    public float sightRange = 20f;      // Tầm nhìn tìm enemy
    public float attackRange = 10f;     // Tầm bắn enemy
    public float followDistance = 3f;   // Khoảng cách giữ với player
    public float moveSpeed = 4f;        // Tốc độ di chuyển (dùng NavMeshAgent)
    public float bulletForce = 30f;     // Lực bắn đạn
    public float attackCooldown = 0.3f; // Thời gian giữa các viên
    public int magazineSize = 5;        // Số đạn / băng
    public float reloadTime = 2f;       // Thời gian nạp lại

    private int currentAmmo;
    private bool isReloading = false;
    private float nextAttackTime = 0f;

    private Transform currentEnemy;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        currentAmmo = magazineSize;
    }

    void Update()
    {
        if (player == null || agent == null) return;
        if (isReloading) return;

        // --- TÌM ENEMY ---
        currentEnemy = FindClosestEnemy();

        if (currentEnemy != null)
        {
            float distToEnemy = Vector3.Distance(transform.position, currentEnemy.position);
            if (distToEnemy <= attackRange)
            {
                // Dừng lại và bắn enemy
                agent.isStopped = true;
                LookAt(currentEnemy.position);
                TryShoot();
            }
            else
            {
                // Tiến đến gần enemy
                agent.isStopped = false;
                agent.SetDestination(currentEnemy.position);
            }
        }
        else
        {
            // --- KHÔNG CÓ ENEMY → ĐI THEO PLAYER ---
            float distToPlayer = Vector3.Distance(transform.position, player.position);

            if (distToPlayer > followDistance)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else
            {
                // Đứng cạnh player và nhìn theo player
                agent.isStopped = true;
                LookAt(player.position);
            }
        }
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

        Debug.Log("Ally bắn đạn! Còn: " + currentAmmo + " viên");
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Ally đang nạp đạn...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        isReloading = false;
        Debug.Log("Ally đã nạp xong!");
    }

    Transform FindClosestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, sightRange, enemyLayer);
        float minDist = Mathf.Infinity;
        Transform best = null;

        foreach (var hit in hits)
        {
            float distToPlayer = Vector3.Distance(hit.transform.position, player.position);
            float distToAlly = Vector3.Distance(hit.transform.position, transform.position);
            float priority = distToPlayer * 0.5f + distToAlly; // ưu tiên gần player hơn

            if (priority < minDist)
            {
                minDist = priority;
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
    }
}
