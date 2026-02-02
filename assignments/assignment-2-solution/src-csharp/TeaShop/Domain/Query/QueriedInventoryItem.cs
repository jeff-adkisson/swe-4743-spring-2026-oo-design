using Assignment2Solution.Domain.Inventory;

namespace Assignment2Solution.Domain.Query;

/// <summary>
///     Represents an inventory item that has been returned from a query, including its display index.
/// </summary>
/// <param name="Index">The display index of the item (1-based).</param>
/// <param name="InventoryItemId">The unique identifier for the inventory item.</param>
/// <param name="Name">The name of the tea.</param>
/// <param name="Price">The price per unit.</param>
/// <param name="Quantity">The available quantity.</param>
/// <param name="StarRating">The star rating of the tea.</param>
public record QueriedInventoryItem(
    int Index,
    Guid InventoryItemId,
    string Name,
    decimal Price,
    int Quantity,
    StarRating StarRating) : InventoryItem(InventoryItemId, Name, Price, Quantity, StarRating)
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="QueriedInventoryItem" /> record based on an existing
    ///     <see cref="InventoryItem" />.
    /// </summary>
    /// <param name="index">The display index of the item.</param>
    /// <param name="item">The inventory item to wrap.</param>
    public QueriedInventoryItem(int index, InventoryItem item)
        : this(index, item.InventoryItemId, item.Name, item.Price, item.Quantity, item.StarRating)
    {
    }
}