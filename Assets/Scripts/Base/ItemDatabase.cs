using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoSingleton<ItemDatabase>
{
    private Dictionary<int, ItemData> dict;

    protected override void Init()
    {
        base.Init();

        DontDestroyOnLoad(gameObject);

        dict = new Dictionary<int, ItemData>();

        // 自动加载 Resources/Items 下所有 ItemDefinition
        var allItems = Resources.LoadAll<ItemData>("Items");

        foreach (var item in allItems)
        {
            if (!dict.ContainsKey(item.itemId))
                dict.Add(item.itemId, item);
            else
                Debug.LogWarning($"ItemDatabase: 重复的 itemId = {item.itemId}, 名称: {item.name}");
        }

        Debug.Log($"ItemDatabase 初始化完成，共加载 {dict.Count} 个物品");
    }

    public ItemData Get(int id)
    {
        if (dict.TryGetValue(id, out var def))
            return def;

        Debug.LogError($"ItemDatabase: 未找到物品 id = {id}");
        return null;
    }
}
