using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject[] playerPrefabs;

    void Start()
    {
        var user = SaveSystem.LoadUser(AuthManager.GetCurrentUsername());
        if (user == null || user.characters == null || user.characters.Count == 0)
        {
            Debug.LogError("User/Characters null");
            return;
        }

        var sel = user.GetSelectedCharacter(); // dựa vào lastSelectedCharacterId
        if (sel == null) sel = user.characters[0];

        // Tìm index theo thứ tự characters
        int index = user.characters.FindIndex(c => c.characterId == sel.characterId);
        if (index < 0) index = 0;

        // Clamp theo mảng prefab
        if (playerPrefabs == null || playerPrefabs.Length == 0)
        {
            Debug.LogError("playerPrefabs rỗng!");
            return;
        }
        if (index >= playerPrefabs.Length) index = 0;

        Instantiate(playerPrefabs[index],
                    spawnPoint ? spawnPoint.position : Vector3.zero,
                    spawnPoint ? spawnPoint.rotation : Quaternion.identity);
    }
}
