using Assignment2Solution.Domain.Inventory;

namespace Assignment2Solution.Domain.Query.Sorts;

/// <summary>
///     A decorator that sorts inventory items by star rating.
/// </summary>
public sealed class SortByStarRatingDecorator : InventoryQueryDecoratorBase
{
    private readonly SortDirection _direction;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SortByStarRatingDecorator" /> class.
    /// </summary>
    /// <param name="inner">The inner query to decorate.</param>
    /// <param name="direction">The sort direction.</param>
    public SortByStarRatingDecorator(IInventoryQuery inner, SortDirection direction = SortDirection.Ascending)
        : base(inner)
    {
        _direction = direction;
    }

    /// <inheritdoc />
    protected override string? AppliedDescription
        => $"Sort: Star rating ({_direction.ToString().ToLowerInvariant()})";

    /// <inheritdoc />
    public override IReadOnlyList<InventoryItem> Execute()
    {
        var items = Inner.Execute();

        var sorted = _direction == SortDirection.Ascending
            ? items.OrderBy(i => i.StarRating.Rating)
            : items.OrderByDescending(i => i.StarRating.Rating);

        return sorted.ToList().AsReadOnly();
    }
}