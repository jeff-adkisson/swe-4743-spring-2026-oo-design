using Assignment2Solution.Domain.Inventory;
using Assignment2Solution.Domain.PaymentStrategy;

namespace TeaShop.UnitTest.Domain.PaymentStrategy;

public class PaymentStrategyTests
{
    private readonly InventoryItem _testItem = new(Guid.NewGuid(), "Green Tea", 15.00m, 50, new StarRating(4));

    [Fact]
    public void ApplePayStrategy_Checkout_WritesToOutput()
    {
        // Arrange
        var strategy = new ApplePayStrategy("user@example.com");
        using var writer = new StringWriter();

        // Act
        strategy.Checkout(_testItem, 2, writer);

        // Assert
        var output = writer.ToString();
        Assert.Contains("Apple Pay", output);
        Assert.Contains("user@example.com", output);
        Assert.Contains("$30.00", output);
    }

    [Fact]
    public void ApplePayStrategy_EmptyUsername_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ApplePayStrategy(""));
    }

    [Fact]
    public void CreditCardStrategy_Checkout_WritesToOutput()
    {
        // Arrange
        var strategy = new CreditCardStrategy("1234567890123456");
        using var writer = new StringWriter();

        // Act
        strategy.Checkout(_testItem, 2, writer);

        // Assert
        var output = writer.ToString();
        Assert.Contains("Credit Card", output);
        Assert.Contains("1234567890123456", output);
        Assert.Contains("$30.00", output);
    }

    [Fact]
    public void CryptoCurrencyStrategy_Checkout_WritesToOutput()
    {
        // Arrange
        var strategy = new CryptoCurrencyStrategy("0x123abc456def", "sig123");
        using var writer = new StringWriter();

        // Act
        strategy.Checkout(_testItem, 2, writer);

        // Assert
        var output = writer.ToString();
        Assert.Contains("Crypto", output);
        Assert.Contains("0x123abc456def", output);
        Assert.Contains("sig123", output);
        Assert.Contains("$30.00", output);
    }
}