using Assignment2Solution.Domain.Inventory;

namespace Assignment2Solution.Domain.InventoryQuery.Filters;

/// <summary>
///     A filter that filters inventory items by their availability.
/// </summary>
public sealed class AvailabilityFilterDecorator : InventoryQueryDecoratorBase
{
    private readonly bool? _isAvailable;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AvailabilityFilterDecorator" /> class.
    /// </summary>
    /// <param name="inner">The inner query to decorate.</param>
    /// <param name="isAvailable">
    ///     True to filter for in-stock items, false for out-of-stock, or null for no availability
    ///     filtering.
    /// </param>
    public AvailabilityFilterDecorator(IInventoryQuery inner, bool? isAvailable) : base(inner)
    {
        _isAvailable = isAvailable;
    }

    /// <inheritdoc />
    protected override string? AppliedDescription
    {
        get
        {
            if (_isAvailable is null) return null;
            return _isAvailable.Value
                ? "Filter: Availability = In Stock (Quantity > 0)"
                : "Filter: Availability = Out of Stock (Quantity = 0)";
        }
    }

    /// <inheritdoc />
    protected override IReadOnlyList<InventoryItem> Decorate(IReadOnlyList<InventoryItem> items)
    {
        if (_isAvailable is null) return items;

        var filtered = _isAvailable.Value ? items.Where(i => i.IsAvailable) : items.Where(i => !i.IsAvailable);
        return filtered.ToList().AsReadOnly();
    }
}