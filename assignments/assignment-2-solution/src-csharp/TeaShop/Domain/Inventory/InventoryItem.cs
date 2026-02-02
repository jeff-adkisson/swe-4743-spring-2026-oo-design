namespace Assignment2Solution.Domain.Inventory;

/// <summary>
///     Represents an item in the tea shop inventory.
/// </summary>
/// <param name="InventoryItemId">The unique identifier for the inventory item.</param>
/// <param name="Name">The name of the tea.</param>
/// <param name="Price">The price per unit.</param>
/// <param name="Quantity">The available quantity.</param>
/// <param name="StarRating">The star rating of the tea.</param>
public record InventoryItem(Guid InventoryItemId, string Name, decimal Price, int Quantity, StarRating StarRating)
{
    /// <summary>
    ///     Gets a value indicating whether the item is in stock.
    /// </summary>
    public bool IsAvailable => Quantity > 0;

    /// <summary>
    ///     Gets the total value of the inventory for this item.
    /// </summary>
    public decimal TotalPrice => Price * Quantity;
}