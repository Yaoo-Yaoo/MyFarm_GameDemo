using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Text priceText;
    [SerializeField] private GameObject bottomPart;

    public void SetupTooltip(ItemDetails itemDetails, SlotType slotType)
    {
        nameText.text = itemDetails.itemName;
        
        typeText.text = GetItemType(itemDetails.itemType);

        descriptionText.text = itemDetails.itemDescription;

        if (itemDetails.itemType == ItemType.Seed || itemDetails.itemType == ItemType.Commodity || itemDetails.itemType == ItemType.Furniture)
        {
            bottomPart.SetActive(true);

            int price = itemDetails.itemPrice;
            if (slotType == SlotType.Bag)
                price = (int)(price * itemDetails.sellPercentage);

            priceText.text = price.ToString();
        }
        else
        {
            bottomPart.SetActive(false);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    private string GetItemType(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Seed:
                return "种子";
            case ItemType.Commodity:
                return "商品";
            case ItemType.Furniture:
                return "家具";
            case ItemType.HoeTool or ItemType.ChopTool or ItemType.BreakTool or ItemType.ReapTool or ItemType.WaterTool or ItemType.CollectTool:
                return "工具";
            default:
                return "无";
        }
    }
}
