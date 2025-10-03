using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletspawn;
    public float bulletVelocity = 30f;
    public float bulletLifetime = 2f;

    public Camera fpsCam;
    public bool isShooting, readytoShoot;
    bool canShoot = true;
    public float timeBetween = 0.1f;

    public int bulletpershoot = 3;
    public int currentburst;

    public float spreadIntensity;

    public int maxAmmo = 180;
    public int reserveAmmo = 180;   // đạn dự trữ
    public int magSize = 30;        // sức chứa 1 băng
    public int currentAmmo;         // đạn trong băng hiện tại
    public Text ammortext;

    public enum fireMode
    {
        automatic,
        burst,
        single
    }

    public fireMode currentshootingmode;

    private void Awake()
    {
        readytoShoot = true;
        currentAmmo = magSize;
        currentburst = bulletpershoot;
    }

    void Update()
    {
        // --- điều khiển bắn ---
        if (currentshootingmode == fireMode.automatic)
            isShooting = Input.GetKey(KeyCode.Mouse0);
        else if (currentshootingmode == fireMode.burst)
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        else if (currentshootingmode == fireMode.single)
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);

        // --- bắn ---
        if (readytoShoot && isShooting && canShoot && currentAmmo > 0)
        {
            currentburst = bulletpershoot;
            Shoot();
        }

        // --- đổi chế độ bắn ---
        if (Input.GetKeyDown(KeyCode.B))
        {
            int nextMode = (int)currentshootingmode + 1;
            if (nextMode > (int)fireMode.single) nextMode = 0;
            currentshootingmode = (fireMode)nextMode;
            Debug.Log("Đã đổi sang chế độ: " + currentshootingmode);
        }

        // --- reload ---
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magSize && reserveAmmo > 0)
        {
            StartCoroutine(Reload(2f));
        }

        // --- update UI ---
        ammortext.text = currentAmmo.ToString() + " / " + reserveAmmo.ToString();
    }

    void Shoot()
    {
        readytoShoot = false;
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        GameObject bullet = Instantiate(bulletPrefab, bulletspawn.position, Quaternion.identity);
        bullet.transform.forward = shootingDirection;
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletLifetime));

        currentAmmo--;

        if (currentAmmo <= 0) return;

        if (canShoot)
        {
            Invoke("ResetShot", timeBetween);
            canShoot = false;
        }

        if (currentshootingmode == fireMode.burst && currentburst > 1)
        {
            currentburst--;
            Invoke("Shoot", timeBetween);
        }
    }

    private void ResetShot()
    {
        readytoShoot = true;
        canShoot = true;
    }

    IEnumerator DestroyBulletAfterTime(GameObject bullet, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(bullet);
    }

    IEnumerator Reload(float time)
    {
        canShoot = false;
        readytoShoot = false;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(time);

        int neededAmmo = magSize - currentAmmo;  // số đạn thiếu
        if (reserveAmmo >= neededAmmo)
        {
            currentAmmo = magSize;
            reserveAmmo -= neededAmmo;
        }
        else
        {
            currentAmmo += reserveAmmo;
            reserveAmmo = 0;
        }

        canShoot = true;
        readytoShoot = true;
        Debug.Log("Reloaded!");
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

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return directionWithoutSpread + new Vector3(x, y, 0);
    }
    public void AddAmmo(int amount)
    {
        reserveAmmo = Mathf.Min(reserveAmmo + amount, maxAmmo);
        Debug.Log("Đạn dự trữ: " + reserveAmmo);
    }

}
