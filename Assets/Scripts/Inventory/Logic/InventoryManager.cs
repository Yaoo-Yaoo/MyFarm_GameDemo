using UnityEngine;

namespace MyFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("物品数据")]
        public ItemDataList_SO itemDataListSO;

        [Header("背包数据")] 
        public InventoryBag_SO playerBag;

        private void Start()
        {
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 返回编号ID的物品明细
        /// </summary>
        /// <param name="ID">itemID</param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataListSO.itemDetailsList.Find(i => i.itemID == ID);
        }
        
        /// <summary>
        /// 添加物品至 Player 背包
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="toDestroy">是否要销毁物品</param>
        public void AddItem(Item item, bool toDestroy)
        {
            int index = GetItemIndexInBag(item.itemID);
            AddItemAtIndex(item.itemID, index, 1);
            
            // Debug.Log("ID: " + GetItemDetails(item.itemID).itemID + " , Name: " + GetItemDetails(item.itemID).itemName);
            
            if (toDestroy)
                Destroy(item.gameObject);
            
            // 更新 UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        
        /// <summary>
        /// 找到物品在背包中的序号
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <returns>若不在背包中则返回-1</returns>
        private int GetItemIndexInBag(int ID)
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID)
                    return i;
            }
            return -1;
        }
        
        /// <summary>
        /// 检查背包是否有空位
        /// </summary>
        /// <returns></returns>
        private bool CheckBagCapacity()
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == 0)
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// 在背包指定序号添加物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="index">物品在背包中的序号</param>
        /// <param name="amount">物品数量</param>
        private void AddItemAtIndex(int ID, int index, int amount)
        {
            if (index == -1 && CheckBagCapacity())  // 背包不存在该物品且有空位
            {
                InventoryItem item = new InventoryItem { itemID = ID, itemAmount = amount };
                for (int i = 0; i < playerBag.itemList.Count; i++)
                {
                    if (playerBag.itemList[i].itemID == 0)
                    {
                        playerBag.itemList[i] = item;
                        break;
                    }
                }
            }
            else  // 背包存在该物品
            {
                int currentAmount = playerBag.itemList[index].itemAmount + amount;
                InventoryItem item = new InventoryItem { itemID = ID, itemAmount = currentAmount };
                playerBag.itemList[index] = item;
            }
        }
        
        /// <summary>
        /// 交换 Player 背包格子物品
        /// </summary>
        /// <param name="fromIndex">拖拽前格子坐标</param>
        /// <param name="toIndex">拖拽后格子坐标</param>
        public void SwapItem(int fromIndex, int targetIndex)
        {
            InventoryItem fromItem = playerBag.itemList[fromIndex];
            InventoryItem targetItem = playerBag.itemList[targetIndex];

            if (targetItem.itemID != 0)  // 交换
            {
                playerBag.itemList[fromIndex] = targetItem;
                playerBag.itemList[targetIndex] = fromItem;
            }
            else  // 移过去
            {
                playerBag.itemList[targetIndex] = fromItem;
                playerBag.itemList[fromIndex] = new InventoryItem();
            }
            
            // 刷新 UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
    }
}
