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

    private int currentAmmo;
    private bool isReloading = false;
    private float nextAttackTime = 0f;
    private Transform currentEnemy;

    private NavMeshAgent agent;
    private Animator animator; // ✅ thêm Animator

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // ✅ gán Animator

        if (agent != null)
            agent.speed = moveSpeed;

        currentAmmo = magazineSize;
    }

    void Update()
    {
        if (agent == null || isReloading) return;

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
            else
            {
                agent.isStopped = false;
                agent.SetDestination(currentEnemy.position);
            }
        }
        else
        {
            agent.isStopped = true;
        }

        // ✅ Cập nhật animation di chuyển
        if (animator != null)
            animator.SetBool("isRunning", agent.velocity.magnitude > 0.1f);
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
            animator.SetTrigger("Shoot"); // ✅ animation bắn

        Debug.Log("dong minh bắn đạn! Còn: " + currentAmmo + " viên");
    }

    IEnumerator Reload()
    {
        isReloading = true;
        if (animator != null)
            animator.SetTrigger("Reload"); // ✅ animation nạp đạn
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
    }
}
