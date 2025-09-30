using UnityEngine;

public class Enemydamaged : MonoBehaviour
{

    public float maxhealth = 100f;
    public float currenthealth;
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
    }
}
