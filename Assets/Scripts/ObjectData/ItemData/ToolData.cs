using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item/Tool")]
public class ToolData : ItemData
{
    public int durability;  // 耐久
    public int power;       // 攻击或效率
}
