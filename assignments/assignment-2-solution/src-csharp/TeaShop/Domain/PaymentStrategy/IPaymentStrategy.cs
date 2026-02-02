using Assignment2Solution.Domain.Inventory;

namespace Assignment2Solution.Domain.PaymentStrategy;

/// <summary>
///     Defines a strategy for processing payments for inventory items.
/// </summary>
public interface IPaymentStrategy
{
    /// <summary>
    ///     Processes the checkout for a specific item and quantity.
    /// </summary>
    /// <param name="item">The inventory item being purchased.</param>
    /// <param name="quantity">The quantity being purchased.</param>
    /// <param name="output">The text writer for displaying checkout information.</param>
    void Checkout(InventoryItem item, int quantity, TextWriter output);
}