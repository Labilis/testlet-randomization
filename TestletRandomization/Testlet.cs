using TestletRandomization.Enums;
using TestletRandomization.Models;

namespace TestletRandomization;

/// <summary>
///     There is a Testlet with a fixed set of 10 items.
///     6 of the items are operational and 4 of them are pretest items.
/// </summary>
/// <remarks>
///     The requirement is that the _order_ of these items should be randomized such that -
///     o The first 2 items are always pretest items selected randomly from the 4 pretest items.
///     o The next 8 items are mix of pretest and operational items ordered randomly from the remaining 8 items.
/// </remarks>
public class Testlet
{
    public string TestletId { get; }
    private List<Item> Items { get; }

    public Testlet(string testletId, List<Item> items)
    {
        if (testletId == null)
            throw new ArgumentNullException(nameof(testletId));

        if (items == null)
            throw new ArgumentNullException(nameof(items));

        if (items.Count != ItemsCountRequired)
            throw new ArgumentException($"Wrong number of items: {items.Count} instead of {items.Count}",
                nameof(items));

        foreach (var validationRule in InputItemsValidationRules)
        {
            var itemsCountByType = items.Count(i => i.ItemType == validationRule.Key);

            if (itemsCountByType != validationRule.Value)
                throw new ArgumentException(
                    $"Wrong number of items of type '{validationRule.Key}': {itemsCountByType} instead of {validationRule.Value}",
                    nameof(items));
        }

        TestletId = testletId;
        Items = items;
    }
    
    public List<Item> Randomize()
    {
        return RandomizeWithPretestFirst(Items, RandomizedPretestItemsCount).ToList();
    }

    private static IEnumerable<Item> RandomizeWithPretestFirst(ICollection<Item> items, int ptetestItemsCount)
    {
        var pretestItems = items
            .Where(i => i.ItemType == ItemType.Pretest)
            .OrderBy(_ => Rnd.Next())
            .Take(ptetestItemsCount).ToList();

        var mixedItems = items
            .Except(pretestItems)
            .OrderBy(_ => Rnd.Next());

        return pretestItems.Union(mixedItems);
    }

    private const int RandomizedPretestItemsCount = 2;

    private const int OperationalItemsCountInitial = 6;
    private const int PretestItemsCountInitial = 4;
    private const int ItemsCountRequired = OperationalItemsCountInitial + PretestItemsCountInitial;

    private static readonly Random Rnd = new();

    private static readonly IDictionary<ItemType, int> InputItemsValidationRules = new Dictionary<ItemType, int>
    {
        { ItemType.Operational, OperationalItemsCountInitial },
        { ItemType.Pretest, PretestItemsCountInitial }
    };
}