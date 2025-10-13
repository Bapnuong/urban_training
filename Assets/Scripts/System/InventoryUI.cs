using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public Transform content;      // chỗ để spawn item UI
    public GameObject itemPrefab;  // prefab ô item (có Text và Image)
    private CharacterData currentChar;

    public void SetCharacter(CharacterData character)
    {
        currentChar = character;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (currentChar == null || currentChar.inventory == null)
        {
            Debug.LogWarning("⚠️ Chưa có nhân vật hoặc inventory trống");
            return;
        }

        // xóa item cũ
        foreach (Transform child in content)
            Destroy(child.gameObject);

        // tạo item mới
        foreach (var item in currentChar.inventory.items)
        {
            GameObject slot = Instantiate(itemPrefab, content);
            slot.transform.Find("ItemName").GetComponent<Text>().text = item.itemName;
            slot.transform.Find("Quantity").GetComponent<Text>().text = "x" + item.quantity;
        }
    }
}
