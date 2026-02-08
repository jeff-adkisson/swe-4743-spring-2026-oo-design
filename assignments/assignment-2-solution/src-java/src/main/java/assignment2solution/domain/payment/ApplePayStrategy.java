package assignment2solution.domain.payment;

import assignment2solution.domain.inventory.InventoryItem;

import java.io.PrintWriter;
import java.io.Writer;
import java.text.NumberFormat;
import java.util.Locale;
import java.util.Objects;

/**
 * A payment strategy for Apple Pay.
 */
public final class ApplePayStrategy extends PaymentStrategyBase {
    private final String appleUsername;
    private final NumberFormat currencyFormatter = NumberFormat.getCurrencyInstance(Locale.US);

    /**
     * Initializes a new instance of the {@link ApplePayStrategy} class.
     */
    public ApplePayStrategy(String appleUsername) {
        if (appleUsername == null || appleUsername.trim().isEmpty()) {
            throw new IllegalArgumentException("Apple username cannot be empty.");
        }
        this.appleUsername = appleUsername;
    }

    @Override
    public void checkout(InventoryItem item, int quantity, Writer output) {
        Objects.requireNonNull(item, "item");
        Objects.requireNonNull(output, "output");
        var writer = output instanceof PrintWriter ? (PrintWriter) output : new PrintWriter(output, true);
        var total = computeTotalAmount(item, quantity);
        writer.println("Checking out " + currencyFormatter.format(total) + " using Apple Pay (User: "
            + appleUsername + ").");
    }
}
