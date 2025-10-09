using UnityEngine;
using UnityEngine.UIElements;

public class Enemydamaged : MonoBehaviour
{

    public float maxhealth = 100f;
    public float currenthealth;
    public GameObject hop; // vật phẩm rơi ra khi chết
    public GameObject HP;
    public GameObject Armor;
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
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
        animator.SetTrigger("Death");
        Debug.Log("Enemy died");
        int henxui = Random.Range(1, 10);
        if (henxui > 5) // 50% cơ hội rơi ra vật phẩm
        {
            if(henxui == 6)
            {
                GameObject drop = Instantiate(hop, transform.position, Quaternion.identity);
                Rigidbody rb = drop.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(Vector3.up * 5f, ForceMode.Impulse); // bắn lên trên 1 chút
                }
            }
            else if(henxui == 7)
            {
                GameObject Hp = Instantiate(HP, transform.position, Quaternion.identity);
                Rigidbody rb = HP.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(Vector3.up * 5f, ForceMode.Impulse); // bắn lên trên 1 chút
                }
            }
            else if(henxui == 8)
            {
                GameObject armor = Instantiate(Armor, transform.position, Quaternion.identity);
                Rigidbody rb = armor.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(Vector3.up * 5f, ForceMode.Impulse); // bắn lên trên 1 chút
                }
            }
        }    
        Destroy(this.gameObject);
    }
}
