using System;
using System.Collections.Generic;

[Serializable]
public class CharacterData
{
    public string characterId;      // id internal
    public string characterName;    // tên hiển thị
    public int level;
    public int exp;
    public int health;
    public int coins;

    public CharacterData(string id, string name)
    {
        characterId = id;
        characterName = name;
        level = 1;
        exp = 0;
        coins = 0;
    }
}

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

    // convenience
    public CharacterData GetSelectedCharacter()
    {
        if (string.IsNullOrEmpty(lastSelectedCharacterId)) return null;
        foreach (var c in characters)
            if (c.characterId == lastSelectedCharacterId) return c;
        return null;
    }

    public CharacterData GetCharacterById(string id)
    {
        return characters.Find(c => c.characterId == id);
    }
}
