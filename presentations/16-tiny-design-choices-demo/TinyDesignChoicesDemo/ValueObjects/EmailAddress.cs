namespace TinyDesignChoicesDemo.ValueObjects;

public sealed record EmailAddress
{
    public string Value { get; }

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.", nameof(value));
        if (!value.Contains('@'))
            throw new ArgumentException($"'{value}' is not a valid email.", nameof(value));

        Value = value.Trim().ToLowerInvariant();
    }

    public override string ToString() => Value;
}
