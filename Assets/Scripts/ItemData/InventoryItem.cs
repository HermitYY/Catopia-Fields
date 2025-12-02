using System;

[Serializable]
public class InventoryItem
{
    public ItemData data;
    public int stackSize;

    public InventoryItem(ItemData data, int initialAmount = 1)
    {
        this.data = data;
        this.stackSize = initialAmount;
    }

    public void AddStack(int amount = 1)
    {
        stackSize += amount;
    }

    public void RemoveStack(int amount = 1)
    {
        stackSize -= amount;
        if (stackSize < 0) stackSize = 0;
    }
}
