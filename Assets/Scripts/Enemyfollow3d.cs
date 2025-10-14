using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BossAIi : MonoBehaviour
{
    private Transform player;
    public Transform firePoint;
    public GameObject bulletPrefab;

    public float attackRange = 10f;
    public float bulletForce = 20f;
    public float attackCooldown = 0.3f;
    public int magazineSize = 5;
    public float reloadTime = 2f;
    public float speed = 800f;

    private int currentAmmo;
    private float nextAttackTime = 0f;
    private bool isReloading = false;

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        currentAmmo = magazineSize;

        agent.acceleration = 12f;
        agent.angularSpeed = 500f;
        agent.stoppingDistance = 2f;

        FindPlayer(); // ✅ Tìm player lúc đầu
        StartCoroutine(CheckPlayerContinuously()); // ✅ Luôn kiểm tra lại nếu Player biến mất
    }

    void Update()
    {
        if (player == null) return;
        if (isReloading) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Di chuyển & xoay
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }

        // Nếu Player ngoài tầm bắn → đuổi theo
        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            float velocity = agent.velocity.magnitude;
            animator.SetBool("Moving", velocity > 0.1f);
        }
        else
        {
            agent.isStopped = true;
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
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
    }

    void Shoot()
    {
        animator.SetBool("IsShooting", true);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null && player != null)
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

    // ✅ Hàm tìm player
    void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            Debug.Log("Boss đã tìm thấy Player mới!");
        }
        else
        {
            player = null;
        }
    }

    // ✅ Kiểm tra liên tục để tìm lại Player khi bị xóa/chết
    IEnumerator CheckPlayerContinuously()
    {
        while (true)
        {
            if (player == null)
            {
                FindPlayer();
            }
            else if (!player.gameObject.activeInHierarchy)
            {
                player = null;
            }
            yield return new WaitForSeconds(1f); // kiểm tra mỗi giây để tiết kiệm hiệu năng
        }
    }
}
