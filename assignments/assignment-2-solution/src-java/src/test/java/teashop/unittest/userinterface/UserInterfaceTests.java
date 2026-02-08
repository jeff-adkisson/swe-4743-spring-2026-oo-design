package teashop.unittest.userinterface;

import assignment2solution.domain.inventory.InventoryRepository;
import assignment2solution.userinterface.paymentbuilder.ApplePayPaymentBuilder;
import assignment2solution.userinterface.paymentbuilder.CreditCardPaymentBuilder;
import assignment2solution.userinterface.paymentbuilder.CryptoCurrencyPaymentBuilder;
import assignment2solution.userinterface.querybuilder.InventoryQueryBuilder;

import org.junit.jupiter.api.Test;

import java.io.StringReader;
import java.io.StringWriter;

import static org.junit.jupiter.api.Assertions.*;

public class UserInterfaceTests {
    @Test
    public void queryBuilderBuildReturnsConfiguredQuery() {
        var repository = new InventoryRepository();
        var inputLines = new String[]{
            "Green",
            "Y",
            "10",
            "20",
            "4",
            "5",
            "A",
            "D"
        };
        var input = new StringReader(String.join(System.lineSeparator(), inputLines));
        var output = new StringWriter();
        var builder = new InventoryQueryBuilder(repository, input, output);

        var query = builder.build();

        assertNotNull(query);
        var filters = query.getAppliedFiltersAndSorts();
        assertTrue(filters.stream().anyMatch(f -> f.contains("Green")));
        assertTrue(filters.stream().anyMatch(f -> f.contains("In Stock")));
        assertTrue(filters.stream().anyMatch(f -> f.contains("$10.00") && f.contains("$20.00")));
        assertTrue(filters.stream().anyMatch(f -> f.contains("Star rating between 4") && f.contains("5")));
        assertTrue(filters.stream().anyMatch(f -> f.contains("Sort: Price (ascending)")));
        assertTrue(filters.stream().anyMatch(f -> f.contains("Sort: Star rating (descending)")));
    }

    @Test
    public void applePayPaymentMethodCreateStrategyReturnsStrategy() {
        var method = new ApplePayPaymentBuilder();
        var input = new StringReader("user@example.com" + System.lineSeparator());
        var output = new StringWriter();

        var strategy = method.createStrategy(input, output);

        assertNotNull(strategy);
        assertEquals("Apple Pay", method.getName());
        assertTrue(output.toString().contains("Enter Apple Username:"));
    }

    @Test
    public void creditCardPaymentMethodCreateStrategyReturnsStrategy() {
        var method = new CreditCardPaymentBuilder();
        var input = new StringReader("1234567890123456" + System.lineSeparator());
        var output = new StringWriter();

        var strategy = method.createStrategy(input, output);

        assertNotNull(strategy);
        assertEquals("Credit Card", method.getName());
    }

    @Test
    public void cryptoCurrencyPaymentMethodCreateStrategyReturnsStrategy() {
        var method = new CryptoCurrencyPaymentBuilder();
        var input = new StringReader("0x123" + System.lineSeparator() + "sig123" + System.lineSeparator());
        var output = new StringWriter();

        var strategy = method.createStrategy(input, output);

        assertNotNull(strategy);
        assertEquals("CryptoCurrency", method.getName());
    }
}
