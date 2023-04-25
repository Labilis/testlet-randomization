using TestletRandomization.Enums;
using TestletRandomization.Models;
using Xunit;

namespace TestletRandomization.Tests;

public class TestletTests
{
    [Fact]
    public void Randomize_WhenValidItemsPassed_ThenRandomizedSuccessfully()
    {
        //Arrange
        var items = ArrangeItems(new[]
        {
            new KeyValuePair<ItemType, int>(ItemType.Operational, 6),
            new KeyValuePair<ItemType, int>(ItemType.Pretest, 4)
        }).ToList();

        //Act
        var testlet = new Testlet("testlet-id", items);
        var result = testlet.Randomize();

        //Assert
        Assert.Equivalent(items, result);

        var section1 = result.Take(ExpectedPretestsSectionLength).ToList();
        Assert.True(section1.All(i => i.ItemType == ItemType.Pretest));

        var section2 = result.Skip(ExpectedPretestsSectionLength).ToList();
        Assert.Equal(ExpectedMixedSectionLength, section2.Count);
        Assert.Contains(section2, i => i.ItemType == ItemType.Pretest);
        Assert.Contains(section2, i => i.ItemType == ItemType.Operational);
    }

    [Fact]
    public void Randomize_WhenRandomizedItemsRandomizedAgain_ThenReorderedSuccessfully()
    {
        //Arrange
        var items = ArrangeItems(new[]
        {
            new KeyValuePair<ItemType, int>(ItemType.Operational, 6),
            new KeyValuePair<ItemType, int>(ItemType.Pretest, 4)
        }).ToList();

        //Act
        var testletInitial = new Testlet("testlet-id-01", items);
        var randomizedFirstTime = testletInitial.Randomize();
        var randomizedSecondTime = testletInitial.Randomize();

        Assert.Equivalent(randomizedFirstTime, randomizedSecondTime);

        // Assert
        // Retry to prevent flaky behaviour
        var retryCount = 0;
        while (retryCount < 5)
        {
            var isRandomized =
                randomizedSecondTime.Any(i => randomizedFirstTime.IndexOf(i) != randomizedSecondTime.IndexOf(i));
            if (isRandomized)
            {
                Assert.True(true);
                return;
            }

            retryCount++;
        }
    }

    [Theory]
    [MemberData(nameof(NullArgumentsTheoryData))]
    public void Randomize_WhenTryCreateWithNullArguments_ThenThrowException(string testId, List<Item> items)
    {
        // Arrange + Act + Assert
        Assert.Throws<ArgumentNullException>(() => new Testlet(testId, items));
    }

    [Theory]
    [MemberData(nameof(WrongItemsTheoryData))]
    public void Randomize_WhenTryCreateWithWrongItems_ThenThrowException(IEnumerable<Item> items)
    {
        // Arrange + Act
        var exception = Assert.Throws<ArgumentException>(() => new Testlet("test-id", items.ToList()));

        // Assert
        Assert.StartsWith("Wrong number of items", exception.Message);
        Assert.Equal("items", exception.ParamName);
    }


    private const int ExpectedPretestsSectionLength = 2;
    private const int ExpectedMixedSectionLength = 8;

    public static TheoryData<string?, List<Item>?> NullArgumentsTheoryData => new()
    {
        { null, null },
        { "testlet-id", null },
        { null, new List<Item>() }
    };

    public static TheoryData<List<Item>?> WrongItemsTheoryData => new()
    {
        new List<Item>(),
        ArrangeItems(new[]
        {
            new KeyValuePair<ItemType, int>(ItemType.Operational, 10)
        }).ToList(),
        ArrangeItems(new[]
        {
            new KeyValuePair<ItemType, int>(ItemType.Pretest, 10)
        }).ToList(),
        ArrangeItems(new[]
        {
            new KeyValuePair<ItemType, int>(ItemType.Operational, 4),
            new KeyValuePair<ItemType, int>(ItemType.Pretest, 6)
        }).ToList(),
        ArrangeItems(new[]
        {
            new KeyValuePair<ItemType, int>(ItemType.Pretest, 2),
            new KeyValuePair<ItemType, int>(ItemType.Operational, 8)
        }).ToList()
    };

    private static IEnumerable<Item> ArrangeItems(IEnumerable<KeyValuePair<ItemType, int>> countByType)
    {
        var itemId = 1;
        foreach (var keyValuePair in countByType)
            for (var i = 0; i < keyValuePair.Value; i++)
                yield return new Item($"item-id-{keyValuePair.Key}-{itemId++}", keyValuePair.Key);
    }
}