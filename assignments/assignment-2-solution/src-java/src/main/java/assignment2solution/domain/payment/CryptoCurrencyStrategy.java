package assignment2solution.domain.payment;

import assignment2solution.domain.inventory.InventoryItem;

import java.io.PrintWriter;
import java.io.Writer;
import java.text.NumberFormat;
import java.util.Locale;
import java.util.Objects;

/**
 * A payment strategy for cryptocurrency payments.
 */
public final class CryptoCurrencyStrategy extends PaymentStrategyBase {
    private final String walletAddress;
    private final String transactionSignature;
    private final NumberFormat currencyFormatter = NumberFormat.getCurrencyInstance(Locale.US);

    /**
     * Initializes a new instance of the {@link CryptoCurrencyStrategy} class.
     */
    public CryptoCurrencyStrategy(String walletAddress, String transactionSignature) {
        if (walletAddress == null || walletAddress.trim().isEmpty()) {
            throw new IllegalArgumentException("Wallet address cannot be empty.");
        }
        if (transactionSignature == null || transactionSignature.trim().isEmpty()) {
            throw new IllegalArgumentException("Transaction signature cannot be empty.");
        }
        this.walletAddress = walletAddress;
        this.transactionSignature = transactionSignature;
    }

    @Override
    public void checkout(InventoryItem item, int quantity, Writer output) {
        Objects.requireNonNull(item, "item");
        Objects.requireNonNull(output, "output");
        var writer = output instanceof PrintWriter ? (PrintWriter) output : new PrintWriter(output, true);
        var total = computeTotalAmount(item, quantity);
        writer.println("Checking out " + currencyFormatter.format(total)
            + " using CryptoCurrency (Wallet: " + walletAddress + ", Signature: " + transactionSignature + ").");
    }
}
