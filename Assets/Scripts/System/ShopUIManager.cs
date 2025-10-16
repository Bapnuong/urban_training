using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUIManager : MonoBehaviour
{
    public static ShopUIManager Instance;

    [Header("UI References")]
    public GameObject detailPanel;
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemStats;
    public TextMeshProUGUI itemPrice;

    private ItemData _currentItem;

    void Awake()
    {
        Instance = this;
        detailPanel.SetActive(false);
    }

    public void ShowItemDetails(ItemData data)
    {
        _currentItem = data;
        detailPanel.SetActive(true);

        itemImage.sprite = data.icon;
        itemName.text = data.itemName;
        itemStats.text = data.stats;
        itemPrice.text = "$" + data.price;
    }

    public void OnBuyButton()
    {
        if (_currentItem != null)
        {
            Debug.Log("Bought " + _currentItem.itemName);
        }
    }
}
