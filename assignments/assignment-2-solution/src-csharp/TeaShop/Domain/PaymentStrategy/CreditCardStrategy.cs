using Assignment2Solution.Domain.Inventory;

namespace Assignment2Solution.Domain.PaymentStrategy;

/// <summary>
///     A payment strategy for credit card payments.
/// </summary>
public sealed class CreditCardStrategy : PaymentStrategyBase
{
    private readonly string _cardNumber;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CreditCardStrategy" /> class.
    /// </summary>
    /// <param name="cardNumber">The credit card number.</param>
    /// <exception cref="ArgumentException">Thrown when card number is empty or non-numeric.</exception>
    public CreditCardStrategy(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber) || !cardNumber.All(char.IsDigit))
            throw new ArgumentException("Card number must be numeric and at least 1 digit.", nameof(cardNumber));
        _cardNumber = cardNumber;
    }

    /// <inheritdoc />
    public override void Checkout(InventoryItem item, int quantity, TextWriter output)
    {
        var total = ComputeTotalAmount(item, quantity);
        output.WriteLine($"Checking out {total:C} using Credit Card (Number: {_cardNumber}).");
    }
}