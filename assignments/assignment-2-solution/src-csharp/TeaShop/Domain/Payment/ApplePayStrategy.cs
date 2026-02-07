using Assignment2Solution.Domain.Inventory;

namespace Assignment2Solution.Domain.Payment;

/// <summary>
///     A payment strategy for Apple Pay.
/// </summary>
public sealed class ApplePayStrategy : PaymentStrategyBase
{
    private readonly string _appleUsername;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApplePayStrategy" /> class.
    /// </summary>
    /// <param name="appleUsername">The Apple Pay username.</param>
    /// <exception cref="ArgumentException">Thrown when username is empty.</exception>
    public ApplePayStrategy(string appleUsername)
    {
        if (string.IsNullOrWhiteSpace(appleUsername))
            throw new ArgumentException("Apple username cannot be empty.", nameof(appleUsername));
        _appleUsername = appleUsername;
    }

    /// <inheritdoc />
    public override void Checkout(InventoryItem item, int quantity, TextWriter output)
    {
        var total = ComputeTotalAmount(item, quantity);
        output.WriteLine($"Checking out {total:C} using Apple Pay (User: {_appleUsername}).");
    }
}