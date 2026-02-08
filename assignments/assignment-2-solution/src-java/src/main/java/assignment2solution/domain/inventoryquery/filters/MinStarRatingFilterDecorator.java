package assignment2solution.domain.inventoryquery.filters;

import assignment2solution.domain.inventory.InventoryItem;
import assignment2solution.domain.inventoryquery.IInventoryQuery;
import assignment2solution.domain.inventoryquery.InventoryQueryDecoratorBase;

import java.util.List;
import java.util.stream.Collectors;

/**
 * A filter that filters inventory items by a minimum star rating.
 */
public final class MinStarRatingFilterDecorator extends InventoryQueryDecoratorBase {
    private final int minRating;

    /**
     * Initializes a new instance of the {@link MinStarRatingFilterDecorator} class.
     */
    public MinStarRatingFilterDecorator(IInventoryQuery inner, int minRatingInclusive) {
        super(inner);
        if (minRatingInclusive < 1 || minRatingInclusive > 5) {
            throw new IllegalArgumentException("Rating must be between 1 and 5");
        }
        this.minRating = minRatingInclusive;
    }

    @Override
    protected String getAppliedDescription() {
        return "Filter: Star rating >= " + minRating;
    }

    @Override
    protected List<InventoryItem> decorate(List<InventoryItem> items) {
        return items.stream()
            .filter(i -> i.getStarRating().getRating() >= minRating)
            .collect(Collectors.toUnmodifiableList());
    }
}
