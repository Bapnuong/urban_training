using UnityEngine;

public class HPsupply : MonoBehaviour
{
    public float healAmount = 20f; 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Damaged playerHealth = other.GetComponent<Damaged>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
                Debug.Log("Nhặt được " + healAmount + " HP!");
            }
            Destroy(gameObject);
        }
    }
}
