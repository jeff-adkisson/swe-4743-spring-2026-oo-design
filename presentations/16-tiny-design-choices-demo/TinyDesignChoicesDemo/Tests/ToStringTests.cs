using TinyDesignChoicesDemo.Models;
using TinyDesignChoicesDemo.ValueObjects;
using Xunit.Abstractions;

namespace TinyDesignChoicesDemo.Tests;

public class ToStringTests
{
    private readonly ITestOutputHelper _out;

    public ToStringTests(ITestOutputHelper output) => _out = output;

    [Fact]
    public void Order_WithoutOverride_ShowsTypeName()
    {
        var order = new OrderWithoutToString { Id = 42, Total = 125m };
        var result = order.ToString();

        _out.WriteLine("OrderWithoutToString has no ToString override.");
        _out.WriteLine($"  Default output: \"{result}\"");
        _out.WriteLine("This is what ends up in your logs — just the type name, no useful info for debugging.");

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

        _out.WriteLine("Order HAS a ToString override.");
        _out.WriteLine($"  Output: \"{result}\"");
        _out.WriteLine("Now a log line can identify the exact order without attaching a debugger.");

        Assert.Contains("Id=42", result);
        Assert.Contains("Status=Pending", result);
        Assert.Contains("2026-04-10", result);
    }

    [Fact]
    public void Order_ToString_WorksInStringInterpolation()
    {
        var order = new Order { Id = 99, Status = "Shipped", Total = 50m };

        var message = $"Processing {order}";

        _out.WriteLine("String interpolation with $\"...{order}...\" calls ToString() automatically.");
        _out.WriteLine($"  Result: \"{message}\"");
        _out.WriteLine("This is why ToString appears in more places than most developers realize:");
        _out.WriteLine("logs, exception messages, test failures, debugger watch windows — all free once you override it.");

        Assert.Contains("Order(", message);
        Assert.Contains("Id=99", message);
    }

    [Fact]
    public void EmailAddress_ToString_ReturnsTheAddress()
    {
        var email = new EmailAddress("alice@example.com");

        _out.WriteLine("Value objects should always override ToString — they appear everywhere in logs.");
        _out.WriteLine($"  $\"{{email}}\" → \"{email}\"");

        Assert.Equal("alice@example.com", $"{email}");
    }

    [Fact]
    public void Money_ToString_InStringInterpolation()
    {
        var price = new Money(9.99m, "USD");

        var label = $"Price: {price}";

        _out.WriteLine($"Constructed: Money(9.99m, \"USD\")");
        _out.WriteLine($"Interpolated into a label: \"{label}\"");
        _out.WriteLine("Currency and amount are always displayed together — no ambiguity in logs.");

        Assert.Equal("Price: 9.99 USD", label);
    }

    [Fact]
    public void Customer_ToString_ShowsIdentityAndState()
    {
        var customer = new Customer { Id = 1, Name = "Alice" };

        _out.WriteLine($"Customer.ToString() = \"{customer}\"");
        _out.WriteLine("Identity (Id) + human-readable state (Name) — enough to pinpoint the object in a log.");

        Assert.Equal("Customer(Id=1, Name=Alice)", customer.ToString());
    }
}
