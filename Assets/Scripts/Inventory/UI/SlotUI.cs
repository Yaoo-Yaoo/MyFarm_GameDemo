using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [Header("组件获取")] 
    [SerializeField] private Image slotImage;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image slotHighlight;
    [SerializeField] private Button button;
    
    [Header("格子类型")]
    public SlotType slotType;
    public bool isSelected;

    //物品信息
    public ItemDetails itemDetails;
    public int itemAmount;

    private void Start()
    {
        isSelected = false;

        if (itemDetails.itemID == 0)
            UpdateEmptySlot();
    }

    /// <summary>
    /// 更新格子信息
    /// </summary>
    /// <param name="item">物品信息明细</param>
    /// <param name="amount">物品数量</param>
    public void UpdateSlot(ItemDetails item, int amount)
    {
        itemDetails = item;
        itemAmount = amount;
        
        slotImage.sprite = item.itemIcon;
        slotImage.enabled = true;
        
        amountText.text = amount.ToString();
        
        button.interactable = true;
    }
    
    /// <summary>
    /// 更新空格子信息
    /// </summary>
    public void UpdateEmptySlot()
    {
        if (!isSelected)
            isSelected = false;

        slotImage.enabled = false;
        amountText.text = string.Empty;
        button.interactable = false;
    }
}
