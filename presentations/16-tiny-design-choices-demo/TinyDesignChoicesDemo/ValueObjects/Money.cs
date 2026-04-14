namespace TinyDesignChoicesDemo.ValueObjects;

public sealed record Money(decimal Amount, string Currency)
{
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException(
                $"Cannot add {Currency} to {other.Currency}.");
        return this with { Amount = Amount + other.Amount };
    }

    public override string ToString() => $"{Amount:F2} {Currency}";
}
