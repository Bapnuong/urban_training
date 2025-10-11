using UnityEngine;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    // Lưu ammo cho từng loại súng theo tên
    private Dictionary<string, (int currentAmmo, int reserveAmmo)> weaponAmmoData =
        new Dictionary<string, (int, int)>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Lưu ammo của một vũ khí
    public void SaveAmmo(string weaponName, int current, int reserve)
    {
        weaponAmmoData[weaponName] = (current, reserve);
    }

    // Lấy ammo đã lưu (nếu có)
    public (int current, int reserve) GetAmmo(string weaponName, int defaultMag, int defaultReserve)
    {
        if (weaponAmmoData.TryGetValue(weaponName, out var ammo))
            return ammo;
        else
            return (defaultMag, defaultReserve);
    }
}
