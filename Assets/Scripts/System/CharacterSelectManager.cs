using UnityEngine;
using TMPro; // thêm dòng này
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class CharacterSelectManager : MonoBehaviour
{
    public Transform listContainer;
    public GameObject charItemPrefab;
    public TMP_InputField newCharNameInput;
    public TMP_Text messageText;

    private UserData currentUser;

    void Start()
    {
        if (!AuthManager.IsLoggedIn()) { SceneManager.LoadScene("LoginScene"); return; }
        currentUser = SaveSystem.LoadUser(AuthManager.GetCurrentUsername());
        PopulateList();
    }

    void PopulateList()
    {
        foreach (Transform t in listContainer) Destroy(t.gameObject);

        foreach (var ch in currentUser.characters)
        {
            GameObject go = Instantiate(charItemPrefab, listContainer);
            TMP_Text txt = go.transform.Find("CharText").GetComponent<TMP_Text>();
            Button selectBtn = go.transform.Find("SelectBtn").GetComponent<Button>();
            Button deleteBtn = go.transform.Find("DeleteBtn").GetComponent<Button>();

            txt.text = $"{ch.characterName}  Lv {ch.level}";
            selectBtn.onClick.AddListener(() => OnSelectCharacter(ch.characterId));
            deleteBtn.onClick.AddListener(() => OnDeleteCharacter(ch.characterId));
        }
    }

    public void OnCreateCharacter()
    {
        string name = newCharNameInput.text.Trim();
        if (string.IsNullOrEmpty(name) || name.Length < 2)
        {
            messageText.text = "Tên nhân vật ít nhất 2 ký tự.";
            return;
        }

        string id = Guid.NewGuid().ToString();
        var newChar = new CharacterData(id, name);
        currentUser.characters.Add(newChar);
        currentUser.lastSelectedCharacterId = id;
        SaveSystem.SaveUser(currentUser);
        PopulateList();
        messageText.text = "Tạo nhân vật thành công.";
    }

    public void OnSelectCharacter(string id)
    {
        currentUser.lastSelectedCharacterId = id;
        SaveSystem.SaveUser(currentUser);
        messageText.text = "Đã chọn nhân vật.";

        // >>> thêm dòng này để quay về menu ngay
        SceneManager.LoadScene("MainMenu");
    }

    public void OnDeleteCharacter(string id)
    {
        var ch = currentUser.GetCharacterById(id);
        if (ch == null) return;

        if (currentUser.characters.Count <= 1)
        {
            messageText.text = "Không thể xóa nhân vật cuối cùng.";
            return;
        }

        currentUser.characters.Remove(ch);
        if (currentUser.lastSelectedCharacterId == id)
            currentUser.lastSelectedCharacterId = currentUser.characters[0].characterId;

        SaveSystem.SaveUser(currentUser);
        PopulateList();
        messageText.text = "Đã xóa nhân vật.";
    }

    public void OnBackToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
