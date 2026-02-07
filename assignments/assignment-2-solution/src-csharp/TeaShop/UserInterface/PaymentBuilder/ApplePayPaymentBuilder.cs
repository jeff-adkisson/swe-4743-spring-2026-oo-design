using Assignment2Solution.Domain.Payment;

namespace Assignment2Solution.UserInterface.PaymentBuilder;

/// <summary>
///     Represents the Apple Pay payment method.
/// </summary>
public sealed class ApplePayPaymentBuilder : IPaymentBuilder
{
    /// <inheritdoc />
    public string Name => "Apple Pay";

    /// <inheritdoc />
    public IPaymentStrategy CreateStrategy(TextReader input, TextWriter output)
    {
        while (true)
        {
            output.Write("Enter Apple Username: ");
            var appleUsername = input.ReadLine();
            if (!string.IsNullOrWhiteSpace(appleUsername)) return new ApplePayStrategy(appleUsername);

            output.WriteLine("Invalid username. Cannot be empty.");
        }
    }
}