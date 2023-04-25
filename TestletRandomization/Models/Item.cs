using TestletRandomization.Enums;

namespace TestletRandomization.Models;

public class Item
{
    public Item(string itemId, ItemType itemType)
    {
        ItemId = itemId;
        ItemType = itemType;
    }

    public string ItemId { get; }
    public ItemType ItemType { get; }
}