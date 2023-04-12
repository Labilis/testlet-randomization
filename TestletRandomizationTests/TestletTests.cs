using TestletRandomization;
using Xunit;

namespace TestletRandomizationTests;

public class TestletTests
{
    private const int NumberOrRandomizationRechecks = 1000;

    /// <summary>
    ///     Test Case described in Assessment
    /// </summary>
    /// <remarks>
    ///     There is a Testlet with a fixed set of 10 items. 6 of the items are operational and 4 of them are pretest items.
    ///     The requirement is that the _order_ of these items should be randomized such that -
    ///     o The first 2 items are always pretest items selected randomly from the 4 pretest items.
    ///     o The next 8 items are mix of pretest and operational items ordered randomly from the remaining 8 items.
    /// </remarks>
    [Fact]
    public void Randomize_WhenItemsPassed_ThenRandomizedAsPerRequirement()
    {
        //Arrange
        var items = ArrangeItems(new[]
        {
            new KeyValuePair<ItemTypeEnum, int>(ItemTypeEnum.Operational, 6),
            new KeyValuePair<ItemTypeEnum, int>(ItemTypeEnum.Pretest, 4)
        }).ToList();

        for (var launchIndex = 0; launchIndex < NumberOrRandomizationRechecks; launchIndex++)
        {
            //Act
            var testlet = new Testlet("test-id", items.OrderBy(_ => Guid.NewGuid()).ToList());
            var result = testlet.Randomize();

            //Assert
            Assert.Equivalent(items, result);

            var section1 = result.Take(2).ToList();
            Assert.True(section1.All(i => i.ItemType == ItemTypeEnum.Pretest));

            var section2 = result.Skip(2).Take(8).ToList();
            Assert.Contains(section2, i => i.ItemType == ItemTypeEnum.Pretest);
            Assert.Contains(section2, i => i.ItemType == ItemTypeEnum.Operational);
        }
    }

    private static IEnumerable<Item> ArrangeItems(IEnumerable<KeyValuePair<ItemTypeEnum, int>> countByType)
    {
        var itemId = 1;
        foreach (var keyValuePair in countByType)
            for (var i = 0; i < keyValuePair.Value; i++)
                yield return new Item
                {
                    ItemId = $"item-id-{keyValuePair.Key}-{itemId++}",
                    ItemType = keyValuePair.Key
                };
    }
}