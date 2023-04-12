using System.ComponentModel.DataAnnotations;

namespace TestletRandomization;

public class Testlet
{
    public string TestletId;

    private readonly List<Item> Items;

    public Testlet(string testletId, List<Item> items)
    {
        TestletId = testletId;
        Items = items;
    }

    public List<Item> Randomize()
    {
        return Randomize(Items, _randomizationSettings);
    }

    // Better to be extracted ind passed as parameter then.
    // But to not amend the signatures and because of criteria describe only one case it is here as private member
    private readonly Tuple<int, ItemTypeEnum[]>[] _randomizationSettings =
    {
        new(2, new[] { ItemTypeEnum.Pretest }),
        new(8, new[] { ItemTypeEnum.Pretest, ItemTypeEnum.Operational })
    };

    private static readonly Random Rnd = new();
    
    // Can be extracted in
    private static List<Item> Randomize(IEnumerable<Item> items,
        IEnumerable<Tuple<int, ItemTypeEnum[]>> randomizationSettings)
    {
        var itemsGroupedByType = items
            .GroupBy(i => i.ItemType)
            .ToDictionary(i => i.Key, gr => gr.ToList());

        var result = new List<Item>();

        foreach (var sectionSettings in randomizationSettings)
        {
            var requiredItemCount = sectionSettings.Item1;
            var requiredItemTypes = sectionSettings.Item2;

            for (var i = 0; i < requiredItemCount; i++)
            {
                var availableTypes = requiredItemTypes.Intersect(itemsGroupedByType.Keys).ToList();
                if (availableTypes.Count == 0)
                    throw new ValidationException("Not enough items of required types");

                var randomRequiredItemType = availableTypes[Rnd.Next(availableTypes.Count)];
                var itemsByType = itemsGroupedByType[randomRequiredItemType];
                var randomItemIndex = Rnd.Next(itemsByType.Count);
                var randomItem = itemsByType[randomItemIndex];

                result.Add(randomItem);
                itemsByType.RemoveAt(randomItemIndex);
                if (itemsByType.Count == 0)
                    itemsGroupedByType.Remove(randomRequiredItemType);
            }
        }

        return result;
    }
}