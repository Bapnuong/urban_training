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
        // Đợi delay
        yield return new WaitForSeconds(Delay);

        // Gọi hiệu ứng nổ nếu có
        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // Tìm tất cả gameobject trong scene
        GameObject[] targets = GameObject.FindObjectsOfType<GameObject>();
        foreach (var target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance > Range) continue;

            // ✅ Rung camera (StressReceiver)
            StressReceiver receiver = target.GetComponent<StressReceiver>();
            if (receiver != null)
            {
                float distance01 = Mathf.Clamp01(distance / Range);
                float stress = (1 - Mathf.Pow(distance01, 2)) * MaximumStress;
                receiver.InduceStress(stress);
            }

            // ✅ Gây sát thương cho Enemy hoặc Player
            if (target.CompareTag("Enemy") || target.CompareTag("Player"))
            {
                Damaged damageReceiver = target.GetComponent<Damaged>();
                if (damageReceiver != null)
                {
                    float distance01 = Mathf.Clamp01(distance / Range);
                    float damage = (1 - distance01) * explosionDamage;
                    damageReceiver.PlayerTakeDamage(damage);
                }
            }
        }

        // Sau khi nổ thì hủy bom
        Destroy(gameObject, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
