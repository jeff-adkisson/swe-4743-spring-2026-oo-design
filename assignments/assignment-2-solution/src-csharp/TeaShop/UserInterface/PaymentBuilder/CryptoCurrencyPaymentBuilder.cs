using Assignment2Solution.Domain.Payment;

namespace Assignment2Solution.UserInterface.PaymentBuilder;

/// <summary>
///     Represents the cryptocurrency payment method.
/// </summary>
public sealed class CryptoCurrencyPaymentBuilder : IPaymentBuilder
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