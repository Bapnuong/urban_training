using UnityEngine;
using System.Collections;

/* Script gây hiệu ứng vụ nổ và gọi hàm BomNo() cho kẻ địch trong phạm vi */
public class bomkien  : MonoBehaviour
{
    [Tooltip("Thời gian chờ trước khi nổ (giây)")]
    public float Delay = 1f;

    [Tooltip("Bán kính vụ nổ (phạm vi ảnh hưởng)")]
    public float Range = 5f;

    [Tooltip("Hiệu ứng hạt (khói, lửa...) khi nổ")]
    public ParticleSystem explosionEffect;

    [Tooltip("Âm thanh nổ (tùy chọn)")]
    public AudioSource explosionSound;

    private IEnumerator Start()
    {
        // Chờ thời gian Delay trước khi nổ
        yield return new WaitForSeconds(Delay);

        // Gọi hiệu ứng nổ
        PlayParticles();

        // Phát âm thanh (nếu có)
        if (explosionSound != null)
            explosionSound.Play();

        // Tìm tất cả các đối tượng có tag "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Duyệt qua từng Enemy
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            // Nếu Enemy nằm trong tầm vụ nổ
            if (distance <= Range)
            {
                // Gọi phương thức "BomNo" nếu có
                enemy.SendMessage("BomNo", SendMessageOptions.DontRequireReceiver);
            }
        }

        // Hủy object sau khi nổ (tuỳ chỉnh)
        Destroy(gameObject, 1.5f);
    }

    // Phát hiệu ứng nổ (Particle System)
    private void PlayParticles()
    {
        if (explosionEffect != null)
        {
            explosionEffect.Play();
        }
        else
        {
            // Nếu có particle nằm trong con, phát tất cả
            var children = transform.GetComponentsInChildren<ParticleSystem>();
            foreach (var p in children)
                p.Play();
        }
    }

    // Vẽ vòng tròn phạm vi nổ trong Scene view để dễ chỉnh
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
