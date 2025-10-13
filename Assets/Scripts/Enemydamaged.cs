using UnityEngine;
using TMPro;
using System.Collections;

public class Enemydamaged : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public GameObject hop;
    public GameObject HP;
    public GameObject Armor;

    public TextMeshProUGUI textKill;
    public AudioSource enemyAudio;     // 🎧 âm thanh riêng của enemy
    public AudioClip killClip;         // file âm kill gắn sẵn

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (textKill != null)
            textKill.gameObject.SetActive(false);

        // Nếu chưa gán audioSource trong Inspector, tự tìm
        if (enemyAudio == null)
            enemyAudio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("bullet"))
        {
            Debug.Log("Enemy hit by bullet");
            EnemyTakeDamage(20f);
            Destroy(collision.gameObject);
        }
    }

    public void EnemyTakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        animator.SetTrigger("Death");
        Debug.Log("Enemy died");

        int luck = Random.Range(1, 11);
        GameObject drop = null;

        if (luck == 6) drop = Instantiate(hop, transform.position, Quaternion.identity);
        else if (luck == 7) drop = Instantiate(HP, transform.position, Quaternion.identity);
        else if (luck == 8) drop = Instantiate(Armor, transform.position, Quaternion.identity);

        if (drop != null)
        {
            Rigidbody rb = drop.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }

        // 🔊 Phát âm kill (từ chính AudioSource của bot)
        if (enemyAudio != null && killClip != null)
        {
            enemyAudio.clip = killClip;
            enemyAudio.Play();
        }

        // 💬 Hiển thị text kill
        if (textKill != null)
            StartCoroutine(ShowKillText());

        // Hủy bot sau 0.9s
        Destroy(gameObject, 0.9f);
    }

    IEnumerator ShowKillText()
    {
        textKill.gameObject.SetActive(true);
        textKill.text = "Kiendeptrai đã hạ gục kẻ địch!";
        yield return new WaitForSeconds(0.8f);
        textKill.gameObject.SetActive(false);
    }
}
