package teashop.unittest.domain.paymentstrategy;

import assignment2solution.domain.inventory.InventoryItem;
import assignment2solution.domain.inventory.StarRating;
import assignment2solution.domain.payment.ApplePayStrategy;
import assignment2solution.domain.payment.CreditCardStrategy;
import assignment2solution.domain.payment.CryptoCurrencyStrategy;

import org.junit.jupiter.api.Test;

import java.io.StringWriter;
import java.math.BigDecimal;
import java.util.UUID;

import static org.junit.jupiter.api.Assertions.*;

public class PaymentStrategyTests {
    private final InventoryItem testItem = new InventoryItem(
        UUID.randomUUID(),
        "Green Tea",
        new BigDecimal("15.00"),
        50,
        new StarRating(4)
    );

    @Test
    public void applePayStrategyCheckoutWritesToOutput() {
        var strategy = new ApplePayStrategy("user@example.com");
        var writer = new StringWriter();

        strategy.checkout(testItem, 2, writer);

        var output = writer.toString();
        assertTrue(output.contains("Apple Pay"));
        assertTrue(output.contains("user@example.com"));
        assertTrue(output.contains("$30.00"));
    }

    @Test
    public void applePayStrategyEmptyUsernameThrowsException() {
        assertThrows(IllegalArgumentException.class, () -> new ApplePayStrategy(""));
    }

    @Test
    public void creditCardStrategyCheckoutWritesToOutput() {
        var strategy = new CreditCardStrategy("1234567890123456");
        var writer = new StringWriter();

        strategy.checkout(testItem, 2, writer);

        var output = writer.toString();
        assertTrue(output.contains("Credit Card"));
        assertTrue(output.contains("1234567890123456"));
        assertTrue(output.contains("$30.00"));
    }

    @Test
    public void cryptoCurrencyStrategyCheckoutWritesToOutput() {
        var strategy = new CryptoCurrencyStrategy("0x123abc456def", "sig123");
        var writer = new StringWriter();

        strategy.checkout(testItem, 2, writer);

        var output = writer.toString();
        assertTrue(output.contains("Crypto"));
        assertTrue(output.contains("0x123abc456def"));
        assertTrue(output.contains("sig123"));
        assertTrue(output.contains("$30.00"));
    }
}
