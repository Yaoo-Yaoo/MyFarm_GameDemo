using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class ItemEditor : EditorWindow
{
    private ItemDataList_SO dataBase;
    private List<ItemDetails> itemList = new List<ItemDetails>();
    private VisualTreeAsset itemRowTemplate;
    private ListView itemListView;
    private ScrollView itemDetailsSection;
    private ItemDetails activeItem;
    private VisualElement iconPreview;
    private Sprite defaultIcon;

    [MenuItem("My Farm/ItemEditor")]
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UIBuilder/ItemEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);
        
        // 加载 ListView 模板
        itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UIBuilder/ItemRowTemplate.uxml");
        
        // 获取默认图片
        defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/M Studio/Art/Items/Icons/icon_M.png");
        
        // 变量赋值
        itemListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");
        itemDetailsSection = root.Q<ScrollView>("ItemDetails");
        iconPreview = itemDetailsSection.Q<VisualElement>("Icon");
        
        // 按键
        root.Q<Button>("AddButton").clicked += OnAddItem;
        root.Q<Button>("DeleteButton").clicked += OnDeleteItem;
        
        // 加载数据
        LoadDataBase();
        
        // 生成 ListView
        GenerateListView();
    }

    #region 按键事件

    private void OnAddItem()
    {
        ItemDetails newItem = new ItemDetails();
        newItem.itemID = 1001 + itemList.Count;
        newItem.itemName = "NEW ITEM";
        itemList.Add(newItem);
        itemListView.Rebuild();
    }
    
    private void OnDeleteItem()
    {
        itemList.Remove(activeItem);
        itemListView.Rebuild();
        itemDetailsSection.visible = false;
    }

    #endregion
    
    private void LoadDataBase()
    {
        var dataArray = AssetDatabase.FindAssets("ItemDataList_SO");

        if (dataArray.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(dataArray[0]);
            dataBase = AssetDatabase.LoadAssetAtPath<ItemDataList_SO>(path);
            itemList = dataBase.itemDetailsList;
        }
        
        // 如果不标记无法保存数据
        EditorUtility.SetDirty(dataBase);
        
        // Debug.Log(itemList[0].itemName);
    }

    private void GenerateListView()
    {
        Func<VisualElement> makeItem = () => itemRowTemplate.CloneTree();
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            if (i < itemList.Count)
            {
                if (itemList[i].itemIcon != null)
                    e.Q<VisualElement>("Icon").style.backgroundImage = itemList[i].itemIcon.texture;
                e.Q<Label>("Name").text = itemList[i].itemName;
            }
        };
        
        itemListView.fixedItemHeight = 50;
        itemListView.itemsSource = itemList;
        itemListView.makeItem = makeItem;
        itemListView.bindItem = bindItem;
        itemListView.onSelectionChange += OnListSelectionChanged;

        itemDetailsSection.visible = false;
    }

    private void OnListSelectionChanged(IEnumerable<object> selectedItem)
    {
        activeItem = (ItemDetails)selectedItem.First();
        GetItemDetails();
        itemDetailsSection.visible = true;
    }

    private void GetItemDetails()
    {
        itemDetailsSection.MarkDirtyRepaint();  // 可以保存、撤销等
        
        // Item ID
        IntegerField itemIDIntegerField = itemDetailsSection.Q<IntegerField>("ItemID");
        itemIDIntegerField.value = activeItem.itemID;
        itemIDIntegerField.RegisterValueChangedCallback((e) =>
        {
            activeItem.itemID = e.newValue;
        });
        
        // Item Name
        TextField itemNameTextField = itemDetailsSection.Q<TextField>("ItemName");
        itemNameTextField.value = activeItem.itemName;
        itemNameTextField.RegisterValueChangedCallback((e) =>
        {
            activeItem.itemName = e.newValue;
            itemListView.Rebuild();  // 同步刷新左侧的列表
        });
        
        // Item Type
        EnumField itemTypeEnumField = itemDetailsSection.Q<EnumField>("ItemType");
        itemTypeEnumField.Init(activeItem.itemType);
        itemTypeEnumField.value = activeItem.itemType;
        itemTypeEnumField.RegisterValueChangedCallback((e) =>
        {
            activeItem.itemType = (ItemType)e.newValue;
        });
        
        // Item Icon
        iconPreview.style.backgroundImage = activeItem.itemIcon == null ? defaultIcon.texture : activeItem.itemIcon.texture;
        ObjectField itemIconObjectField = itemDetailsSection.Q<ObjectField>("ItemIcon");
        itemIconObjectField.value = activeItem.itemIcon;
        itemIconObjectField.RegisterValueChangedCallback((e) =>
        {
            Sprite newIcon = e.newValue as Sprite;
            activeItem.itemIcon = newIcon;
            iconPreview.style.backgroundImage = newIcon == null ? defaultIcon.texture : newIcon.texture;
            itemListView.Rebuild();
        });
        
        // Item On World Sprite
        ObjectField itemOnWorldSpriteObjectField = itemDetailsSection.Q<ObjectField>("ItemOnWorldSprite");
        itemOnWorldSpriteObjectField.value = activeItem.itemOnWorldSprite;
        itemOnWorldSpriteObjectField.RegisterValueChangedCallback((e) =>
        {
            activeItem.itemOnWorldSprite = e.newValue as Sprite;
        });
        
        // Item Description
        TextField itemDescriptionTextField = itemDetailsSection.Q<TextField>("ItemDescription");
        itemDescriptionTextField.value = activeItem.itemDescription;
        itemDescriptionTextField.RegisterValueChangedCallback((e) =>
        {
            activeItem.itemDescription = e.newValue;
        });
        
        // Item Use Radius
        IntegerField itemUseRadiusIntegerField = itemDetailsSection.Q<IntegerField>("ItemUseRadius");
        itemUseRadiusIntegerField.value = activeItem.itemUseRadius;
        itemUseRadiusIntegerField.RegisterValueChangedCallback((e) =>
        {
            activeItem.itemUseRadius = e.newValue;
        });
        
        // Item Can Pickedup
        Toggle canPickedupToggle = itemDetailsSection.Q<Toggle>("CanPickedup");
        canPickedupToggle.value = activeItem.canPickedup;
        canPickedupToggle.RegisterValueChangedCallback((e) =>
        {
            activeItem.canPickedup = e.newValue;
        });
        
        // Item Can Dropped
        Toggle canDroppedToggle = itemDetailsSection.Q<Toggle>("CanDropped");
        canDroppedToggle.value = activeItem.canDropped;
        canDroppedToggle.RegisterValueChangedCallback((e) =>
        {
            activeItem.canDropped = e.newValue;
        });
        
        // Item Can Carried
        Toggle canCarriedToggle = itemDetailsSection.Q<Toggle>("CanCarried");
        canCarriedToggle.value = activeItem.canCarried;
        canCarriedToggle.RegisterValueChangedCallback((e) =>
        {
            activeItem.canCarried = e.newValue;
        });
        
        // Item Price
        IntegerField itemPriceIntegerField = itemDetailsSection.Q<IntegerField>("ItemPrice");
        itemPriceIntegerField.value = activeItem.itemPrice;
        itemPriceIntegerField.RegisterValueChangedCallback((e) =>
        {
            activeItem.itemPrice = e.newValue;
        });
        
        // Item Sell Percentage
        Slider itemSellPercentageSlider = itemDetailsSection.Q<Slider>("ItemSellPercentage");
        itemSellPercentageSlider.value = activeItem.sellPercentage;
        itemSellPercentageSlider.RegisterValueChangedCallback((e) =>
        {
            activeItem.sellPercentage = e.newValue;
        });
    }
}