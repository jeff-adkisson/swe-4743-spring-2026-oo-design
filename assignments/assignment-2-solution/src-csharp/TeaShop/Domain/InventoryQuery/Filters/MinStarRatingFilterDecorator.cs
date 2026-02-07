using Assignment2Solution.Domain.Inventory;

namespace Assignment2Solution.Domain.InventoryQuery.Filters;

/// <summary>
///     A filter that filters inventory items by a minimum star rating.
/// </summary>
public sealed class MinStarRatingFilterDecorator : InventoryQueryDecoratorBase
{
    private readonly int _minRating;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MinStarRatingFilterDecorator" /> class.
    /// </summary>
    /// <param name="inner">The inner query to decorate.</param>
    /// <param name="minRatingInclusive">The minimum star rating (1-5).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when rating is not between 1 and 5.</exception>
    public MinStarRatingFilterDecorator(IInventoryQuery inner, int minRatingInclusive) : base(inner)
    {
        if (minRatingInclusive < 1 || minRatingInclusive > 5)
            throw new ArgumentOutOfRangeException(nameof(minRatingInclusive), "Rating must be between 1 and 5");

        _minRating = minRatingInclusive;
    }

    /// <inheritdoc />
    protected override string? AppliedDescription
        => $"Filter: Star rating >= {_minRating}";

    /// <inheritdoc />
    protected override IReadOnlyList<InventoryItem> Decorate(IReadOnlyList<InventoryItem> items)
    {
        return items.Where(i => i.StarRating.Rating >= _minRating).ToList().AsReadOnly();
    }
}