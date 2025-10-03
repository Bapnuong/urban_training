using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    private Transform player;
    public Transform firePoint;
    public GameObject bulletPrefab;

    public float attackRange = 10f;
    public float bulletForce = 20f;
    public float attackCooldown = 0.3f; // delay giữa mỗi viên
    public int magazineSize = 5;        // số viên trong 1 băng
    public float reloadTime = 2f;       // thời gian nạp đạn
    public float speed = 800f;  

    private int currentAmmo;
    private float nextAttackTime = 0f;
    private bool isReloading = false;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentAmmo = magazineSize; // full đạn khi bắt đầu

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

        if (isReloading) return; // khi đang reload thì không làm gì

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;

            // Xoay về phía Player
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);

            // Bắn cooldown
            if (Time.time >= nextAttackTime && currentAmmo > 0)
            {
                Shoot();
                currentAmmo--;
                nextAttackTime = Time.time + attackCooldown;

                if (currentAmmo <= 0)
                {
                    StartCoroutine(Reload());
                }
            }
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(firePoint.forward * bulletForce * speed, ForceMode.Impulse);
        }
        Debug.Log("Boss bắn đạn! Còn: " + currentAmmo + " viên");
    }

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Boss đang nạp đạn...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        isReloading = false;
        Debug.Log("Boss đã nạp xong!");
    }
}
