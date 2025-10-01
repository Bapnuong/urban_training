using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead) return; // chết rồi thì ko xử lý gì nữa

        HandleMovement();
        HandleActions();
    }

    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);

        // Di chuyển nhân vật
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);

        // Idle / Run animation
        bool isRunning = move.magnitude > 0.1f;
        animator.SetBool("isRunning", isRunning);
    }

    private void HandleActions()
    {
        // Shoot
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Shoot");
        }

        // Reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            animator.SetTrigger("Reload");
        }

        // Test bị trúng đạn (phím H)
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(20);
        }

        // Test chết ngay lập tức (phím K)
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(999);
        }
    }

    // ---- Health System ----
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth > 0)
        {
            animator.SetTrigger("Hit"); // Animation bị trúng đạn
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        Debug.Log("Player died!");
    }
}
