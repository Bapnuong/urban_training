using UnityEngine;
using UnityEngine.UI;

public class Damaged : MonoBehaviour
{
    [Header("Player Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    [Header("Ammor")]
    public float maxAmmor = 100f;
    public float currentAmmor;
    [Header("UI")]
    public Text textHealth;
    public Text textAmmor;
    [Header("Animation")]
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        currentAmmor = maxAmmor;
        currentHealth = maxHealth;
        UpdateHealthText();
        UpdateAmmorText();

        animator = GetComponent<Animator>(); // Lấy Animator từ Player
    }

    private void Update()
    {
        UpdateHealthText();
        UpdateAmmorText();
    }

    public void PlayerTakeDamage(float damage)
    {
        if (isDead) return;
        if(currentAmmor > 0)
        {
            currentAmmor -= damage;
            return; 
        }
        else currentHealth -= damage;
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
    public void UpdateAmmorText()
    {
        if (textAmmor != null)
            textAmmor.text = currentAmmor.ToString();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("bullet"))
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
