using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryBag", menuName = "Inventory/InventoryBag")]
public class InventoryBag_SO : ScriptableObject
{
    public List<InventoryItem> itemList;
}
