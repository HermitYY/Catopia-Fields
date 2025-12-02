using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/World/Crop")]
public class CropData : ScriptableObject
{
    public int cropId;
    public Sprite[] stageSprites;
    public float[] stageDurations;
    public List<InventoryItem> harvestDrops;
}
