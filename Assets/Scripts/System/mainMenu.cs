using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class mainMenu : MonoBehaviour
{
    [Header("UI")]
    public Text welcomeText;
    public Text charInfoText;
    public Text messageText;

    private UserData currentUser;

    [Header("Player Display")]
    public Transform playerSpawnPoint;
    public GameObject[] playerPrefabs;

    private GameObject currentPlayerObj;
    private int currentIndex = 0;   // index của character đang preview

    private GameObject inventoryUI;

    private void Awake()
    {
        inventoryUI = GameObject.FindGameObjectWithTag("InventoryUI");
        if (inventoryUI != null) inventoryUI.SetActive(false);
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

        // ✅ Tìm index từ lastSelectedCharacterId (string)
        currentIndex = GetSelectedIndexFromUser(currentUser);
        ShowCharacterByIndex(currentIndex);
    }

    void LoadUser()
    {
        string user = AuthManager.GetCurrentUsername();
        currentUser = SaveSystem.LoadUser(user);
        if (currentUser == null) Debug.LogError("LoadUser fail");
    }

    void RefreshUI()
    {
        if (currentUser == null) return;

        if (welcomeText) welcomeText.text = "Chào, " + currentUser.username;

        var sel = currentUser.GetSelectedCharacter();
        if (charInfoText)
        {
            if (sel != null)
                charInfoText.text = $"Nhân vật: {sel.characterName}\nLv {sel.level} | EXP {sel.exp} | Coins: {sel.coins}";
            else
                charInfoText.text = "Chưa chọn nhân vật.";
        }
    }

    // ======= Điều hướng trái/phải =======

    public void OnPrevCharacter()
    {
        if (!HasPrefabs()) return;
        currentIndex = (currentIndex - 1 + playerPrefabs.Length) % playerPrefabs.Length;
        ShowCharacterByIndex(currentIndex);
    }

    public void OnNextCharacter()
    {
        if (!HasPrefabs()) return;
        currentIndex = (currentIndex + 1) % playerPrefabs.Length;
        ShowCharacterByIndex(currentIndex);
    }

    // ======= Xác nhận chọn nhân vật =======

    public void OnConfirmCharacter()
    {
        if (currentUser == null || currentUser.characters == null || currentUser.characters.Count == 0) return;

        // ✅ Lưu bằng string ID
        currentUser.lastSelectedCharacterId = currentUser.characters[currentIndex].characterId;
        SaveSystem.SaveUser(currentUser);

        if (messageText)
            messageText.text = $"✅ Đã chọn: {currentUser.characters[currentIndex].characterName} (ID: {currentUser.lastSelectedCharacterId})";

        RefreshUI();
    }

    // ======= Vào gameplay =======

    public void gamescene()
    {
        SaveSystem.SaveUser(currentUser);
        SceneManager.LoadScene("gamescene");
    }

    public void OnOpenCharacterSelect() => SceneManager.LoadScene("CharacterSelect");

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

    public void home()
    {
        if (inventoryUI) inventoryUI.SetActive(!inventoryUI.activeSelf);
    }

    // ======= EXP demo =======

    public void AddExpToSelected(int amount)
    {
        var sel = currentUser?.GetSelectedCharacter();
        if (sel == null) return;

        sel.exp += amount;
        TryLevelUp(sel);
        SaveSystem.SaveUser(currentUser);
        RefreshUI();

        if (messageText) messageText.text = $"Đã cộng {amount} EXP.";

        // Giữ preview đang xem
        ShowCharacterByIndex(currentIndex);
    }

    void TryLevelUp(CharacterData c)
    {
        int need = 100 + (c.level - 1) * 50;
        while (c.exp >= need)
        {
            c.exp -= need;
            c.level++;
            c.coins += 50;
            need = 100 + (c.level - 1) * 50;
        }
    }

    // ======= Hiển thị preview =======

    void ShowCharacterByIndex(int index)
    {
        if (playerSpawnPoint == null) { Debug.LogWarning("Chưa gán spawn point"); return; }
        if (!HasPrefabs()) return;

        index = Mathf.Clamp(index, 0, playerPrefabs.Length - 1);
        currentIndex = index;

        if (currentPlayerObj) Destroy(currentPlayerObj);

        var prefab = playerPrefabs[index];
        if (!prefab) { Debug.LogWarning($"Prefab {index} null"); return; }

        currentPlayerObj = Instantiate(prefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
        currentPlayerObj.name = $"Preview_{prefab.name}";

        var rb = currentPlayerObj.GetComponent<Rigidbody>(); if (rb) rb.isKinematic = true;
        var col = currentPlayerObj.GetComponent<Collider>(); if (col) col.enabled = false;

        // (tuỳ) quay mặt về camera
        var cam = Camera.main;
        if (cam)
        {
            var lookPos = new Vector3(cam.transform.position.x, currentPlayerObj.transform.position.y, cam.transform.position.z);
            currentPlayerObj.transform.LookAt(lookPos);
        }

        // (tuỳ) cập nhật text preview
        if (charInfoText && currentUser?.characters != null && currentUser.characters.Count > index)
        {
            var ch = currentUser.characters[index];
            charInfoText.text = $"Preview: {ch.characterName} (ID: {ch.characterId})";
        }
    }

    // ======= Helpers =======

    int GetSelectedIndexFromUser(UserData user)
    {
        if (user == null || user.characters == null || user.characters.Count == 0) return 0;

        // đảm bảo lastSelectedCharacterId có giá trị hợp lệ
        if (string.IsNullOrEmpty(user.lastSelectedCharacterId) ||
            !user.characters.Any(c => c.characterId == user.lastSelectedCharacterId))
        {
            user.lastSelectedCharacterId = user.characters[0].characterId;
        }

        int idx = user.characters.FindIndex(c => c.characterId == user.lastSelectedCharacterId);
        if (idx < 0) idx = 0;

        // clamp theo số prefab (phòng khi characters > prefabs)
        if (playerPrefabs != null && playerPrefabs.Length > 0)
            idx = Mathf.Clamp(idx, 0, playerPrefabs.Length - 1);

        return idx;
    }

    bool HasPrefabs()
    {
        if (playerPrefabs == null || playerPrefabs.Length == 0)
        {
            Debug.LogWarning("Danh sách playerPrefabs trống!");
            return false;
        }
        return true;
    }
}
