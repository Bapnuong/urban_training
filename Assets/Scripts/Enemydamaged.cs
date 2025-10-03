using UnityEngine;

public class Enemydamaged : MonoBehaviour
{

    public float maxhealth = 100f;
    public float currenthealth;
    public GameObject hop; // vật phẩm rơi ra khi chết
    void Start()
    {
        currenthealth = maxhealth;
    }
    public void EnemyTakeDamage(float damage)
    {
        currenthealth -= damage;
        if (currenthealth <= 0)
        {
            Die();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("bullet"))
        {
            Debug.Log("Enemy hit by bullet");
            EnemyTakeDamage(20f);
            Destroy(collision.gameObject);
        }
    }
    void Die()
    {
        Debug.Log("Enemy died");
        GameObject drop = Instantiate(hop, transform.position, Quaternion.identity);
        Rigidbody rb = drop.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse); // bắn lên trên 1 chút
        }
        Destroy(this.gameObject);
    }
}
