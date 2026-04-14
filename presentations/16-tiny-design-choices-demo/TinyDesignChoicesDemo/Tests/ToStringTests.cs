using TinyDesignChoicesDemo.Models;
using TinyDesignChoicesDemo.ValueObjects;

namespace TinyDesignChoicesDemo.Tests;

public class ToStringTests
{
    [Fact]
    public void Order_WithoutOverride_ShowsTypeName()
    {
        var order = new OrderWithoutToString { Id = 42, Total = 125m };
        var result = order.ToString();

        // Default ToString returns only the type name — useless for debugging
        Assert.Equal("TinyDesignChoicesDemo.Models.OrderWithoutToString", result);
    }

    [Fact]
    public void Order_WithOverride_ShowsUsefulInfo()
    {
        var order = new Order
        {
            Id = 42,
            Status = "Pending",
            Total = 125m,
            CreatedAt = new DateTime(2026, 4, 10)
        };

        var result = order.ToString();

        Assert.Contains("Id=42", result);
        Assert.Contains("Status=Pending", result);
        Assert.Contains("2026-04-10", result);
    }

    [Fact]
    public void Order_ToString_WorksInStringInterpolation()
    {
        var order = new Order { Id = 99, Status = "Shipped", Total = 50m };

        var message = $"Processing {order}";

        Assert.Contains("Order(", message);
        Assert.Contains("Id=99", message);
    }

    [Fact]
    public void EmailAddress_ToString_ReturnsTheAddress()
    {
        var email = new EmailAddress("alice@example.com");

        Assert.Equal("alice@example.com", $"{email}");
    }

    [Fact]
    public void Money_ToString_InStringInterpolation()
    {
        var price = new Money(9.99m, "USD");

        var label = $"Price: {price}";

        Assert.Equal("Price: 9.99 USD", label);
    }

    [Fact]
    public void Customer_ToString_ShowsIdentityAndState()
    {
        var customer = new Customer { Id = 1, Name = "Alice" };

        Assert.Equal("Customer(Id=1, Name=Alice)", customer.ToString());
    }
}
