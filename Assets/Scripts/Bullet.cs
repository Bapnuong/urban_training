using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;     // Tốc độ bay
    public int damage = 20;       // Sát thương gây ra
    public float lifeTime = 3f;   // Thời gian tồn tại

    private Transform target;

    void Start()
    {
        Destroy(gameObject, lifeTime); // Tự hủy sau lifeTime giây
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (target == null) return;

        // Bay về phía Player
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Gây sát thương cho Player
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject); // Hủy viên đạn sau khi trúng
        }
    }
}
