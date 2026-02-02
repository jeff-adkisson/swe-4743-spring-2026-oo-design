using Assignment2Solution.Domain.Inventory;
using Assignment2Solution.UserInterface.PaymentMethodGenerator;
using Assignment2Solution.UserInterface.Query;

namespace TeaShop.UnitTest.UserInterface;

public class UserInterfaceTests
{
    [Fact]
    public void QueryBuilder_Build_ReturnsConfiguredQuery()
    {
        // Arrange
        var repository = new InventoryRepository();
        // Simulate user input for all prompts
        var inputLines = new[]
        {
            "Green", // Name contains
            "Y", // Is available
            "10", // Price min
            "20", // Price max
            "4", // Rating min
            "5", // Rating max
            "A", // Sort by Price,
            "D" // Sort by Star rating
        };
        var input = new StringReader(string.Join(Environment.NewLine, inputLines));
        var output = new StringWriter();
        var builder = new QueryBuilder(repository, input, output);

        // Act
        var query = builder.Build();

        // Assert
        Assert.NotNull(query);
        var filters = query.AppliedFiltersAndSorts;
        Assert.Contains(filters, f => f.Contains("Green"));
        Assert.Contains(filters, f => f.Contains("In Stock"));
        Assert.Contains(filters, f => f.Contains("$10.00") && f.Contains("$20.00"));
        Assert.Contains(filters, f => f.Contains("Star rating between 4") && f.Contains("5"));
        Assert.Contains(filters, f => f.Contains("Sort: Price (ascending)"));
        Assert.Contains(filters, f => f.Contains("Sort: Star rating (descending)"));
    }

    [Fact]
    public void ApplePayPaymentMethod_CreateStrategy_ReturnsStrategy()
    {
        // Arrange
        var method = new ApplePayPaymentStrategyGenerator();
        var input = new StringReader("user@example.com" + Environment.NewLine);
        var output = new StringWriter();

        // Act
        var strategy = method.CreateStrategy(input, output);

        // Assert
        Assert.NotNull(strategy);
        Assert.Equal("Apple Pay", method.Name);

        // Check output prompt
        Assert.Contains("Enter Apple Username:", output.ToString());
    }

    [Fact]
    public void CreditCardPaymentMethod_CreateStrategy_ReturnsStrategy()
    {
        // Arrange
        var method = new CreditCardPaymentStrategyGenerator();
        var input = new StringReader("1234567890123456" + Environment.NewLine);
        var output = new StringWriter();

        // Act
        var strategy = method.CreateStrategy(input, output);

        // Assert
        Assert.NotNull(strategy);
        Assert.Equal("Credit Card", method.Name);
    }

    [Fact]
    public void CryptoCurrencyPaymentMethod_CreateStrategy_ReturnsStrategy()
    {
        // Arrange
        var method = new CryptoCurrencyPaymentStrategyGenerator();
        var input = new StringReader("0x123" + Environment.NewLine + "sig123" + Environment.NewLine);
        var output = new StringWriter();

        // Act
        var strategy = method.CreateStrategy(input, output);

        // Assert
        Assert.NotNull(strategy);
        Assert.Equal("CryptoCurrency", method.Name);
    }
}