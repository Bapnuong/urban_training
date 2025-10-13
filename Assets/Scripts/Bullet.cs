using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;     // Tốc độ bay
    public int damage = 20;       // Sát thương gây ra
    public float lifeTime = 3f;   // Thời gian tồn tại

    private Transform target;

    private Vector3 direction;

    void Start()
    {
        Destroy(gameObject, lifeTime); // Tự hủy sau lifeTime giây
        target = GameObject.FindGameObjectWithTag("Player").transform;
        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
        }
        else
        {
            direction = transform.forward;
        }
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound0();  // hoặc PlaySound(index) tùy âm bạn gán
        }
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
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
