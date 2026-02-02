using Assignment2Solution.Domain.Inventory;

namespace Assignment2Solution.Domain.Query.Sorts;

/// <summary>
///     A decorator that sorts inventory items by price.
/// </summary>
public sealed class SortByPriceDecorator : InventoryQueryDecoratorBase
{
    private readonly SortDirection _direction;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SortByPriceDecorator" /> class.
    /// </summary>
    /// <param name="inner">The inner query to decorate.</param>
    /// <param name="direction">The sort direction.</param>
    public SortByPriceDecorator(IInventoryQuery inner, SortDirection direction = SortDirection.Ascending)
        : base(inner)
    {
        _direction = direction;
    }

    /// <inheritdoc />
    protected override string? AppliedDescription
        => $"Sort: Price ({_direction.ToString().ToLowerInvariant()})";

    /// <inheritdoc />
    public override IReadOnlyList<InventoryItem> Execute()
    {
        var items = Inner.Execute();

        var sorted = _direction == SortDirection.Ascending
            ? items.OrderBy(i => i.Price)
            : items.OrderByDescending(i => i.Price);

        return sorted.ToList().AsReadOnly();
    }
}