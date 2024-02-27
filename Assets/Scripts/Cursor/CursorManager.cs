using MyFarm.Map;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, item, tool, seed;
    private Sprite currentSprite;
    private Image cursorImage;
    private RectTransform cursorCanvas;

    // 鼠标检测
    private Camera mainCamera;
    private Grid currentGrid;

    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;
    private bool cursorEnabled;
    private bool cursorPositionValid;
    private ItemDetails currentItem;
    private Transform playerTrans => FindObjectOfType<Player>().transform;

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void Start()
    {
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        currentSprite = normal;
        SetCursorImage(normal);

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (cursorCanvas == null)
            return;

        cursorImage.transform.position = Input.mousePosition;

        if (!InteractWithUI() && cursorEnabled)
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();
            CheckPlayerInput();
        }
        else
        {
            SetCursorImage(normal);
        }
    }

    private void CheckPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionValid)
        {
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnabled = false;
    }

    private void OnAfterSceneLoadedEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
    }

    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = Color.white;
    }

    private void SetCursorValid()
    {
        cursorPositionValid = true;
        cursorImage.color = Color.white;
    }

    private void SetCursorInValid()
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.4f);
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (!isSelected)
        {
            currentSprite = normal;
            cursorEnabled = false;
        }
        else
        {
            currentItem = itemDetails;
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    currentSprite = seed;
                    break;
                case ItemType.Commodity:
                    currentSprite = item;
                    break;
                case ItemType.Furniture or ItemType.BreakTool or ItemType.HoeTool or ItemType.ChopTool or ItemType.ReapTool or ItemType.WaterTool:
                    currentSprite = tool;
                    break;
                default:
                    currentSprite = normal;
                    break;
            }
            cursorEnabled = true;
        }
    }

    private void CheckCursorValid()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        // Debug.Log("WorldPos:" + mouseWorldPos + "    GridPos:" + mouseGridPos);

        var playerGridPos = currentGrid.WorldToCell(playerTrans.position);

        // 判断在使用范围内
        if (Mathf.Abs(playerGridPos.x - mouseGridPos.x) > currentItem.itemUseRadius
            || Mathf.Abs(playerGridPos.y - mouseGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInValid();
            return;
        }

        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);

        if (currentTile != null)
        {
            switch (currentItem.itemType)
            {
                case ItemType.Commodity:
                    if (currentTile.canDropItem && currentItem.canDropped)
                        SetCursorValid();
                    else
                        SetCursorInValid();
                    break;
            }
        }
        else
        {
            SetCursorInValid();
        }
    }

    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;
        return false;
    }
}
