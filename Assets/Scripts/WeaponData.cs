using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Weapon/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Settings")]
    public string weaponName = "New Gun";
    public GameObject bulletPrefab;
    public float bulletVelocity = 30f;
    public float bulletLifetime = 2f;

    [Header("Fire Settings")]
    public float timeBetween = 0.1f;
    public int bulletsPerShot = 1;
    public float spreadIntensity = 0.05f;
    public Weapon.fireMode fireMode = Weapon.fireMode.automatic;

    [Header("Ammo Settings")]
    public int magSize = 30;
    public int reserveAmmo = 90;
    public int maxAmmo = 180;

    [Header("Reload Settings")]
    public float reloadTime = 2f;
}
