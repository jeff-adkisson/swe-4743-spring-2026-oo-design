using Assignment2Solution.Domain.Inventory;

namespace Assignment2Solution.Domain.InventoryQuery.Filters;

/// <summary>
///     A filter that filters inventory items by checking if their name contains a substring.
/// </summary>
public sealed class NameContainsFilterDecorator : InventoryQueryDecoratorBase
{
    private readonly string _substring;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NameContainsFilterDecorator" /> class.
    /// </summary>
    /// <param name="inner">The inner query to decorate.</param>
    /// <param name="substring">The substring to search for in the item names.</param>
    /// <exception cref="ArgumentNullException">Thrown when substring is null.</exception>
    public NameContainsFilterDecorator(IInventoryQuery inner, string? substring) : base(inner)
    {
        _substring = substring ?? throw new ArgumentNullException(nameof(substring));
    }

    /// <inheritdoc />
    protected override string? AppliedDescription
        => string.IsNullOrWhiteSpace(_substring) ? null : $"Filter: Name contains \"{_substring}\"";

    /// <inheritdoc />
    protected override IReadOnlyList<InventoryItem> Decorate(IReadOnlyList<InventoryItem> items)
    {
        if (string.IsNullOrWhiteSpace(_substring))
            return items;

        return items.Where(i =>
                i.Name.Contains(_substring, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();
    }
}