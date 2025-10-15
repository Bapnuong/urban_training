using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class GrenadeThrower : MonoBehaviour
{
    [Header("Grenade & Throw settings")]
    public GameObject grenadePrefab;       // prefab bom (cần có Rigidbody + BombExplosion)
    public Transform throwPoint;           // điểm spawn bom (ví dụ near camera or hand)
    public float throwForce = 12f;         // lực ném (forward)
    public float upwardForce = 2.5f;       // lực nâng để tạo đường cung
    public float cooldown = 0.6f;          // thời gian giữa các lần ném
    public int grenadeCount = 5;           // số bom hiện có (-1 = vô hạn)

    [Header("Spawn safety")]
    public float spawnOffset = 0.6f;       // đẩy ra khỏi người để không chạm collider người chơi

    private bool canThrow = true;

    private Collider playerCollider;

    void Start()
    {
        // tìm collider trên cùng object (của player) để ignore collision với grenade spawn
        playerCollider = GetComponent<Collider>();
        if (throwPoint == null)
        {
            Debug.LogWarning("ThrowPoint not assigned. Using this.transform as throwPoint.");
            throwPoint = transform;
        }
    }

    void Update()
    {
        // Ném khi nhấn chuột trái (hoặc đổi thành phím khác)
        if (Input.GetMouseButtonDown(0) && canThrow)
        {
            if (grenadeCount == 0)
                return;

            StartCoroutine(ThrowGrenadeRoutine());

            if (grenadeCount > 0)
                grenadeCount--;
        }
    }

    private IEnumerator ThrowGrenadeRoutine()
    {
        canThrow = false;

        // Spawn position: lệch theo forward * spawnOffset để tránh va chạm với player
        Vector3 spawnPos = throwPoint.position + throwPoint.forward * spawnOffset;

        GameObject g = Instantiate(grenadePrefab, spawnPos, Quaternion.identity);

        // Nếu prefab chứa Rigidbody, áp lực để ném
        Rigidbody rb = g.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Tính vận tốc ném: forward + lên trên
            Vector3 velocity = throwPoint.forward * throwForce + Vector3.up * upwardForce;
            rb.linearVelocity = velocity;

            // Ignore collision giữa grenade và player collider (nếu có)
            Collider grenadeCol = g.GetComponent<Collider>();
            if (grenadeCol != null && playerCollider != null)
            {
                Physics.IgnoreCollision(grenadeCol, playerCollider, true);
                // Khôi phục ignore collision sau 0.5s để bom có thể va chạm với người sau này
                StartCoroutine(RestoreCollisionAfterDelay(grenadeCol, playerCollider, 0.6f));
            }
        }
        else
        {
            Debug.LogWarning("Grenade prefab has no Rigidbody. It won't be thrown physically.");
        }

        // Optionally play throw animation / sound here
        // Animator anim = GetComponent<Animator>(); if(anim) anim.SetTrigger("Throw");

        // cooldown
        yield return new WaitForSeconds(cooldown);
        canThrow = true;
    }

    private IEnumerator RestoreCollisionAfterDelay(Collider a, Collider b, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (a != null && b != null)
        {
            Physics.IgnoreCollision(a, b, false);
        }
    }

    // Public helper to add grenades (pickup)
    public void AddGrenades(int amount)
    {
        if (grenadeCount < 0) return; // infinite
        grenadeCount += amount;
    }
}
