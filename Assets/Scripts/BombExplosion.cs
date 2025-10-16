using UnityEngine;
using System.Collections;

public class BombExplosion : MonoBehaviour
{
    [Header("Explosion Settings")]
    [Tooltip("Seconds to wait before triggering the explosion")]
    public float Delay = 1f;

    [Tooltip("Maximum stress for camera shake (0-1)")]
    public float MaximumStress = 0.6f;

    [Tooltip("Explosion range radius")]
    public float Range = 45f;

    [Tooltip("Damage dealt to objects in range")]
    public float explosionDamage = 50f;

    [Header("Effect")]
    public GameObject explosionEffect; // hiệu ứng nổ

    private IEnumerator Start()
    {
        // ⏱️ Đợi thời gian delay trước khi nổ
        yield return new WaitForSeconds(Delay);

        // 💥 Hiệu ứng nổ
        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // 🔊 Phát âm thanh nổ (nếu có SoundManager)
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound6();

        // 🔎 Tìm tất cả đối tượng trong scene
        GameObject[] targets = GameObject.FindObjectsOfType<GameObject>();
        foreach (var target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance > Range) continue;

            // 🎥 Rung camera
            StressReceiver receiver = target.GetComponent<StressReceiver>();
            if (receiver != null)
            {
                float distance01 = Mathf.Clamp01(distance / Range);
                float stress = (1 - Mathf.Pow(distance01, 2)) * MaximumStress;
                receiver.InduceStress(stress);
            }

            // 🧍‍♂️ Gây sát thương cho Player
            if (target.CompareTag("Player"))
            {
                Damaged playerDamage = target.GetComponent<Damaged>();
                if (playerDamage != null)
                {
                    float distance01 = Mathf.Clamp01(distance / Range);
                    float damage = (1 - distance01) * explosionDamage;
                    playerDamage.PlayerTakeDamage(damage);
                }
            }
            // 👾 Gây sát thương cho Enemy
            else if (target.CompareTag("Enemy"))
            {
                Enemydamaged enemyDamage = target.GetComponent<Enemydamaged>();
                if (enemyDamage != null)
                {
                    float distance01 = Mathf.Clamp01(distance / Range);
                    float damage = (1 - distance01) * explosionDamage;
                    enemyDamage.EnemyTakeDamage(damage);
                }
            }
        }

        // 🧨 Hủy bom sau khi nổ
        Destroy(gameObject, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
