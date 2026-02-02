using Assignment2Solution.Domain.Inventory;

namespace Assignment2Solution.Domain.PaymentStrategy;

/// <summary>
///     A payment strategy for cryptocurrency payments.
/// </summary>
public sealed class CryptoCurrencyStrategy : PaymentStrategyBase
{
    private readonly string _transactionSignature;
    private readonly string _walletAddress;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CryptoCurrencyStrategy" /> class.
    /// </summary>
    /// <param name="walletAddress">The cryptocurrency wallet address.</param>
    /// <param name="transactionSignature">The transaction signature.</param>
    /// <exception cref="ArgumentException">Thrown when wallet address or transaction signature is empty.</exception>
    public CryptoCurrencyStrategy(string walletAddress, string transactionSignature)
    {
        if (string.IsNullOrWhiteSpace(walletAddress))
            throw new ArgumentException("Wallet address cannot be empty.", nameof(walletAddress));
        if (string.IsNullOrWhiteSpace(transactionSignature))
            throw new ArgumentException("Transaction signature cannot be empty.", nameof(transactionSignature));

        _walletAddress = walletAddress;
        _transactionSignature = transactionSignature;
    }

    /// <inheritdoc />
    public override void Checkout(InventoryItem item, int quantity, TextWriter output)
    {
        var total = ComputeTotalAmount(item, quantity);
        output.WriteLine(
            $"Checking out {total:C} using CryptoCurrency (Wallet: {_walletAddress}, Signature: {_transactionSignature}).");
    }
}