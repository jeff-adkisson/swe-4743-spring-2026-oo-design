namespace TinyDesignChoicesDemo.ValueObjects;

public sealed record DateRange
{
    public DateOnly Start { get; }
    public DateOnly End { get; }

    public DateRange(DateOnly start, DateOnly end)
    {
        if (start > end)
            throw new ArgumentException(
                $"Start ({start}) must not be after End ({end}).");
        Start = start;
        End = end;
    }

    public bool Contains(DateOnly date) => date >= Start && date <= End;

    public override string ToString() => $"{Start} to {End}";
}
