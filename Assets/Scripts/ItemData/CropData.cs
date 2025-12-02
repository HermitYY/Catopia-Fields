using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item/Crop")]
public class CropData : ItemData
{
    [Header("stage")]
    public Sprite[] stageSprites;
    public float[] stageDurations;  // 秒
    public bool destroyOnHarvest = true;
    public bool revertOnHarvest = false;
    public int revertStageIndex = 0;

    [Header("gift")]
    public List<InventoryItem> harvestDrops;
}
