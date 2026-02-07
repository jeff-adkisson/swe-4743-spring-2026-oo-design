using Assignment2Solution.Domain.Inventory;

namespace Assignment2Solution.Domain.Query.Filters;

/// <summary>
///     A filter that filters inventory items by a price range.
/// </summary>
public sealed class PriceRangeFilterDecorator : InventoryQueryDecoratorBase
{
    private readonly decimal? _max;
    private readonly decimal? _min;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PriceRangeFilterDecorator" /> class.
    /// </summary>
    /// <param name="inner">The inner query to decorate.</param>
    /// <param name="minInclusive">The minimum price (inclusive), or null for no minimum.</param>
    /// <param name="maxInclusive">The maximum price (inclusive), or null for no maximum.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when min or max is negative.</exception>
    /// <exception cref="ArgumentException">Thrown when min is greater than max.</exception>
    public PriceRangeFilterDecorator(IInventoryQuery inner, decimal? minInclusive, decimal? maxInclusive) : base(inner)
    {
        if (minInclusive is not null && minInclusive < 0) throw new ArgumentOutOfRangeException(nameof(minInclusive));
        if (maxInclusive is not null && maxInclusive < 0) throw new ArgumentOutOfRangeException(nameof(maxInclusive));
        if (minInclusive is not null && maxInclusive is not null && minInclusive > maxInclusive)
            throw new ArgumentException("minInclusive cannot be greater than maxInclusive.");

        _min = minInclusive;
        _max = maxInclusive;
    }

    /// <inheritdoc />
    protected override string? AppliedDescription
    {
        get
        {
            if (_min is null && _max is null) return null;
            if (_min is not null && _max is not null)
                return $"Filter: Price between {_min.Value:C} and {_max.Value:C} (inclusive)";
            if (_min is not null)
                return $"Filter: Price >= {_min.Value:C}";
            return $"Filter: Price <= {_max!.Value:C}";
        }
    }

    /// <inheritdoc />
    protected override IReadOnlyList<InventoryItem> Decorate(IReadOnlyList<InventoryItem> items)
    {
        IEnumerable<InventoryItem> filtered = items;

        if (_min is not null) filtered = filtered.Where(i => i.Price >= _min.Value);
        if (_max is not null) filtered = filtered.Where(i => i.Price <= _max.Value);

        return filtered.ToList().AsReadOnly();
    }
}