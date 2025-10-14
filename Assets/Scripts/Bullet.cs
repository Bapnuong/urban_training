using UnityEngine;

public class Bullet : MonoBehaviour
{
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

}
