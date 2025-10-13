using System;
using System.Collections.Generic;

// ==================== CLASS ITEM ====================
[Serializable]
public class ItemData
{
    public string itemId;     // Mã định danh
    public string itemName;   // Tên hiển thị
    public int quantity;      // Số lượng
    public string itemType;   // Loại: "weapon", "armor", "potion", ...

    public ItemData(string id, string name, int qty, string type)
    {
        itemId = id;
        itemName = name;
        quantity = qty;
        itemType = type;
    }
}

// ==================== CLASS INVENTORY ====================
[Serializable]
public class Inventory
{
    public List<ItemData> items = new List<ItemData>();

    // Thêm item vào túi
    public void AddItem(string id, string name, int qty = 1, string type = "misc")
    {
        ItemData existing = items.Find(i => i.itemId == id);
        if (existing != null)
        {
            existing.quantity += qty;
        }
        else
        {
            items.Add(new ItemData(id, name, qty, type));
        }
    }

    // Xóa item (toàn bộ)
    public void RemoveItem(string id)
    {
        items.RemoveAll(i => i.itemId == id);
    }

    // Giảm số lượng
    public void UseItem(string id, int qty = 1)
    {
        ItemData existing = items.Find(i => i.itemId == id);
        if (existing != null)
        {
            existing.quantity -= qty;
            if (existing.quantity <= 0)
                items.Remove(existing);
        }
    }

    // Kiểm tra có item hay không
    public bool HasItem(string id)
    {
        return items.Exists(i => i.itemId == id);
    }

    // Lấy item
    public ItemData GetItem(string id)
    {
        return items.Find(i => i.itemId == id);
    }
}

// ==================== CLASS CHARACTER ====================
[Serializable]
public class CharacterData
{
    public string characterId;      // id internal
    public string characterName;    // tên hiển thị
    public int level;
    public int exp;
    public int health;
    public int coins;

    public Inventory inventory;     // 🧳 Thêm inventory cho nhân vật

    public CharacterData(string id, string name)
    {
        characterId = id;
        characterName = name;
        level = 1;
        exp = 0;
        coins = 0;
        inventory = new Inventory(); // khởi tạo inventory riêng
    }
}

// ==================== CLASS USER ====================
[Serializable]
public class UserData
{
    public string username;
    public string passwordObf; // obfuscated password
    public List<CharacterData> characters;
    public string lastSelectedCharacterId;

    public UserData(string username, string passwordObf)
    {
        this.username = username;
        this.passwordObf = passwordObf;
        characters = new List<CharacterData>();
        lastSelectedCharacterId = "";
    }

    public CharacterData GetSelectedCharacter()
    {
        if (characters == null || characters.Count == 0)
            return null;

        if (string.IsNullOrEmpty(lastSelectedCharacterId) ||
            !characters.Exists(c => c.characterId == lastSelectedCharacterId))
        {
            lastSelectedCharacterId = characters[0].characterId;
        }

        return characters.Find(c => c.characterId == lastSelectedCharacterId);
    }

    public CharacterData GetCharacterById(string id)
    {
        return characters.Find(c => c.characterId == id);
    }
}
