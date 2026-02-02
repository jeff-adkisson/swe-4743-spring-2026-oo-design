using Assignment2Solution.Domain.Query;

namespace Assignment2Solution.UserInterface.Query;

/// <summary>
///     Represents the result of an inventory query, ready for display.
/// </summary>
/// <param name="Items">The list of queried inventory items.</param>
/// <param name="AppliedFiltersAndSorts">The list of filters and sorts applied to the query.</param>
public sealed record QueryOutput(
    IReadOnlyList<QueriedInventoryItem> Items,
    IReadOnlyList<string> AppliedFiltersAndSorts)
{
    /// <summary>
    ///     Creates a <see cref="QueryOutput" /> from an <see cref="IInventoryQuery" />.
    /// </summary>
    /// <param name="query">The query to execute.</param>
    /// <returns>A new query output.</returns>
    /// <exception cref="ArgumentNullException">Thrown when query is null.</exception>
    public static QueryOutput From(IInventoryQuery query)
    {
        if (query is null) throw new ArgumentNullException(nameof(query));

        // query.Execute() already returns an IReadOnlyList, 
        // but we still need to wrap them into QueriedInventoryItem with index.
        var items = query.Execute()
            .Select((item, index) => new QueriedInventoryItem(
                index + 1,
                item))
            .ToList()
            .AsReadOnly();
        var applied = query.AppliedFiltersAndSorts.ToList().AsReadOnly();

        return new QueryOutput(items, applied);
    }
}