using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyFarm.Inventory
{
    public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("组件获取")] 
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amountText;
        public Image slotHighlight;
        [SerializeField] private Button button;
        
        [Header("格子类型")]
        public SlotType slotType;
        public bool isSelected;
    
        //物品信息
        public ItemDetails itemDetails;
        public int itemAmount;
        public int slotIndex;

        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();
        
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

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemAmount == 0)
                return;

            isSelected = !isSelected;
            
            inventoryUI.UpdateHighlight(slotIndex);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemAmount != 0)
            {
                inventoryUI.dragItem.enabled = true;
                inventoryUI.dragItem.sprite = slotImage.sprite;
                inventoryUI.dragItem.SetNativeSize();

                isSelected = true;
                inventoryUI.UpdateHighlight(slotIndex);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.enabled = false;
            // Debug.Log(eventData.pointerCurrentRaycast.gameObject);

            if (eventData.pointerCurrentRaycast.gameObject != null)  // UI 格子交换
            {
                SlotUI targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                if (targetSlot == null)
                    return;
                
                // 在 Player 自身背包范围内交换
                if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(slotIndex, targetSlot.slotIndex);
                }
                
                // 清除所有高亮
                inventoryUI.UpdateHighlight(-1);
            }
            // else  // 测试：扔到世界场景中
            // {
            //     if (itemDetails.canDropped)
            //     {
            //         // 鼠标对应的世界坐标
            //         Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            //
            //         EventHandler.CallInstantiateItemInScene(itemDetails.itemID, pos);
            //     }
            // }
        }
    }
}
