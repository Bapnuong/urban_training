using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    private Transform player;  // không cần public nữa
    public Transform firePoint;
    public GameObject bulletPrefab;

    public float attackRange = 10f;
    public float bulletForce = 20f;
    public float attackCooldown = 1.5f;

    private float nextAttackTime = 0f;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Tìm Player bằng tag
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
        }
        else
        {
            Debug.LogError("Không tìm thấy Player! Hãy chắc chắn Player có tag 'Player'");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            // Luôn chạy theo player khi ngoài tầm bắn
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            // Trong tầm bắn -> dừng lại
            agent.isStopped = true;

            // Xoay về phía Player
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);

            // Bắn cooldown
            if (Time.time >= nextAttackTime)
            {
                Shoot();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(firePoint.forward * bulletForce*1000, ForceMode.Impulse);
        }
        Debug.Log("Boss bắn đạn!");
    }
}
