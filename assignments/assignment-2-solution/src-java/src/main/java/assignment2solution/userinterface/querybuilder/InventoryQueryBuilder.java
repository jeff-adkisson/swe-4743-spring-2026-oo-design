package assignment2solution.userinterface.querybuilder;

import assignment2solution.domain.inventory.InventoryRepository;
import assignment2solution.domain.inventoryquery.AllInventoryQuery;
import assignment2solution.domain.inventoryquery.IInventoryQuery;
import assignment2solution.domain.inventoryquery.filters.AvailabilityFilterDecorator;
import assignment2solution.domain.inventoryquery.filters.NameContainsFilterDecorator;
import assignment2solution.domain.inventoryquery.filters.PriceRangeFilterDecorator;
import assignment2solution.domain.inventoryquery.filters.StarRatingRangeFilterDecorator;
import assignment2solution.domain.inventoryquery.sorts.SortByPriceDecorator;
import assignment2solution.domain.inventoryquery.sorts.SortByStarRatingDecorator;
import assignment2solution.domain.inventoryquery.sorts.SortDirection;

import java.io.BufferedReader;
import java.io.PrintWriter;
import java.io.Reader;
import java.io.Writer;
import java.math.BigDecimal;
import java.util.Locale;
import java.util.Objects;

/**
 * A builder for creating {@link IInventoryQuery} objects by prompting the user for input.
 */
public final class InventoryQueryBuilder {
    private final BufferedReader input;
    private final PrintWriter output;
    private final InventoryRepository repository;

    /**
     * Initializes a new instance of the {@link InventoryQueryBuilder} class.
     */
    public InventoryQueryBuilder(InventoryRepository repository, Reader input, Writer output) {
        this.repository = Objects.requireNonNull(repository, "repository");
        Objects.requireNonNull(input, "input");
        Objects.requireNonNull(output, "output");
        this.input = input instanceof BufferedReader ? (BufferedReader) input : new BufferedReader(input);
        this.output = output instanceof PrintWriter ? (PrintWriter) output : new PrintWriter(output, true);
    }

    /**
     * Builds a query by prompting the user for search criteria.
     */
    public IInventoryQuery build() {
        IInventoryQuery query = new AllInventoryQuery(repository);

        var nameContains = readOptionalString("* Tea name contains (leave blank for all names): ");
        query = new NameContainsFilterDecorator(query, nameContains);

        var isAvailable = readAvailability();
        query = new AvailabilityFilterDecorator(query, isAvailable);

        var priceRange = readPriceRange();
        query = new PriceRangeFilterDecorator(query, priceRange.min, priceRange.max);

        var starRange = readStarRatingRange();
        query = new StarRatingRangeFilterDecorator(query, starRange.min, starRange.max);

        var priceSort = readSortDirection("* Sort by Price (A/D, default A): ", SortDirection.ASCENDING);
        query = new SortByPriceDecorator(query, priceSort);

        var starSort = readSortDirection("* Sort by Star rating (A/D, default D): ", SortDirection.DESCENDING);
        query = new SortByStarRatingDecorator(query, starSort);

        return query;
    }

    private String readOptionalString(String prompt) {
        printPrompt(prompt);
        return readLine();
    }

    private Boolean readAvailability() {
        while (true) {
            printPrompt("* Is available? (Y/N, default Y): ");
            var inputLine = readLine();
            if (inputLine == null || inputLine.trim().isEmpty()) {
                return true;
            }

            var trimmed = inputLine.trim();
            if (trimmed.equalsIgnoreCase("Y") || trimmed.equalsIgnoreCase("Yes")) {
                return true;
            }
            if (trimmed.equalsIgnoreCase("N") || trimmed.equalsIgnoreCase("No")) {
                return false;
            }

            output.println("Please enter Y or N.");
        }
    }

    private PriceRange readPriceRange() {
        var defaultMin = BigDecimal.ZERO;
        var defaultMax = new BigDecimal("1000");

        while (true) {
            var min = readDecimal("* Price minimum (default $0): ", defaultMin);
            var max = readDecimal("* Price maximum (default $1000): ", defaultMax);

            if (min.compareTo(max) <= 0) {
                return new PriceRange(min, max);
            }

            output.println("Minimum price cannot be greater than maximum price.");
        }
    }

    private StarRatingRange readStarRatingRange() {
        var defaultMin = 3;
        var defaultMax = 5;

        while (true) {
            var min = readInt("* Star rating minimum (1-5, default 3): ", defaultMin, 1, 5);
            var max = readInt("* Star rating maximum (1-5, default 5): ", defaultMax, 1, 5);

            if (min <= max) {
                return new StarRatingRange(min, max);
            }

            output.println("Minimum star rating cannot be greater than maximum star rating.");
        }
    }

    private SortDirection readSortDirection(String prompt, SortDirection defaultDirection) {
        while (true) {
            printPrompt(prompt);
            var inputLine = readLine();
            if (inputLine == null || inputLine.trim().isEmpty()) {
                return defaultDirection;
            }

            var trimmed = inputLine.trim();
            if (trimmed.equalsIgnoreCase("A")) {
                return SortDirection.ASCENDING;
            }
            if (trimmed.equalsIgnoreCase("D")) {
                return SortDirection.DESCENDING;
            }

            output.println("Please enter A or D.");
        }
    }

    private BigDecimal readDecimal(String prompt, BigDecimal defaultValue) {
        while (true) {
            printPrompt(prompt);
            var inputLine = readLine();
            if (inputLine == null || inputLine.trim().isEmpty()) {
                return defaultValue;
            }

            var cleaned = inputLine.trim().replace("$", "").replace(",", "");
            try {
                var value = new BigDecimal(cleaned);
                if (value.compareTo(BigDecimal.ZERO) >= 0) {
                    return value;
                }
            } catch (NumberFormatException ignored) {
            }

            output.println("Please enter a non-negative number.");
        }
    }

    private int readInt(String prompt, int defaultValue, int min, int max) {
        while (true) {
            printPrompt(prompt);
            var inputLine = readLine();
            if (inputLine == null || inputLine.trim().isEmpty()) {
                return defaultValue;
            }

            try {
                var value = Integer.parseInt(inputLine.trim());
                if (value >= min && value <= max) {
                    return value;
                }
            } catch (NumberFormatException ignored) {
            }

            output.println(String.format(Locale.US, "Please enter a whole number between %d and %d.", min, max));
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
        output.print(prompt);
        output.flush();
    }

    private static final class PriceRange {
        private final BigDecimal min;
        private final BigDecimal max;

        private PriceRange(BigDecimal min, BigDecimal max) {
            this.min = min;
            this.max = max;
        }
    }

    private static final class StarRatingRange {
        private final int min;
        private final int max;

        private StarRatingRange(int min, int max) {
            this.min = min;
            this.max = max;
        }
    }
}
