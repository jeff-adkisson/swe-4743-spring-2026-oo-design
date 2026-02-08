package teashop.unittest.userinterface;

import assignment2solution.userinterface.Application;

import org.junit.jupiter.api.Test;

import java.io.StringReader;
import java.io.StringWriter;

import static org.junit.jupiter.api.Assertions.*;

public class ApplicationTests {
    @Test
    public void processPurchaseWhenEmptyInputShouldContinue() {
        var inputLines = new String[]{
            "", // QueryBuilder: Name contains (empty)
            "", // QueryBuilder: Is available (empty)
            "", // QueryBuilder: Price min (empty)
            "", // QueryBuilder: Price max (empty)
            "", // QueryBuilder: Rating min (empty)
            "", // QueryBuilder: Rating max (empty)
            "", // QueryBuilder: Sort by Price (empty, default A)
            "", // QueryBuilder: Sort by Star rating (empty, default D)
            "", // Application: Purchase an item? (empty input)
            "N" // Application: Search for more tea? (N)
        };

        var input = new StringReader(String.join(System.lineSeparator(), inputLines));
        var output = new StringWriter();
        var app = new Application(input, output);

        app.run();

        var outputString = output.toString();
        assertTrue(outputString.contains("or 0 to continue (default): "));
        assertFalse(outputString.contains("Invalid index. Try again."));
    }
}
