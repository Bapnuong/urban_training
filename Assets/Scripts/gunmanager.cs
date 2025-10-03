using UnityEngine;

public class danhsachvukhi : MonoBehaviour
{
    [Header("Danh sách prefab vũ khí")]
    public GameObject[] weapons;   // danh sách prefab súng kéo từ Project vào
    public Transform weaponHolder; // empty object đặt ở tay player

    private GameObject currentWeapon;
    private int currentIndex = -1;

    void Start()
    {
        // Nếu muốn mặc định có súng đầu tiên
        if (weapons.Length > 0)
        {
            SwitchWeapon(0);
        }
    }

    void Update()
    {
        // Nhấn phím số để đổi vũ khí
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchWeapon(2);

        // Hoặc cuộn chuột
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) // lên
        {
            int next = (currentIndex + 1) % weapons.Length;
            SwitchWeapon(next);
        }
        else if (scroll < 0f) // xuống
        {
            int prev = (currentIndex - 1 + weapons.Length) % weapons.Length;
            SwitchWeapon(prev);
        }
    }

    void SwitchWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;

        // Xóa vũ khí cũ
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        // Sinh vũ khí mới làm con của weaponHolder
        currentWeapon = Instantiate(weapons[index], weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        currentIndex = index;

        Debug.Log("Đổi sang vũ khí: " + currentWeapon.name);
    }
}
