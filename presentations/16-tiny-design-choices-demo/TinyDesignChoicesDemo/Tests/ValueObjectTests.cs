using TinyDesignChoicesDemo.ValueObjects;
using Xunit.Abstractions;

namespace TinyDesignChoicesDemo.Tests;

public class ValueObjectTests
{
    private readonly ITestOutputHelper _out;

    public ValueObjectTests(ITestOutputHelper output) => _out = output;

    [Fact]
    public void EmailAddress_NormalizesToLowerCase()
    {
        _out.WriteLine("Value objects can transform input during validation, not just reject it.");
        var email = new EmailAddress("Alice@Example.COM");
        _out.WriteLine($"  Input:  \"Alice@Example.COM\"");
        _out.WriteLine($"  Stored: \"{email.Value}\"");
        _out.WriteLine("Because Value is normalized at construction, every downstream comparison is consistent.");
        Assert.Equal("alice@example.com", email.Value);
    }

    [Fact]
    public void EmailAddress_RejectsEmptyString()
    {
        _out.WriteLine("Attempting to construct EmailAddress(\"\") — expecting ArgumentException.");
        var ex = Assert.Throws<ArgumentException>(() => new EmailAddress(""));
        _out.WriteLine($"  Threw: {ex.GetType().Name}: {ex.Message}");
        _out.WriteLine("An invalid EmailAddress cannot exist. The type system guarantees validity.");
    }

    [Fact]
    public void EmailAddress_RejectsMissingAtSign()
    {
        _out.WriteLine("Attempting to construct EmailAddress(\"not-an-email\") — expecting ArgumentException.");
        var ex = Assert.Throws<ArgumentException>(() => new EmailAddress("not-an-email"));
        _out.WriteLine($"  Threw: {ex.GetType().Name}: {ex.Message}");
    }

    [Fact]
    public void EmailAddress_EqualByValue()
    {
        var a = new EmailAddress("alice@example.com");
        var b = new EmailAddress("Alice@Example.COM");

        _out.WriteLine($"Created two separate EmailAddress instances:");
        _out.WriteLine($"  a = {a}   (object hash: {System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(a)})");
        _out.WriteLine($"  b = {b}   (object hash: {System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(b)})");
        _out.WriteLine($"Different object references, but records compare by value:");
        _out.WriteLine($"  a.Equals(b) = {a.Equals(b)}");
        _out.WriteLine($"  a == b      = {a == b}");

        Assert.Equal(a, b);
        Assert.True(a == b);
    }

    [Fact]
    public void EmailAddress_UnequalForDifferentAddresses()
    {
        var a = new EmailAddress("alice@example.com");
        var b = new EmailAddress("bob@example.com");

        _out.WriteLine($"a = {a}");
        _out.WriteLine($"b = {b}");
        _out.WriteLine($"a == b? {a == b}   (expected: False — different values)");
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void EmailAddress_ToString_ReturnsValue()
    {
        var email = new EmailAddress("alice@example.com");
        _out.WriteLine($"Without ToString override, logs would show: MyApp.ValueObjects.EmailAddress");
        _out.WriteLine($"With our override, logs show:               {email}");
        Assert.Equal("alice@example.com", email.ToString());
    }

    [Fact]
    public void Money_AddSameCurrency()
    {
        var a = new Money(10.50m, "USD");
        var b = new Money(4.25m, "USD");
        var result = a.Add(b);

        _out.WriteLine($"  {a}");
        _out.WriteLine($"+ {b}");
        _out.WriteLine($"= {result}");

        Assert.Equal(14.75m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Money_CannotAddDifferentCurrencies()
    {
        var usd = new Money(10m, "USD");
        var eur = new Money(5m, "EUR");

        _out.WriteLine($"Attempting to add {usd} + {eur} — expecting InvalidOperationException.");
        var ex = Assert.Throws<InvalidOperationException>(() => usd.Add(eur));
        _out.WriteLine($"  Threw: {ex.GetType().Name}: {ex.Message}");
        _out.WriteLine("The invariant is enforced by the type, not by every caller.");
    }

    [Fact]
    public void Money_EqualByValue()
    {
        var a = new Money(10.00m, "USD");
        var b = new Money(10.00m, "USD");
        _out.WriteLine($"a = {a}");
        _out.WriteLine($"b = {b}");
        _out.WriteLine($"a.Equals(b) = {a.Equals(b)}   (records compare by value)");
        Assert.Equal(a, b);
    }

    [Fact]
    public void Money_ToString_FormatsCorrectly()
    {
        var money = new Money(42.50m, "EUR");
        _out.WriteLine($"Money.ToString() → \"{money}\"");
        Assert.Equal("42.50 EUR", money.ToString());
    }

    [Fact]
    public void DateRange_RejectsStartAfterEnd()
    {
        _out.WriteLine("Attempting DateRange(2026-12-31, 2026-01-01) — start after end — expecting ArgumentException.");
        var ex = Assert.Throws<ArgumentException>(() =>
            new DateRange(new DateOnly(2026, 12, 31), new DateOnly(2026, 1, 1)));
        _out.WriteLine($"  Threw: {ex.GetType().Name}: {ex.Message}");
        _out.WriteLine("The invariant (Start <= End) is enforced at construction — no invalid range ever exists.");
    }

    [Fact]
    public void DateRange_ContainsDateWithinRange()
    {
        var range = new DateRange(new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        var probe = new DateOnly(2026, 6, 15);
        _out.WriteLine($"Range: {range}");
        _out.WriteLine($"Contains {probe}? {range.Contains(probe)}");
        Assert.True(range.Contains(probe));
    }

    [Fact]
    public void DateRange_DoesNotContainDateOutsideRange()
    {
        var range = new DateRange(new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        var probe = new DateOnly(2027, 1, 1);
        _out.WriteLine($"Range: {range}");
        _out.WriteLine($"Contains {probe}? {range.Contains(probe)}");
        Assert.False(range.Contains(probe));
    }

    [Fact]
    public void DateRange_SafeAsDictionaryKey()
    {
        var dict = new Dictionary<DateRange, string>();
        var range = new DateRange(new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 30));
        dict[range] = "Q1-Q2";

        var lookupRange = new DateRange(new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 30));

        _out.WriteLine($"Stored key: {range}                (hash: {range.GetHashCode()})");
        _out.WriteLine($"Lookup key: {lookupRange}          (hash: {lookupRange.GetHashCode()})");
        _out.WriteLine($"Different object reference, same value → same hash → dictionary hit.");
        _out.WriteLine($"dict[lookupRange] = \"{dict[lookupRange]}\"");

        Assert.Equal("Q1-Q2", dict[lookupRange]);
    }
}
