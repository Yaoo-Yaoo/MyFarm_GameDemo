using System.Collections.Generic;
using UnityEngine;

public class AnimatorOverride : MonoBehaviour
{
    private Animator[] animators;
    public SpriteRenderer holdItem;

    public List<AnimatorType> animatorTypes;

    private Dictionary<string, Animator> animatorNameDict = new Dictionary<string, Animator>();

    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();

        foreach (Animator anim in animators)
        {
            animatorNameDict.Add(anim.name, anim);
        }
    }

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        holdItem.enabled = false;
        SwitchAnimator(PartType.None);
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        PartType currentPartType = PartType.None;

        if (itemDetails != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Seed or ItemType.Commodity:
                    currentPartType = PartType.Carry;
                    break;
                case ItemType.HoeTool:
                    currentPartType = PartType.Hoe;
                    break;
                case ItemType.WaterTool:
                    currentPartType = PartType.Water;
                    break;
                default:
                    break;
            }
        }

        if (!isSelected)
        {
            currentPartType = PartType.None;
            holdItem.enabled = false;
        }
        else
        {
            if (currentPartType == PartType.Carry)
            {
                holdItem.sprite = itemDetails.itemOnWorldSprite == null ? itemDetails.itemIcon : itemDetails.itemOnWorldSprite;
                holdItem.enabled = true;
            }
        }

        SwitchAnimator(currentPartType);
    }

    private void SwitchAnimator(PartType partType)
    {
        foreach (AnimatorType item in animatorTypes)
        {
            if (item.partType == partType)
            {
                animatorNameDict[item.partName.ToString()].runtimeAnimatorController = item.overrideController;
            }
        }
    }
}
