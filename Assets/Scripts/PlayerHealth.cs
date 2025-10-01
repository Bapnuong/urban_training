using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;   // Máu tối đa
    private int currentHealth;    // Máu hiện tại

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Hàm nhận sát thương
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player bị bắn trúng! Máu còn: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player đã chết!");
        // TODO: Có thể thêm màn hình Game Over, respawn hoặc restart scene
        Destroy(gameObject); // Tạm thời xóa Player khi chết
    }
}
