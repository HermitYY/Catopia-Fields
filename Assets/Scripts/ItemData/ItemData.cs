using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Seed,       // 种子
    Crop,       // 作物
    Food,       // 食物
    Tool,       // 工具
    Weapon,     // 武器
    MonsterDrop,// 怪物掉落物
    Furniture,  // 家具
    Ground      // 地块
}


public abstract class ItemData : ScriptableObject
{
    [Header("BaseInfo")]
    public int itemId;
    public ItemType itemType;
    public string itemName;
    public Sprite showIcon;

    [Header("Materials")]
    public List<InventoryItem> craftingMaterials;
}
