using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletspawn;
    public float bulletVelocity = 30f;
    public float bulletLifetime = 2f;

    public Camera fpsCam;
    public bool isShooting, readytoShoot;
    bool canShoot = true;
    public float timeBetween = 2f;

    public int bulletpershoot = 3;
    public int currentburst;

    public float spreadIntensity;

    public enum fireMode
    {
        automatic,
        burst,
        single
    }

    public fireMode  currentshootingmode;

    private void Awake() 
    {
        readytoShoot = true;

        currentburst = bulletpershoot;
    }


    void Update()
    {
        if(currentshootingmode == fireMode.automatic)
        {
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if(currentshootingmode == fireMode.burst)
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }
        else if(currentshootingmode == fireMode.single)
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }
        if(readytoShoot && isShooting && canShoot)
        {
            currentburst = bulletpershoot;
            Shoot();
        }
    }
    void Shoot()
    {
        readytoShoot = false;
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
        
        GameObject bullet = Instantiate(bulletPrefab, bulletspawn.position, Quaternion.identity);
        bullet.transform.forward = shootingDirection;
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity,ForceMode.Impulse);
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletLifetime));
        if(canShoot)
        {
            Invoke("ResetShot", timeBetween);
            canShoot = false;
        }
        if(currentshootingmode == fireMode.burst && currentburst>1)
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
    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
        }
        Vector3 directionWithoutSpread = targetPoint - bulletspawn.position;
        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        return directionWithoutSpread + new Vector3(x,y,0);
    }
}
