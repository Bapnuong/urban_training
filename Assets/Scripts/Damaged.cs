using UnityEngine;
using UnityEngine.UI;

public class Damaged : MonoBehaviour
{
    [Header("Player Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Text textHealth;

    [Header("Animation")]
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthText();

        animator = GetComponent<Animator>(); // Lấy Animator từ Player
    }

    private void Update()
    {
        UpdateHealthText();
    }

    public void PlayerTakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        UpdateHealthText();

        // Animation bị trúng đạn
        animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void UpdateHealthText()
    {
        if (textHealth != null)
            textHealth.text = currentHealth.ToString();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Player hit by enemy");
            PlayerTakeDamage(20f);
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player died");

        animator.SetTrigger("Die"); // gọi animation Die

        // Nếu muốn xoá nhân vật sau khi chết
        // Destroy(gameObject, 3f);
    }
}
