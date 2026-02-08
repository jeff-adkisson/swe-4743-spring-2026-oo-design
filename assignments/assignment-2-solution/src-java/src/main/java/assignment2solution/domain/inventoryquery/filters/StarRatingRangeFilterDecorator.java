package assignment2solution.domain.inventoryquery.filters;

import assignment2solution.domain.inventory.InventoryItem;
import assignment2solution.domain.inventoryquery.IInventoryQuery;
import assignment2solution.domain.inventoryquery.InventoryQueryDecoratorBase;

import java.util.List;
import java.util.stream.Collectors;

/**
 * A filter that filters inventory items by a range of star ratings.
 */
public final class StarRatingRangeFilterDecorator extends InventoryQueryDecoratorBase {
    private final Integer min;
    private final Integer max;

    /**
     * Initializes a new instance of the {@link StarRatingRangeFilterDecorator} class.
     */
    public StarRatingRangeFilterDecorator(IInventoryQuery inner, Integer minInclusive, Integer maxInclusive) {
        super(inner);
        if (minInclusive != null && (minInclusive < 1 || minInclusive > 5)) {
            throw new IllegalArgumentException("Rating must be between 1 and 5");
        }
        if (maxInclusive != null && (maxInclusive < 1 || maxInclusive > 5)) {
            throw new IllegalArgumentException("Rating must be between 1 and 5");
        }
        if (minInclusive != null && maxInclusive != null && minInclusive > maxInclusive) {
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
            return "Filter: Star rating between " + min + " and " + max + " (inclusive)";
        }
        if (min != null) {
            return "Filter: Star rating >= " + min;
        }
        return "Filter: Star rating <= " + max;
    }

    @Override
    protected List<InventoryItem> decorate(List<InventoryItem> items) {
        var stream = items.stream();

        if (min != null) {
            stream = stream.filter(i -> i.getStarRating().getRating() >= min);
        }
        if (max != null) {
            stream = stream.filter(i -> i.getStarRating().getRating() <= max);
        }

        return stream.collect(Collectors.toUnmodifiableList());
    }
}
