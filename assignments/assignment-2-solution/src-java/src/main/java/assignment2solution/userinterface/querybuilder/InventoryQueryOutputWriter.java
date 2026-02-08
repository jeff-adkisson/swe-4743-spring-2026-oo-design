package assignment2solution.userinterface.querybuilder;

import java.io.PrintWriter;
import java.io.Writer;
import java.text.NumberFormat;
import java.util.Locale;
import java.util.Objects;

/**
 * A writer for displaying query results to the user.
 */
public final class InventoryQueryOutputWriter {
    private final PrintWriter output;
    private final NumberFormat currencyFormatter = NumberFormat.getCurrencyInstance(Locale.US);

    /**
     * Initializes a new instance of the {@link InventoryQueryOutputWriter} class.
     */
    public InventoryQueryOutputWriter(Writer output) {
        Objects.requireNonNull(output, "output");
        this.output = output instanceof PrintWriter ? (PrintWriter) output : new PrintWriter(output, true);
    }

    /**
     * Writes the query output to the configured text writer.
     */
    public void write(InventoryQueryOutput output) {
        Objects.requireNonNull(output, "output");

        this.output.println("");
        this.output.println("Applied Filters and Sorts:");
        if (output.appliedFiltersAndSorts().isEmpty()) {
            this.output.println("- (none)");
        } else {
            for (var description : output.appliedFiltersAndSorts()) {
                this.output.println("- " + description);
            }
        }

        this.output.println();
        this.output.println(output.items().size() + " items matched your query:");

        for (var item : output.items()) {
            var quantityAvailable = item.getQuantity() == 0
                ? "(OUT OF STOCK)"
                : String.format(Locale.US, "Qty: %-4d", item.getQuantity());
            var price = currencyFormatter.format(item.getPrice());
            this.output.println(String.format(Locale.US, "%2d. %-20s  %6s  %s  %s",
                item.getIndex(),
                item.getName(),
                price,
                quantityAvailable,
                item.getStarRating()));
        }
    }
}
