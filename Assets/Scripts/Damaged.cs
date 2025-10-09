using UnityEngine;
using UnityEngine.UI;

public class Damaged : MonoBehaviour
{
    [Header("Player Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    //hieu ung 
    public GameObject objhieuung;
    public float effectDuration = 0.4f;

    [Header("Armor")]
    public float maxArmor = 100f;
    public float currentArmor;

    [Header("UI")]
    public Text textHealth;
    public Text textArmor;

    [Header("Animation")]
    private Animator animator;
    private bool isDead = false;
    private float lastHitTime = 0f;
    public float hitCooldown = 0.5f;

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

        // Giảm armor trước
        if (currentArmor > 0)
        {
            float armorDamage = Mathf.Min(damage, currentArmor);
            currentArmor -= armorDamage;
            damage -= armorDamage;
        }

        if (damage > 0)
        {
            currentHealth -= damage;

            // Bật hiệu ứng trong thời gian ngắn
            StartCoroutine(ShowHitEffect());
        }

        UpdateHealthText();
        UpdateArmorText();

        // Trigger animation Hit nếu chưa chết và cooldown đã hết
        if (currentHealth > 0 && Time.time - lastHitTime >= hitCooldown)
        {
            animator.SetTrigger("Hit");
            lastHitTime = Time.time;
        }

        // Kiểm tra chết
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private System.Collections.IEnumerator ShowHitEffect()
    {
        objhieuung.SetActive(true);
        yield return new WaitForSeconds(effectDuration);
        objhieuung.SetActive(false);
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
            Destroy(collision.gameObject);
        }
    }
    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        UpdateHealthText();
    }
    public void AddArmor(float amount)
    {
        if (isDead) return;
        currentArmor += amount;
        if (currentArmor > maxArmor)
            currentArmor = maxArmor;
        UpdateArmorText();
    }
    void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger("Die");
        Debug.Log("Player died!");
    }
}
