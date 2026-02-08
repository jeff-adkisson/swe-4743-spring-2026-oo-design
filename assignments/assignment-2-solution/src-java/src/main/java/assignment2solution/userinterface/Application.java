package assignment2solution.userinterface;

import assignment2solution.domain.inventory.InventoryRepository;
import assignment2solution.domain.inventoryquery.QueriedInventoryItem;
import assignment2solution.userinterface.paymentbuilder.IPaymentBuilder;
import assignment2solution.userinterface.paymentbuilder.PaymentBuilderListFactory;
import assignment2solution.userinterface.querybuilder.InventoryQueryBuilder;
import assignment2solution.userinterface.querybuilder.InventoryQueryOutput;
import assignment2solution.userinterface.querybuilder.InventoryQueryOutputWriter;

import java.io.BufferedReader;
import java.io.Reader;
import java.io.Writer;
import java.io.PrintWriter;
import java.math.BigDecimal;
import java.text.NumberFormat;
import java.util.List;
import java.util.Locale;
import java.util.Objects;

/**
 * The main application class that coordinates the user interface and domain logic.
 */
public final class Application {
    private final BufferedReader input;
    private final PrintWriter output;
    private final InventoryRepository repository;
    private final InventoryQueryBuilder inventoryQueryBuilder;
    private final InventoryQueryOutputWriter inventoryQueryOutputWriter;
    private final List<IPaymentBuilder> paymentMethods;
    private final NumberFormat currencyFormatter = NumberFormat.getCurrencyInstance(Locale.US);

    /**
     * Initializes a new instance of the {@link Application} class.
     *
     * @param input  The text reader for user input.
     * @param output The text writer for application output.
     */
    public Application(Reader input, Writer output) {
        Objects.requireNonNull(input, "input");
        Objects.requireNonNull(output, "output");

        this.input = new BufferedReader(input);
        this.output = new PrintWriter(output, true);
        this.repository = new InventoryRepository();
        this.inventoryQueryBuilder = new InventoryQueryBuilder(repository, this.input, this.output);
        this.inventoryQueryOutputWriter = new InventoryQueryOutputWriter(this.output);
        this.paymentMethods = PaymentBuilderListFactory.get();
    }

    /**
     * Runs the application loop.
     */
    public void run() {
        displayWelcomeMessage();

        while (true) {
            var query = inventoryQueryBuilder.build();
            var output = InventoryQueryOutput.from(query);
            inventoryQueryOutputWriter.write(output);
            this.output.println();

            if (output.items().isEmpty()) {
                this.output.println("*** Nothing matched your search ***");
            } else {
                processPurchase(output);
            }

            this.output.println();
            var searchForMore = readYesNo("Search for more tea? (Y/N, default Y): ", true);
            if (!searchForMore) {
                break;
            }
            this.output.println();
        }
    }

    private void displayWelcomeMessage() {
        output.println("WELCOME TO JEFF'S TEA SHOP");
        output.println();
        output.println("Complete the prompts to search our selection of fine teas.");
        output.println();
    }

    private void processPurchase(InventoryQueryOutput output) {
        Objects.requireNonNull(output, "output");

        if (output.items().isEmpty()) {
            this.output.println("!!! No items available to purchase.");
            return;
        }

        while (true) {
            var itemSelectionPrompt = "1-" + output.items().size() + " or 0 to continue (default)";
            printPrompt("Purchase an item? Enter item number " + itemSelectionPrompt + ": ");
            var inputLine = readLine();

            if (inputLine == null || inputLine.trim().isEmpty()) {
                return;
            }

            int index;
            try {
                index = Integer.parseInt(inputLine.trim());
            } catch (NumberFormatException ex) {
                this.output.println("!!! Invalid item number. Try again.");
                continue;
            }

            if (index < 0 || index > output.items().size()) {
                this.output.println("!!! Invalid item number. Try again.");
                continue;
            }

            if (index == 0) {
                return;
            }

            var selected = output.items().get(index - 1);

            if (selected.getQuantity() <= 0) {
                this.output.println("!!! Sorry, there is inventory available for " + selected.getName() + ".");
                return;
            }

            printPrompt("Quantity for \"" + selected.getName() + "\" (1-" + selected.getQuantity() + "): ");
            var qtyInput = readLine();
            int quantity;
            try {
                quantity = Integer.parseInt(qtyInput == null ? "" : qtyInput.trim());
            } catch (NumberFormatException ex) {
                this.output.println("!!! Invalid quantity.");
                continue;
            }
            if (quantity < 1 || quantity > selected.getQuantity()) {
                this.output.println("!!! Invalid quantity.");
                continue;
            }

            var totalPrice = selected.getPrice().multiply(BigDecimal.valueOf(quantity));
            this.output.println("*** Total Price: " + currencyFormatter.format(totalPrice));

            processCheckout(selected, quantity);
            break;
        }
    }

    private void processCheckout(QueriedInventoryItem item, int quantity) {
        this.output.println("*** Choose a payment method:");
        for (int i = 0; i < paymentMethods.size(); i++) {
            this.output.println((i + 1) + ". " + paymentMethods.get(i).getName());
        }

        printPrompt("Selection: ");

        var choice = readLine();
        int index;
        try {
            index = Integer.parseInt(choice == null ? "" : choice.trim());
        } catch (NumberFormatException ex) {
            this.output.println("!!! Invalid payment method selection. Checkout cancelled.");
            return;
        }

        if (index < 1 || index > paymentMethods.size()) {
            this.output.println("!!! Invalid payment method selection. Checkout cancelled.");
            return;
        }

        var strategy = paymentMethods.get(index - 1).createStrategy(input, output);
        strategy.checkout(item, quantity, output);

        // Decrease inventory quantity (using negative value for decrease)
        repository.updateQuantity(item.getInventoryItemId(), -1 * quantity);

        var purchaseDesc = quantity + " packages of " + item.getName();
        this.output.println("*** Purchase complete. Your " + purchaseDesc + " is on the way ***");
    }

    private boolean readYesNo(String prompt, boolean defaultValue) {
        while (true) {
            printPrompt(prompt);
            var inputLine = readLine();

            if (inputLine == null || inputLine.trim().isEmpty()) {
                return defaultValue;
            }

            var trimmed = inputLine.trim();
            if (trimmed.equalsIgnoreCase("Y")) {
                return true;
            }
            if (trimmed.equalsIgnoreCase("N")) {
                return false;
            }

            this.output.println("Please enter Y or N.");
        }
    }

    private String readLine() {
        try {
            return input.readLine();
        } catch (Exception ex) {
            return null;
        }
    }

    private void printPrompt(String prompt) {
        this.output.print(prompt);
        this.output.flush();
    }
}
