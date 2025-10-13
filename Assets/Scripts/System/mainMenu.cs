using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    public Text welcomeText;
    public Text charInfoText;
    public Text messageText;

    private UserData currentUser;

    // ✅ Thêm phần hiển thị nhân vật
    [Header("Player Display")]
    public Transform playerSpawnPoint;  // vị trí spawn trong scene
    public GameObject playerPrefab;     // prefab nhân vật (drag từ Project vào)
    private GameObject currentPlayerObj;
    private GameObject inventoryUI;
    private void Awake()
    {
        inventoryUI = GameObject.FindGameObjectWithTag("InventoryUI");
        if (inventoryUI != null)
            inventoryUI.SetActive(false); // ẩn UI Inventory khi vào Main Menu
    }
    void Start()
    {
        if (!AuthManager.IsLoggedIn())
        {
            SceneManager.LoadScene("LoginScene");
            return;
        }

        LoadUser();
        MainMenuRef.currentUser = currentUser;
        SaveSystem.SaveUser(currentUser);
        RefreshUI();

        // ✅ Hiển thị nhân vật ra màn hình
        ShowSelectedCharacter();
    }

    void LoadUser()
    {
        string user = AuthManager.GetCurrentUsername();
        currentUser = SaveSystem.LoadUser(user);
    }

    void RefreshUI()
    {
        if (currentUser == null) return;
        welcomeText.text = "Chào, " + currentUser.username;

        var sel = currentUser.GetSelectedCharacter();
        if (sel != null)
        {
            charInfoText.text = $"Nhân vật: {sel.characterName}\nLv {sel.level} | EXP {sel.exp} | Coins: {sel.coins}";
        }
        else
        {
            charInfoText.text = "Chưa chọn nhân vật.";
        }
    }

    public void OnOpenCharacterSelect()
    {
        SceneManager.LoadScene("CharacterSelect"); // scene chọn nhân vật
    }

    public void OnLogout()
    {
        AuthManager.Logout();
        SceneManager.LoadScene("LoginScene");
    }

    public void OnDeleteAccount()
    {
        SaveSystem.DeleteUser(currentUser.username);
        AuthManager.Logout();
        SceneManager.LoadScene("LoginScene");
    }

    // example: add EXP to selected character (debug / for gameplay hook)
    public void AddExpToSelected(int amount)
    {
        var sel = currentUser.GetSelectedCharacter();
        if (sel == null) return;
        sel.exp += amount;
        TryLevelUp(sel);
        SaveSystem.SaveUser(currentUser);
        RefreshUI();
        messageText.text = $"Đã cộng {amount} EXP.";

        // ✅ Cập nhật hiển thị nhân vật nếu cần
        ShowSelectedCharacter();
    }

    void TryLevelUp(CharacterData c)
    {
        int need = 100 + (c.level - 1) * 50; // formula: tăng dần
        while (c.exp >= need)
        {
            c.exp -= need;
            c.level++;
            c.coins += 50; // thưởng coins khi lên cấp
            need = 100 + (c.level - 1) * 50;
        }
    }
    public void home()
    {
        if(!inventoryUI.activeSelf)
        {
            inventoryUI.SetActive(true);
        }
        else{
            inventoryUI.SetActive(false);
        }
    }
    // ✅ Hàm hiển thị nhân vật ra Main Menu
    void ShowSelectedCharacter()
    {
        if (playerPrefab == null || playerSpawnPoint == null)
        {
            Debug.LogWarning("⚠️ Chưa gán Player Prefab hoặc Player Spawn Point trong Inspector!");
            return;
        }

        var sel = currentUser.GetSelectedCharacter();
        if (sel == null)
        {
            Debug.Log("❌ Không có nhân vật nào được chọn để hiển thị.");
            return;
        }

        // Xóa nhân vật cũ (nếu có)
        if (currentPlayerObj != null)
            Destroy(currentPlayerObj);

        // Spawn nhân vật mới
        currentPlayerObj = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);

        // Có thể thêm tuỳ chỉnh hiển thị (vd: đặt tên)
        currentPlayerObj.name = sel.characterName;
    }
}
