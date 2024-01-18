using UnityEngine;

namespace MyFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        public ItemDataList_SO itemDataListSO;
        
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
            Debug.Log("ID: " + GetItemDetails(item.itemID).itemID + " , Name: " + GetItemDetails(item.itemID).itemName);
            
            if (toDestroy)
                Destroy(item.gameObject);
        }
    }
}
