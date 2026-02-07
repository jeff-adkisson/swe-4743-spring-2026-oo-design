using Assignment2Solution.Domain.Payment;

namespace Assignment2Solution.UserInterface.PaymentBuilder;

/// <summary>
///     Represents the credit card payment method.
/// </summary>
public sealed class CreditCardPaymentBuilder : IPaymentBuilder
{
    /// <inheritdoc />
    public string Name => "Credit Card";

    /// <inheritdoc />
    public IPaymentStrategy CreateStrategy(TextReader input, TextWriter output)
    {
        while (true)
        {
            output.Write("Enter Credit Card Number: ");
            var cardNumber = input.ReadLine();
            if (!string.IsNullOrWhiteSpace(cardNumber) && cardNumber.All(char.IsDigit))
                return new CreditCardStrategy(cardNumber);

            output.WriteLine("Invalid card number. Must be numeric and at least 1 digit.");
        }
    }
}