package assignment2solution.userinterface.paymentbuilder;

import assignment2solution.domain.payment.CreditCardStrategy;
import assignment2solution.domain.payment.IPaymentStrategy;

import java.io.BufferedReader;
import java.io.PrintWriter;
import java.io.Reader;
import java.io.Writer;

/**
 * Represents the credit card payment method.
 */
public final class CreditCardPaymentBuilder implements IPaymentBuilder {
    @Override
    public String getName() {
        return "Credit Card";
    }

    @Override
    public IPaymentStrategy createStrategy(Reader input, Writer output) {
        var reader = input instanceof BufferedReader ? (BufferedReader) input : new BufferedReader(input);
        var writer = output instanceof PrintWriter ? (PrintWriter) output : new PrintWriter(output, true);

        while (true) {
            writer.print("Enter Credit Card Number: ");
            writer.flush();
            var cardNumber = readLine(reader);
            if (cardNumber != null && !cardNumber.trim().isEmpty() && cardNumber.chars().allMatch(Character::isDigit)) {
                return new CreditCardStrategy(cardNumber);
            }

            writer.println("Invalid card number. Must be numeric and at least 1 digit.");
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
