using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Data")]
    public WeaponData weaponData;

    [Header("References")]
    public Transform bulletspawn;
    public Camera fpsCam;
    public Text ammortext;
    public GameObject smokeEffect;
    [HideInInspector] public bool isShooting;
    [HideInInspector] public bool readyToShoot;
    [HideInInspector] public bool canShoot = true;

    private int currentAmmo;
    private int reserveAmmo;
    private int currentburst;

    private Animator animator;
    private Damaged playerAnim;


    private void Awake()
    {
        ammortext = GameObject.FindGameObjectWithTag("AmmoText")?.GetComponent<Text>();
        if (weaponData == null)
        {
            Debug.LogError("WeaponData missing on " + name);
            return;
        }
        if (fpsCam == null)
        {
            fpsCam = Camera.main;
            if (fpsCam == null)
                Debug.LogError("No Camera.main found for weapon " + name);
        }

        readyToShoot = true;

        // ✅ Lấy ammo đã lưu từ WeaponManager (nếu có)
        var savedAmmo = WeaponManager.Instance != null
            ? WeaponManager.Instance.GetAmmo(weaponData.weaponName, weaponData.magSize, weaponData.reserveAmmo)
            : (weaponData.magSize, weaponData.reserveAmmo);

        currentAmmo = savedAmmo.Item1;
        reserveAmmo = savedAmmo.Item2;

        currentburst = weaponData.bulletsPerShot;

        playerAnim = FindObjectOfType<Damaged>();
    }

    private void OnDisable()
    {
        // ✅ Khi tắt vũ khí (đổi sang vũ khí khác) → Lưu lại ammo
        if (WeaponManager.Instance != null && weaponData != null)
        {
            WeaponManager.Instance.SaveAmmo(weaponData.weaponName, currentAmmo, reserveAmmo);
        }
    }

    void Update()
    {
        // Điều khiển bắn
        if (weaponData.fireMode == fireMode.automatic)
            isShooting = Input.GetKey(KeyCode.Mouse0);
        else if (weaponData.fireMode == fireMode.burst)
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        else if (weaponData.fireMode == fireMode.single)
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        else if (weaponData.fireMode == fireMode.shotgun)
            isShooting = Input.GetKeyDown(KeyCode.Mouse0); // shotgun dùng nhấn

        // Bắn
        if (readyToShoot && isShooting && canShoot && currentAmmo > 0)
        {
            currentburst = weaponData.bulletsPerShot;
            Shoot();
        }
        else if (playerAnim != null)
            playerAnim.GetComponent<Animator>().SetBool("isShooting", false);

        // Reload
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < weaponData.magSize && reserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }

        // UI
        if (ammortext != null)
            ammortext.text = currentAmmo + " / " + reserveAmmo;
    }

    void Shoot()
    {
        readyToShoot = false;

        // Shotgun xử lý khác: bắn nhiều viên (pellets) cùng lúc
        if (weaponData.fireMode == fireMode.shotgun)
        {
            int pellets = Mathf.Max(1, weaponData.bulletsPerShot);
            float shotgunSpread = weaponData.spreadIntensity * 2f;

            for (int i = 0; i < pellets; i++)
            {
                Vector3 pelletDir = CalculateDirectionAndSpread(shotgunSpread).normalized;
                GameObject pellet = Instantiate(weaponData.bulletPrefab, bulletspawn.position, Quaternion.identity);
                pellet.transform.forward = pelletDir;
                Rigidbody rb = pellet.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.AddForce(pelletDir * weaponData.bulletVelocity, ForceMode.Impulse);
                StartCoroutine(DestroyBulletAfterTime(pellet, weaponData.bulletLifetime));
            }
            Instantiate(smokeEffect, bulletspawn.position, bulletspawn.rotation);
            currentAmmo--;

            if (playerAnim != null)
                playerAnim.GetComponent<Animator>().SetBool("isShooting", true);

            if (currentAmmo <= 0) return;

            if (canShoot)
            {
                Invoke(nameof(ResetShot), weaponData.timeBetween);
                canShoot = false;
            }

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySound2();

            return;
        }

        // Các loại súng bình thường
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        GameObject bullet = Instantiate(weaponData.bulletPrefab, bulletspawn.position, Quaternion.identity);
        bullet.transform.forward = shootingDirection;
        Rigidbody rbBullet = bullet.GetComponent<Rigidbody>();
        if (rbBullet != null)
            rbBullet.AddForce(shootingDirection * weaponData.bulletVelocity, ForceMode.Impulse);
        StartCoroutine(DestroyBulletAfterTime(bullet, weaponData.bulletLifetime));
        Instantiate(smokeEffect, bulletspawn.position, bulletspawn.rotation);
        currentAmmo--;

        if (playerAnim != null)
            playerAnim.GetComponent<Animator>().SetBool("isShooting", true);
        if (currentAmmo <= 0) return;

        if (canShoot)
        {
            Invoke(nameof(ResetShot), weaponData.timeBetween);
            canShoot = false;
        }

        if (weaponData.fireMode == fireMode.burst && currentburst > 1)
        {
            currentburst--;
            Invoke(nameof(Shoot), weaponData.timeBetween);
        }

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound2();
    }

    private void ResetShot()
    {
        readyToShoot = true;
        canShoot = true;
    }

    IEnumerator DestroyBulletAfterTime(GameObject bullet, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(bullet);
    }

    IEnumerator Reload()
    {
        canShoot = false;
        readyToShoot = false;
        Debug.Log("Reloading...");

        if (playerAnim != null)
        {
            playerAnim.GetComponent<Animator>().SetBool("isShooting", false);
            playerAnim.GetComponent<Animator>().SetBool("Reload", true);
        }

        yield return new WaitForSeconds(weaponData.reloadTime);

        int neededAmmo = weaponData.magSize - currentAmmo;
        if (reserveAmmo >= neededAmmo)
        {
            currentAmmo = weaponData.magSize;
            reserveAmmo -= neededAmmo;
        }
        else
        {
            currentAmmo += reserveAmmo;
            reserveAmmo = 0;
        }

        canShoot = true;
        readyToShoot = true;
        if (playerAnim != null)
            playerAnim.GetComponent<Animator>().SetBool("Reload", false);

        Debug.Log("Reloaded!");

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound3();
    }

    public Vector3 CalculateDirectionAndSpread(float customSpread = -1f)
    {
        float spreadValue = customSpread > 0f ? customSpread : weaponData.spreadIntensity;

        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        Vector3 directionWithoutSpread = targetPoint - bulletspawn.position;
        float x = UnityEngine.Random.Range(-spreadValue, spreadValue);
        float y = UnityEngine.Random.Range(-spreadValue, spreadValue);
        return directionWithoutSpread + new Vector3(x, y, 0);
    }

    public void AddAmmo(int amount)
    {
        reserveAmmo = Mathf.Min(reserveAmmo + amount, weaponData.maxAmmo);
        Debug.Log("Đạn dự trữ: " + reserveAmmo);
    }

    public int GetCurrentAmmo() => currentAmmo;
    public int GetReserveAmmo() => reserveAmmo;

    public enum fireMode
    {
        automatic,
        burst,
        single,
        shotgun
    }
}
