namespace Assignment2Solution.UserInterface.PaymentMethodGenerator;

/// <summary>
///     Provides a factory for creating a predefined list of payment strategy generators.
///     This class is responsible for returning a read-only list of objects implementing
///     the <see cref="IPaymentStrategyGenerator" /> interface, representing various payment
///     methods that can be used to generate payment strategies.
/// </summary>
public static class PaymentStrategyGeneratorListFactory
{
    public static IReadOnlyList<IPaymentStrategyGenerator> Get()
    {
        return new List<IPaymentStrategyGenerator>
        {
            new CreditCardPaymentStrategyGenerator(),
            new ApplePayPaymentStrategyGenerator(),
            new CryptoCurrencyPaymentStrategyGenerator()
        }.AsReadOnly();
    }
}