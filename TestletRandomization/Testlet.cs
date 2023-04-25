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
    private static readonly Random Rnd = new();
    private const int RandomizedPretestItemsCount = 2;
    private const int InitialOperationalItemsCount = 6;
    private const int InitialPretestItemsCount = 4;
    private const int ItemsCountRequired = InitialOperationalItemsCount + InitialPretestItemsCount;
    
    public string TestletId { get; }
    private readonly List<Item> _items;

    public Testlet(string testletId, List<Item> items)
    {
        if (testletId == null)
            throw new ArgumentNullException(nameof(testletId));

        if (items == null)
            throw new ArgumentNullException(nameof(items));

        if (items.Count != ItemsCountRequired)
            throw new ArgumentException($"Wrong number of items: {items.Count} instead of {items.Count}",
                nameof(items));

        if (items.Count(i => i.ItemType == ItemType.Pretest) != InitialPretestItemsCount)
            throw new ArgumentException(
                $"Wrong number of items of type 'Pretest'. Only {InitialPretestItemsCount} items are allowed.",
                nameof(items));

        if (items.Count(i => i.ItemType == ItemType.Operational) != InitialOperationalItemsCount)
            throw new ArgumentException(
                $"Wrong number of items of type 'Operational'. Only {InitialOperationalItemsCount} items are allowed.",
                nameof(items));

        TestletId = testletId;
        _items = items;
    }

    public List<Item> Randomize()
    {
        return RandomizeWithPretestFirst(_items, RandomizedPretestItemsCount).ToList();
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
}