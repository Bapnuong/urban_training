using UnityEngine;
using UnityEngine.UI; // ⚡ thêm để dùng UI

public class danhsachvukhi : MonoBehaviour
{
    [Header("Danh sách súng")]
    public GameObject[] weapons;

    [Header("Hình ảnh minh họa tương ứng (theo thứ tự)")]
    public Sprite[] weaponIcons;

    [Header("Vị trí gắn súng")]
    public Transform weaponHolder;

    [Header("UI hiển thị ảnh súng")]
    public Image weaponImage; // ⚡ ảnh trên Canvas

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
            Invoke(nameof(ApplyIKSafe), 0.05f);
        }

        // ✅ Cập nhật hình ảnh minh họa UI
        UpdateWeaponImage(index);

        Debug.Log("Đổi sang vũ khí: " + currentWeapon.name);
    }

    void ApplyIKSafe()
    {
        if (currentWeapon == null) return;
        Weapon weaponScript = currentWeapon.GetComponent<Weapon>();
        if (weaponScript != null)
            weaponScript.ApplyIKTargets();
    }

    void UpdateWeaponImage(int index)
    {
        if (weaponImage != null && weaponIcons != null && index < weaponIcons.Length)
        {
            weaponImage.sprite = weaponIcons[index];
            weaponImage.enabled = true; // hiện ảnh
        }
        else if (weaponImage != null)
        {
            weaponImage.enabled = false; // ẩn nếu không có ảnh
        }
    }
}
