using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public ItemData itemData; 

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnItemClick);
    }

    void OnItemClick()
    {
        ShopUIManager.Instance.ShowItemDetails(itemData);
    }
}
