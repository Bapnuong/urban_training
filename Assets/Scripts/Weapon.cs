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
    public GameObject smokeEffect;  // Prefab khói

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
        currentAmmo = weaponData.magSize;
        reserveAmmo = weaponData.reserveAmmo;
        currentburst = weaponData.bulletsPerShot;

        playerAnim = FindObjectOfType<Damaged>();
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

        // Bắn
        if (readyToShoot && isShooting && canShoot && currentAmmo > 0)
        {
            currentburst = weaponData.bulletsPerShot;
            Shoot();
        }else if (playerAnim != null)
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
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        GameObject bullet = Instantiate(weaponData.bulletPrefab, bulletspawn.position, Quaternion.identity);
        bullet.transform.forward = shootingDirection;
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * weaponData.bulletVelocity, ForceMode.Impulse);
        StartCoroutine(DestroyBulletAfterTime(bullet, weaponData.bulletLifetime));
        //khoi
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

    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        Vector3 directionWithoutSpread = targetPoint - bulletspawn.position;
        float x = UnityEngine.Random.Range(-weaponData.spreadIntensity, weaponData.spreadIntensity);
        float y = UnityEngine.Random.Range(-weaponData.spreadIntensity, weaponData.spreadIntensity);
        return directionWithoutSpread + new Vector3(x, y, 0);
    }

    public void AddAmmo(int amount)
    {
        reserveAmmo = Mathf.Min(reserveAmmo + amount, weaponData.maxAmmo);
        Debug.Log("Đạn dự trữ: " + reserveAmmo);
    }

    public enum fireMode
    {
        automatic,
        burst,
        single
    }
}
