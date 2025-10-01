using UnityEngine;

public class BossShooter : MonoBehaviour
{
    public Transform FirePoint;        // chỗ bắn đạn
    public GameObject BulletPrefab;    // prefab đạn
    public float BulletForce = 20f;    // lực bắn
    public float DetectionRange = 30f;
    public float AttackCooldown = 1f;

    private float nextAttackTime = 0f;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= DetectionRange && Time.time >= nextAttackTime)
        {
            Shoot();
            nextAttackTime = Time.time + AttackCooldown;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(BulletPrefab, FirePoint.position, FirePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(FirePoint.forward * BulletForce, ForceMode.Impulse);
        }
        Debug.Log("Boss bắn đạn!");
    }
}
