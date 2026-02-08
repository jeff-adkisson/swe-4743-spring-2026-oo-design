package assignment2solution.domain.payment;

import assignment2solution.domain.inventory.InventoryItem;

import java.io.PrintWriter;
import java.io.Writer;
import java.text.NumberFormat;
import java.util.Locale;
import java.util.Objects;

/**
 * A payment strategy for credit card payments.
 */
public final class CreditCardStrategy extends PaymentStrategyBase {
    private final String cardNumber;
    private final NumberFormat currencyFormatter = NumberFormat.getCurrencyInstance(Locale.US);

    /**
     * Initializes a new instance of the {@link CreditCardStrategy} class.
     */
    public CreditCardStrategy(String cardNumber) {
        if (cardNumber == null || cardNumber.trim().isEmpty() || !cardNumber.chars().allMatch(Character::isDigit)) {
            throw new IllegalArgumentException("Card number must be numeric and at least 1 digit.");
        }
        this.cardNumber = cardNumber;
    }

    @Override
    public void checkout(InventoryItem item, int quantity, Writer output) {
        Objects.requireNonNull(item, "item");
        Objects.requireNonNull(output, "output");
        var writer = output instanceof PrintWriter ? (PrintWriter) output : new PrintWriter(output, true);
        var total = computeTotalAmount(item, quantity);
        writer.println("Checking out " + currencyFormatter.format(total) + " using Credit Card (Number: "
            + cardNumber + ").");
    }
}
