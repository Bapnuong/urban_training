using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class mainMenu : MonoBehaviour
{
    public Text welcomeText;
    public Text charInfoText;
    public Text messageText;

    private UserData currentUser;

    void Start()
    {
        if (!AuthManager.IsLoggedIn())
        {
            SceneManager.LoadScene("LoginScene");
            return;
        }

        LoadUser();
        RefreshUI();
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
            charInfoText.text = $"Nhân vật: {sel.characterName}\nLv {sel.level} | EXP {sel.exp}\nHP {sel.health} | Coins: {sel.coins}";
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
}
