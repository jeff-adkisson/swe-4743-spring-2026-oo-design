package assignment2solution.domain.inventoryquery.filters;

import assignment2solution.domain.inventory.InventoryItem;
import assignment2solution.domain.inventoryquery.IInventoryQuery;
import assignment2solution.domain.inventoryquery.InventoryQueryDecoratorBase;

import java.util.List;
import java.util.stream.Collectors;

/**
 * A filter that filters inventory items by their availability.
 */
public final class AvailabilityFilterDecorator extends InventoryQueryDecoratorBase {
    private final Boolean isAvailable;

    /**
     * Initializes a new instance of the {@link AvailabilityFilterDecorator} class.
     */
    public AvailabilityFilterDecorator(IInventoryQuery inner, Boolean isAvailable) {
        super(inner);
        this.isAvailable = isAvailable;
    }

    @Override
    protected String getAppliedDescription() {
        if (isAvailable == null) {
            return null;
        }
        return isAvailable
            ? "Filter: Availability = In Stock (Quantity > 0)"
            : "Filter: Availability = Out of Stock (Quantity = 0)";
    }

    @Override
    protected List<InventoryItem> decorate(List<InventoryItem> items) {
        if (isAvailable == null) {
            return items;
        }

        var filtered = isAvailable ? items.stream().filter(InventoryItem::isAvailable)
            : items.stream().filter(i -> !i.isAvailable());

        return filtered.collect(Collectors.toUnmodifiableList());
    }
}
