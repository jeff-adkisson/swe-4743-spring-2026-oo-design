package assignment2solution.userinterface.paymentbuilder;

import assignment2solution.domain.payment.CryptoCurrencyStrategy;
import assignment2solution.domain.payment.IPaymentStrategy;

import java.io.BufferedReader;
import java.io.PrintWriter;
import java.io.Reader;
import java.io.Writer;

/**
 * Represents the cryptocurrency payment method.
 */
public final class CryptoCurrencyPaymentBuilder implements IPaymentBuilder {
    @Override
    public String getName() {
        return "CryptoCurrency";
    }

    @Override
    public IPaymentStrategy createStrategy(Reader input, Writer output) {
        var reader = input instanceof BufferedReader ? (BufferedReader) input : new BufferedReader(input);
        var writer = output instanceof PrintWriter ? (PrintWriter) output : new PrintWriter(output, true);

        while (true) {
            writer.print("Enter Wallet Address: ");
            writer.flush();
            var walletAddress = readLine(reader);
            writer.print("Enter Transaction Signature: ");
            writer.flush();
            var transactionSignature = readLine(reader);

            if (walletAddress != null && !walletAddress.trim().isEmpty()
                && transactionSignature != null && !transactionSignature.trim().isEmpty()) {
                return new CryptoCurrencyStrategy(walletAddress, transactionSignature);
            }

            writer.println("Invalid wallet address or transaction signature. Cannot be empty.");
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
