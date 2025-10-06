using UnityEngine;
using UnityEngine.UI;

public class Damaged : MonoBehaviour
{
    [Header("Player Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Armor")]
    public float maxArmor = 100f;
    public float currentArmor;

    [Header("UI")]
    public Text textHealth;
    public Text textArmor;

    [Header("Animation")]
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        currentArmor = maxArmor;
        currentHealth = maxHealth;
        UpdateHealthText();
        UpdateArmorText();

        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
        }
    }

    private void Update()
    {
        UpdateHealthText();
        UpdateArmorText();
    }

    public void PlayerTakeDamage(float damage)
    {
        if (isDead) return;

        // Xử lý damage vào armor trước, sau đó health
        if (currentArmor > 0)
        {
            float armorDamage = Mathf.Min(damage, currentArmor);
            currentArmor -= armorDamage;
            damage -= armorDamage; // Damage còn lại
        }

        if (damage > 0 && currentArmor <= 0)
        {
            currentHealth -= damage;
        }

        // ✅ LUÔN TRIGGER ANIMATION KHI BỊ BẮN (nếu chưa chết)
        if (currentHealth > 0)
        {
            animator.SetTrigger("Hit");
        }

        UpdateHealthText();
        UpdateArmorText();

        // Kiểm tra chết
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void UpdateHealthText()
    {
        if (textHealth != null)
            textHealth.text = "HP: " + Mathf.Max(0, currentHealth).ToString("F0");
    }

    public void UpdateArmorText()
    {
        if (textArmor != null)
            textArmor.text = "Armor: " + Mathf.Max(0, currentArmor).ToString("F0");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("bullet"))
        {
            Debug.Log("Player hit by bullet!");
            PlayerTakeDamage(20f);

            // Hủy bullet sau khi va chạm
            Destroy(collision.gameObject);
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player died!");

        animator.SetTrigger("Die");
    }
}