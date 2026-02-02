using Assignment2Solution.Domain.Inventory;

namespace Assignment2Solution.Domain.Query;

/// <summary>
///     Defines a query for retrieving and filtering inventory items.
/// </summary>
public interface IInventoryQuery
{
    /// <summary>
    ///     Gets a list of descriptions for the filters and sorts applied to this query.
    /// </summary>
    IReadOnlyList<string> AppliedFiltersAndSorts { get; }

    /// <summary>
    ///     Executes the query and returns the filtered and sorted items.
    /// </summary>
    /// <returns>A read-only list of inventory items.</returns>
    IReadOnlyList<InventoryItem> Execute();
}