package assignment2solution.userinterface.paymentbuilder;

import assignment2solution.domain.payment.IPaymentStrategy;

import java.io.Reader;
import java.io.Writer;

/**
 * Defines a payment method that can create a corresponding payment strategy.
 */
public interface IPaymentBuilder {
    /**
     * Gets the display name of the payment method.
     */
    String getName();

    /**
     * Creates a payment strategy by prompting the user for necessary information.
     */
    IPaymentStrategy createStrategy(Reader input, Writer output);
}
