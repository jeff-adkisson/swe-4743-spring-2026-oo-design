using Assignment2Solution.Domain.PaymentStrategy;

namespace Assignment2Solution.UserInterface.PaymentMethodGenerator;

/// <summary>
///     Defines a payment method that can create a corresponding payment strategy.
/// </summary>
public interface IPaymentStrategyGenerator
{
    /// <summary>
    ///     Gets the display name of the payment method.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Creates a payment strategy by prompting the user for necessary information.
    /// </summary>
    /// <param name="input">The text reader for user input.</param>
    /// <param name="output">The text writer for application output.</param>
    /// <returns>A new payment strategy.</returns>
    IPaymentStrategy CreateStrategy(TextReader input, TextWriter output);
}