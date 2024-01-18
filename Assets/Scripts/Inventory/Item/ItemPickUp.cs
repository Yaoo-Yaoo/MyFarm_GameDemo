using System;
using UnityEngine;

namespace MyFarm.Inventory
{
    public class ItemPickUp : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            Item item = other.GetComponent<Item>();
            if (item != null)
            {
                if (item.itemDetails.canPickedup)
                {
                    // 拾取物品并添加到背包
                    InventoryManager.Instance.AddItem(item, true);
                }
            }
        }
    }
}
