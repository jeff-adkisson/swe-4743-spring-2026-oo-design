using Assignment2Solution.Domain.PaymentStrategy;

namespace Assignment2Solution.UserInterface.PaymentMethodGenerator;

/// <summary>
///     Represents the cryptocurrency payment method.
/// </summary>
public sealed class CryptoCurrencyPaymentStrategyGenerator : IPaymentStrategyGenerator
{
    /// <inheritdoc />
    public string Name => "CryptoCurrency";

    /// <inheritdoc />
    public IPaymentStrategy CreateStrategy(TextReader input, TextWriter output)
    {
        while (true)
        {
            output.Write("Enter Wallet Address: ");
            var walletAddress = input.ReadLine();
            output.Write("Enter Transaction Signature: ");
            var transactionSignature = input.ReadLine();
            if (!string.IsNullOrWhiteSpace(walletAddress) && !string.IsNullOrWhiteSpace(transactionSignature))
                return new CryptoCurrencyStrategy(walletAddress, transactionSignature);

            output.WriteLine("Invalid wallet address or transaction signature. Cannot be empty.");
        }
    }
}