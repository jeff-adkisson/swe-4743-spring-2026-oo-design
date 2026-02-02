using Assignment2Solution.Domain.Inventory;
using Assignment2Solution.Domain.Query;
using Assignment2Solution.Domain.Query.Filters;
using Assignment2Solution.Domain.Query.Sorts;
using Moq;

namespace TeaShop.UnitTest.Domain.Query;

public class QueryTests
{
    private readonly List<InventoryItem> _testItems;

    public QueryTests()
    {
        _testItems = new List<InventoryItem>
        {
            new(Guid.NewGuid(), "Green Tea", 15.00m, 10, new StarRating(4)),
            new(Guid.NewGuid(), "Black Tea", 10.00m, 0, new StarRating(5)),
            new(Guid.NewGuid(), "Oolong Tea", 20.00m, 5, new StarRating(3)),
            new(Guid.NewGuid(), "Matcha", 30.00m, 2, new StarRating(5))
        };
    }

    private Mock<IInventoryQuery> GetMockQuery()
    {
        var mock = new Mock<IInventoryQuery>();
        mock.Setup(q => q.Execute()).Returns(_testItems.AsReadOnly());
        mock.Setup(q => q.AppliedFiltersAndSorts).Returns(new List<string>().AsReadOnly());
        return mock;
    }

    [Fact]
    public void AllInventoryQuery_Execute_ReturnsAllItems()
    {
        // Arrange
        var repository = new InventoryRepository();
        var query = new AllInventoryQuery(repository);

        // Act
        var result = query.Execute();

        // Assert
        Assert.Equal(repository.Get().Count, result.Count);
    }

    [Theory]
    [InlineData(true, 3)]
    [InlineData(false, 1)]
    [InlineData(null, 4)]
    public void AvailabilityFilter_Execute_FiltersCorrectly(bool? isAvailable, int expectedCount)
    {
        // Arrange
        var mockQuery = GetMockQuery();
        var filter = new AvailabilityFilterDecorator(mockQuery.Object, isAvailable);

        // Act
        var result = filter.Execute();

        // Assert
        Assert.Equal(expectedCount, result.Count);
    }

    [Fact]
    public void PriceRangeFilter_Execute_FiltersCorrectly()
    {
        // Arrange
        var mockQuery = GetMockQuery();
        var filter = new PriceRangeFilterDecorator(mockQuery.Object, 12.00m, 25.00m);

        // Act
        var result = filter.Execute();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, i => Assert.True(i.Price >= 12.00m && i.Price <= 25.00m));
    }

    [Fact]
    public void NameContainsFilter_Execute_FiltersCorrectly()
    {
        // Arrange
        var mockQuery = GetMockQuery();
        var filter = new NameContainsFilterDecorator(mockQuery.Object, "Green");

        // Act
        var result = filter.Execute();

        // Assert
        Assert.Single(result);
        Assert.Equal("Green Tea", result[0].Name);
    }

    [Fact]
    public void SortByPrice_Execute_SortsDescendingCorrectly()
    {
        // Arrange
        var mockQuery = GetMockQuery();
        var sort = new SortByPriceDecorator(mockQuery.Object, SortDirection.Descending);

        // Act
        var result = sort.Execute();

        // Assert
        Assert.Equal(30.00m, result[0].Price);
        Assert.Equal(20.00m, result[1].Price);
        Assert.Equal(15.00m, result[2].Price);
        Assert.Equal(10.00m, result[3].Price);
    }

    [Fact]
    public void CombinedFiltersAndSorts_WorksCorrectly()
    {
        // Arrange
        var mockQuery = GetMockQuery();
        var availabilityFilter = new AvailabilityFilterDecorator(mockQuery.Object, true);
        var priceFilter = new PriceRangeFilterDecorator(availabilityFilter, 10.00m, 20.00m);
        var sort = new SortByPriceDecorator(priceFilter);

        // Act
        var result = sort.Execute();

        // Assert
        Assert.Equal(2, result.Count); // Green Tea (15), Oolong Tea (20)
        Assert.Equal("Green Tea", result[0].Name);
        Assert.Equal("Oolong Tea", result[1].Name);

        Assert.Equal(3, sort.AppliedFiltersAndSorts.Count);
    }
}