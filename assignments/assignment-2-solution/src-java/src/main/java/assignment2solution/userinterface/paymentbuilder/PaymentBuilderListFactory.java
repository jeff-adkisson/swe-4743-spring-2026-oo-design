package assignment2solution.userinterface.paymentbuilder;

import java.util.List;

/**
 * Provides a factory for creating a predefined list of payment strategy generators.
 */
public final class PaymentBuilderListFactory {
    private PaymentBuilderListFactory() {
    }

    public static List<IPaymentBuilder> get() {
        return List.of(
            new CreditCardPaymentBuilder(),
            new ApplePayPaymentBuilder(),
            new CryptoCurrencyPaymentBuilder()
        );
    }
}
