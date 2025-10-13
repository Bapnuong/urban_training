using System.IO;
using UnityEngine;

public static class SaveLoadManager
{
    private static string savePath = Application.persistentDataPath + "/userdata.json";

    public static void SaveUser(UserData user)
    {
        string json = JsonUtility.ToJson(user, true);
        File.WriteAllText(savePath, json);
        Debug.Log("✅ Dữ liệu đã lưu: " + savePath);
    }

    public static UserData LoadUser()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("⚠️ Chưa có file lưu, tạo mới user mặc định.");
            return new UserData("Guest", "123");
        }

        string json = File.ReadAllText(savePath);
        UserData loaded = JsonUtility.FromJson<UserData>(json);
        Debug.Log("📂 Dữ liệu đã load từ file: " + savePath);
        return loaded;
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("🗑️ Đã xóa file save");
        }
    }
}
