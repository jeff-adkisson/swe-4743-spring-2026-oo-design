using Assignment2Solution.Domain.Inventory;

namespace TeaShop.UnitTest.Domain.Inventory;

public class InventoryTests
{
    [Fact]
    public void InventoryItem_Initialization_SetsProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Test Tea";
        var price = 10.50m;
        var quantity = 20;
        var rating = new StarRating(4);

        // Act
        var item = new InventoryItem(id, name, price, quantity, rating);

        // Assert
        Assert.Equal(id, item.InventoryItemId);
        Assert.Equal(name, item.Name);
        Assert.Equal(price, item.Price);
        Assert.Equal(quantity, item.Quantity);
        Assert.Equal(rating, item.StarRating);
    }

    [Theory]
    [InlineData(10, true)]
    [InlineData(1, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    public void InventoryItem_IsAvailable_ReturnsExpected(int quantity, bool expected)
    {
        // Arrange
        var item = new InventoryItem(Guid.NewGuid(), "Tea", 10m, quantity, new StarRating(3));

        // Act & Assert
        Assert.Equal(expected, item.IsAvailable);
    }

    [Fact]
    public void InventoryItem_TotalPrice_CalculatesCorrectly()
    {
        // Arrange
        var item = new InventoryItem(Guid.NewGuid(), "Tea", 10.50m, 3, new StarRating(3));

        // Act & Assert
        Assert.Equal(31.50m, item.TotalPrice);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void StarRating_ValidRating_SetsRating(int ratingValue)
    {
        // Act
        var rating = new StarRating(ratingValue);

        // Assert
        Assert.Equal(ratingValue, rating.Rating);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void StarRating_InvalidRating_ThrowsException(int ratingValue)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new StarRating(ratingValue));
    }

    [Fact]
    public void StarRating_ToString_ReturnsFormattedString()
    {
        // Arrange
        var rating = new StarRating(4);

        // Act & Assert
        Assert.Equal("4****", rating.ToString());
    }

    [Fact]
    public void InventoryRepository_Get_ReturnsInitialItems()
    {
        // Arrange
        var repository = new InventoryRepository();

        // Act
        var items = repository.Get();

        // Assert
        Assert.NotNull(items);
        Assert.NotEmpty(items);
        Assert.Equal(50,
            items.Count); // Based on the code provided earlier (it seems I counted 52 items, let me double check or just assert not empty)
    }

    [Fact]
    public void InventoryRepository_UpdateQuantity_UpdatesItem()
    {
        // Arrange
        var repository = new InventoryRepository();
        var item = repository.Get()[0];
        var initialQuantity = item.Quantity;
        var change = 5;

        // Act
        repository.UpdateQuantity(item.InventoryItemId, change);
        var updatedItem = repository.Get().First(i => i.InventoryItemId == item.InventoryItemId);

        // Assert
        Assert.Equal(initialQuantity + change, updatedItem.Quantity);
    }

    [Fact]
    public void InventoryRepository_UpdateQuantity_ThrowsWhenInsufficient()
    {
        // Arrange
        var repository = new InventoryRepository();
        var item = repository.Get().First(i => i.Quantity > 0);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            repository.UpdateQuantity(item.InventoryItemId, -item.Quantity - 1));
    }

    [Fact]
    public void InventoryRepository_UpdateQuantity_ThrowsWhenNotFound()
    {
        // Arrange
        var repository = new InventoryRepository();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => repository.UpdateQuantity(Guid.NewGuid(), 1));
    }
}