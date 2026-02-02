using Assignment2Solution.Domain.PaymentStrategy;

namespace Assignment2Solution.UserInterface.PaymentMethodGenerator;

/// <summary>
///     Represents the Apple Pay payment method.
/// </summary>
public sealed class ApplePayPaymentStrategyGenerator : IPaymentStrategyGenerator
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