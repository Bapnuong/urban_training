using UnityEngine;

public static class AuthManager
{
    private const string CURRENT_USER_KEY = "currentUser";

    public static bool IsLoggedIn()
    {
        return PlayerPrefs.HasKey(CURRENT_USER_KEY) && !string.IsNullOrEmpty(PlayerPrefs.GetString(CURRENT_USER_KEY));
    }

    public static string GetCurrentUsername()
    {
        return PlayerPrefs.GetString(CURRENT_USER_KEY, "");
    }

    public static void SetCurrentUser(string username)
    {
        PlayerPrefs.SetString(CURRENT_USER_KEY, username);
        PlayerPrefs.Save();
    }

    public static void Logout()
    {
        PlayerPrefs.DeleteKey(CURRENT_USER_KEY);
        PlayerPrefs.Save();
    }
}
