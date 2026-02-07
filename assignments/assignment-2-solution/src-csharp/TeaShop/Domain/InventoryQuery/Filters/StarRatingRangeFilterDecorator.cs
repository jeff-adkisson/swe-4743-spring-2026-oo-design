using Assignment2Solution.Domain.Inventory;

namespace Assignment2Solution.Domain.InventoryQuery.Filters;

/// <summary>
///     A filter that filters inventory items by a range of star ratings.
/// </summary>
public sealed class StarRatingRangeFilterDecorator : InventoryQueryDecoratorBase
{
    private readonly int? _max;
    private readonly int? _min;

    /// <summary>
    ///     Initializes a new instance of the <see cref="StarRatingRangeFilterDecorator" /> class.
    /// </summary>
    /// <param name="inner">The inner query to decorate.</param>
    /// <param name="minInclusive">The minimum star rating (inclusive), or null for no minimum.</param>
    /// <param name="maxInclusive">The maximum star rating (inclusive), or null for no maximum.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when ratings are not between 1 and 5.</exception>
    /// <exception cref="ArgumentException">Thrown when min is greater than max.</exception>
    public StarRatingRangeFilterDecorator(IInventoryQuery inner, int? minInclusive, int? maxInclusive) : base(inner)
    {
        if (minInclusive is not null && (minInclusive < 1 || minInclusive > 5))
            throw new ArgumentOutOfRangeException(nameof(minInclusive), "Rating must be between 1 and 5");
        if (maxInclusive is not null && (maxInclusive < 1 || maxInclusive > 5))
            throw new ArgumentOutOfRangeException(nameof(maxInclusive), "Rating must be between 1 and 5");
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
                return $"Filter: Star rating between {_min.Value} and {_max.Value} (inclusive)";
            if (_min is not null)
                return $"Filter: Star rating >= {_min.Value}";
            return $"Filter: Star rating <= {_max!.Value}";
        }
    }

    /// <inheritdoc />
    protected override IReadOnlyList<InventoryItem> Decorate(IReadOnlyList<InventoryItem> items)
    {
        IEnumerable<InventoryItem> filtered = items;

        if (_min is not null) filtered = filtered.Where(i => i.StarRating.Rating >= _min.Value);
        if (_max is not null) filtered = filtered.Where(i => i.StarRating.Rating <= _max.Value);

        return filtered.ToList().AsReadOnly();
    }
}