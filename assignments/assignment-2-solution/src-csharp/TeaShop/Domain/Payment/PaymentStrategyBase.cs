using Assignment2Solution.Domain.Inventory;

namespace Assignment2Solution.Domain.Payment;

/// <summary>
///     Base class for payment strategies that provides common functionality.
/// </summary>
public abstract class PaymentStrategyBase : IPaymentStrategy
{
    /// <inheritdoc />
    public abstract void Checkout(InventoryItem item, int quantity, TextWriter output);

    /// <summary>
    ///     Computes the total amount for a purchase.
    /// </summary>
    /// <param name="item">The inventory item.</param>
    /// <param name="quantity">The quantity.</param>
    /// <returns>The total price.</returns>
    protected decimal ComputeTotalAmount(InventoryItem item, int quantity)
    {
        return item.Price * quantity;
    }
}