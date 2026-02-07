using Assignment2Solution.Domain.Inventory;

namespace Assignment2Solution.Domain.Query;

/// <summary>
///     Base class for decorators that add filtering or sorting behavior to an <see cref="IInventoryQuery" />.
/// </summary>
public abstract class InventoryQueryDecoratorBase : IInventoryQuery
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="InventoryQueryDecoratorBase" /> class.
    /// </summary>
    /// <param name="inner">The inner query to decorate.</param>
    protected InventoryQueryDecoratorBase(IInventoryQuery inner)
    {
        Inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    /// <summary>
    ///     Gets the inner query being decorated.
    /// </summary>
    private IInventoryQuery Inner { get; }

    /// <summary>
    ///     Gets the description of the filter or sort applied by this decorator.
    /// </summary>
    protected virtual string? AppliedDescription => null;

    /// <inheritdoc />
    public IReadOnlyList<InventoryItem> Execute()
    {
        var items = Inner.Execute();
        return Decorate(items);
    }

    /// <summary>
    ///     Applies the decorator's specific logic (filtering or sorting) to the results of the inner query.
    /// </summary>
    /// <param name="items">The items returned by the inner query.</param>
    /// <returns>the decorated (filtered or sorted) list of items.</returns>
    protected abstract IReadOnlyList<InventoryItem> Decorate(IReadOnlyList<InventoryItem> items);

    /// <inheritdoc />
    public IReadOnlyList<string> AppliedFiltersAndSorts
    {
        get
        {
            if (AppliedDescription is null)
                return Inner.AppliedFiltersAndSorts;

            var combined = new List<string>(Inner.AppliedFiltersAndSorts.Count + 1);
            combined.AddRange(Inner.AppliedFiltersAndSorts);
            combined.Add(AppliedDescription);
            return combined.AsReadOnly();
        }
    }
}