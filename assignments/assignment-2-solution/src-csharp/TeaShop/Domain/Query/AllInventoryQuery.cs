using Assignment2Solution.Domain.Inventory;

namespace Assignment2Solution.Domain.Query;

/// <summary>
///     The base query that retrieves all items from the inventory repository.
/// </summary>
/// <param name="repository">The inventory repository to query.</param>
public sealed class AllInventoryQuery(InventoryRepository repository) : IInventoryQuery
{
    private readonly InventoryRepository
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    /// <inheritdoc />
    public IReadOnlyList<InventoryItem> Execute()
    {
        return _repository.Get();
    }

    /// <inheritdoc />
    public IReadOnlyList<string> AppliedFiltersAndSorts { get; } = Array.Empty<string>();
}