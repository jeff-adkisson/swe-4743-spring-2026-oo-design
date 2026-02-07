namespace Assignment2Solution.UserInterface.PaymentBuilder;

/// <summary>
///     Provides a factory for creating a predefined list of payment strategy generators.
///     This class is responsible for returning a read-only list of objects implementing
///     the <see cref="IPaymentBuilder" /> interface, representing various payment
///     methods that can be used to generate payment strategies.
/// </summary>
public static class PaymentBuilderListFactory
{
    public static IReadOnlyList<IPaymentBuilder> Get()
    {
        return new List<IPaymentBuilder>
        {
            new CreditCardPaymentBuilder(),
            new ApplePayPaymentBuilder(),
            new CryptoCurrencyPaymentBuilder()
        }.AsReadOnly();
    }
}