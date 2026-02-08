package assignment2solution.userinterface.paymentbuilder;

import assignment2solution.domain.payment.ApplePayStrategy;
import assignment2solution.domain.payment.IPaymentStrategy;

import java.io.BufferedReader;
import java.io.PrintWriter;
import java.io.Reader;
import java.io.Writer;

/**
 * Represents the Apple Pay payment method.
 */
public final class ApplePayPaymentBuilder implements IPaymentBuilder {
    @Override
    public String getName() {
        return "Apple Pay";
    }

    @Override
    public IPaymentStrategy createStrategy(Reader input, Writer output) {
        var reader = input instanceof BufferedReader ? (BufferedReader) input : new BufferedReader(input);
        var writer = output instanceof PrintWriter ? (PrintWriter) output : new PrintWriter(output, true);

        while (true) {
            writer.print("Enter Apple Username: ");
            writer.flush();
            var appleUsername = readLine(reader);
            if (appleUsername != null && !appleUsername.trim().isEmpty()) {
                return new ApplePayStrategy(appleUsername);
            }

            writer.println("Invalid username. Cannot be empty.");
        }
    }

    private String readLine(BufferedReader reader) {
        try {
            return reader.readLine();
        } catch (Exception ex) {
            return null;
        }
    }
}
