using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string GetPath(string username)
    {
        return Path.Combine(Application.persistentDataPath, username + ".json");
    }

    public static void SaveUser(UserData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(data.username), json);
        Debug.Log("[SaveSystem] Saved user: " + GetPath(data.username));
    }

    public static UserData LoadUser(string username)
    {
        string path = GetPath(username);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<UserData>(json);
        }
        return null;
    }

    public static bool UserExists(string username)
    {
        return File.Exists(GetPath(username));
    }

    public static void DeleteUser(string username)
    {
        string path = GetPath(username);
        if (File.Exists(path)) File.Delete(path);
    }
}
