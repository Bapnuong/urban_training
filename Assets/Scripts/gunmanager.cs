using UnityEngine;

public class danhsachvukhi : MonoBehaviour
{
    public GameObject[] weapons;
    public Transform weaponHolder;
    private GameObject currentWeapon;
    private int currentIndex = 0;

    void Start()
    {
        EquipWeapon(0);
    }

    void Update()
    {
        // Đổi bằng phím số
        for (int i = 0; i < weapons.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (i != currentIndex)
                    EquipWeapon(i);
            }
        }

        // Đổi bằng cuộn chuột
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
            EquipWeapon((currentIndex + 1) % weapons.Length);
        else if (scroll < 0f)
            EquipWeapon((currentIndex - 1 + weapons.Length) % weapons.Length);
    }

    void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;

        if (currentWeapon != null)
            Destroy(currentWeapon);

        currentWeapon = Instantiate(weapons[index], weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentIndex = index;

        // Gọi lại ApplyIKTargets sau khi đổi súng
        Weapon weaponScript = currentWeapon.GetComponent<Weapon>();
        if (weaponScript != null)
        {
            // delay 1 frame để chắc chắn rig cập nhật xong
            Invoke(nameof(ApplyIKSafe), 0.05f);
        }

        Debug.Log("Đổi sang vũ khí: " + currentWeapon.name);
    }

    void ApplyIKSafe()
    {
        if (currentWeapon == null) return;
        Weapon weaponScript = currentWeapon.GetComponent<Weapon>();
        if (weaponScript != null)
            weaponScript.ApplyIKTargets();
    }
}
    