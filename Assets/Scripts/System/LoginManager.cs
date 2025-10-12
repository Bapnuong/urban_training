using UnityEngine;
using TMPro; // thêm dòng này
using UnityEngine.SceneManagement;
using System;

public class LoginManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text messageText;

    // lightweight obfuscation (not secure) - base64-like
    private string Obf(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        var bytes = System.Text.Encoding.UTF8.GetBytes(s);
        return System.Convert.ToBase64String(bytes);
    }

    private string Deobf(string o)
    {
        try
        {
            var bytes = System.Convert.FromBase64String(o);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        catch { return ""; }
    }

    public void OnRegister()
    {
        string user = usernameInput.text.Trim();
        string pass = passwordInput.text;

        if (user.Length < 3 || pass.Length < 3)
        {
            messageText.text = "Tên và mật khẩu phải >= 3 ký tự.";
            return;
        }

        if (SaveSystem.UserExists(user))
        {
            messageText.text = "Tài khoản đã tồn tại.";
            return;
        }

        string obf = Obf(pass);
        UserData u = new UserData(user, obf);

        // tạo nhân vật mặc định
        var starter = new CharacterData(Guid.NewGuid().ToString(), "Tân binh");
        u.characters.Add(starter);
        u.lastSelectedCharacterId = starter.characterId;

        SaveSystem.SaveUser(u);
        messageText.text = "Đăng ký thành công! Bạn có 1 nhân vật khởi tạo.";
    }

    public void OnLogin()
    {
        string user = usernameInput.text.Trim();
        string pass = passwordInput.text;

        if (!SaveSystem.UserExists(user))
        {
            messageText.text = "Không tìm thấy tài khoản.";
            return;
        }

        UserData u = SaveSystem.LoadUser(user);
        if (u == null)
        {
            messageText.text = "Lỗi đọc dữ liệu.";
            return;
        }

        string stored = Deobf(u.passwordObf);
        if (stored == pass)
        {
            messageText.text = "Đăng nhập thành công!";
            AuthManager.SetCurrentUser(user);
            SceneManager.LoadScene("MainMenu"); // Đặt đúng tên scene
        }
        else
        {
            messageText.text = "Sai mật khẩu.";
        }
    }
}
