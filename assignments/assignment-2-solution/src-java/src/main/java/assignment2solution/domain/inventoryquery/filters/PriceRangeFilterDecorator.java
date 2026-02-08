package assignment2solution.domain.inventoryquery.filters;

import assignment2solution.domain.inventory.InventoryItem;
import assignment2solution.domain.inventoryquery.IInventoryQuery;
import assignment2solution.domain.inventoryquery.InventoryQueryDecoratorBase;

import java.math.BigDecimal;
import java.text.NumberFormat;
import java.util.List;
import java.util.Locale;
import java.util.stream.Collectors;

/**
 * A filter that filters inventory items by a price range.
 */
public final class PriceRangeFilterDecorator extends InventoryQueryDecoratorBase {
    private final BigDecimal min;
    private final BigDecimal max;
    private final NumberFormat currencyFormatter = NumberFormat.getCurrencyInstance(Locale.US);

    /**
     * Initializes a new instance of the {@link PriceRangeFilterDecorator} class.
     */
    public PriceRangeFilterDecorator(IInventoryQuery inner, BigDecimal minInclusive, BigDecimal maxInclusive) {
        super(inner);
        if (minInclusive != null && minInclusive.compareTo(BigDecimal.ZERO) < 0) {
            throw new IllegalArgumentException("minInclusive must be non-negative");
        }
        if (maxInclusive != null && maxInclusive.compareTo(BigDecimal.ZERO) < 0) {
            throw new IllegalArgumentException("maxInclusive must be non-negative");
        }
        if (minInclusive != null && maxInclusive != null && minInclusive.compareTo(maxInclusive) > 0) {
            throw new IllegalArgumentException("minInclusive cannot be greater than maxInclusive.");
        }
        this.min = minInclusive;
        this.max = maxInclusive;
    }

    @Override
    protected String getAppliedDescription() {
        if (min == null && max == null) {
            return null;
        }
        if (min != null && max != null) {
            return "Filter: Price between " + currencyFormatter.format(min) + " and "
                + currencyFormatter.format(max) + " (inclusive)";
        }
        if (min != null) {
            return "Filter: Price >= " + currencyFormatter.format(min);
        }
        return "Filter: Price <= " + currencyFormatter.format(max);
    }

    @Override
    protected List<InventoryItem> decorate(List<InventoryItem> items) {
        var stream = items.stream();

        if (min != null) {
            stream = stream.filter(i -> i.getPrice().compareTo(min) >= 0);
        }
        if (max != null) {
            stream = stream.filter(i -> i.getPrice().compareTo(max) <= 0);
        }

        return stream.collect(Collectors.toUnmodifiableList());
    }
}
