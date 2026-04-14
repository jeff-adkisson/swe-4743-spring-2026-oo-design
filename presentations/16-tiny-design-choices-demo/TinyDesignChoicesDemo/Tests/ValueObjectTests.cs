using TinyDesignChoicesDemo.ValueObjects;

namespace TinyDesignChoicesDemo.Tests;

public class ValueObjectTests
{
    [Fact]
    public void EmailAddress_NormalizesToLowerCase()
    {
        var email = new EmailAddress("Alice@Example.COM");
        Assert.Equal("alice@example.com", email.Value);
    }

    [Fact]
    public void EmailAddress_RejectsEmptyString()
    {
        Assert.Throws<ArgumentException>(() => new EmailAddress(""));
    }

    [Fact]
    public void EmailAddress_RejectsMissingAtSign()
    {
        Assert.Throws<ArgumentException>(() => new EmailAddress("not-an-email"));
    }

    [Fact]
    public void EmailAddress_EqualByValue()
    {
        var a = new EmailAddress("alice@example.com");
        var b = new EmailAddress("Alice@Example.COM");
        Assert.Equal(a, b);
        Assert.True(a == b);
    }

    [Fact]
    public void EmailAddress_UnequalForDifferentAddresses()
    {
        var a = new EmailAddress("alice@example.com");
        var b = new EmailAddress("bob@example.com");
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void EmailAddress_ToString_ReturnsValue()
    {
        var email = new EmailAddress("alice@example.com");
        Assert.Equal("alice@example.com", email.ToString());
    }

    [Fact]
    public void Money_AddSameCurrency()
    {
        var a = new Money(10.50m, "USD");
        var b = new Money(4.25m, "USD");
        var result = a.Add(b);

        Assert.Equal(14.75m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Money_CannotAddDifferentCurrencies()
    {
        var usd = new Money(10m, "USD");
        var eur = new Money(5m, "EUR");

        Assert.Throws<InvalidOperationException>(() => usd.Add(eur));
    }

    [Fact]
    public void Money_EqualByValue()
    {
        var a = new Money(10.00m, "USD");
        var b = new Money(10.00m, "USD");
        Assert.Equal(a, b);
    }

    [Fact]
    public void Money_ToString_FormatsCorrectly()
    {
        var money = new Money(42.50m, "EUR");
        Assert.Equal("42.50 EUR", money.ToString());
    }

    [Fact]
    public void DateRange_RejectsStartAfterEnd()
    {
        Assert.Throws<ArgumentException>(() =>
            new DateRange(new DateOnly(2026, 12, 31), new DateOnly(2026, 1, 1)));
    }

    [Fact]
    public void DateRange_ContainsDateWithinRange()
    {
        var range = new DateRange(new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        Assert.True(range.Contains(new DateOnly(2026, 6, 15)));
    }

    [Fact]
    public void DateRange_DoesNotContainDateOutsideRange()
    {
        var range = new DateRange(new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        Assert.False(range.Contains(new DateOnly(2027, 1, 1)));
    }

    [Fact]
    public void DateRange_SafeAsDictionaryKey()
    {
        var dict = new Dictionary<DateRange, string>();
        var range = new DateRange(new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 30));
        dict[range] = "Q1-Q2";

        var lookupRange = new DateRange(new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 30));
        Assert.Equal("Q1-Q2", dict[lookupRange]);
    }
}
