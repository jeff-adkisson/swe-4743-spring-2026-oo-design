package assignment2solution.domain.inventoryquery.sorts;

import assignment2solution.domain.inventory.InventoryItem;
import assignment2solution.domain.inventoryquery.IInventoryQuery;
import assignment2solution.domain.inventoryquery.InventoryQueryDecoratorBase;

import java.util.Comparator;
import java.util.List;
import java.util.Locale;
import java.util.stream.Collectors;

/**
 * A decorator that sorts inventory items by price.
 */
public final class SortByPriceDecorator extends InventoryQueryDecoratorBase {
    private final SortDirection direction;

    /**
     * Initializes a new instance of the {@link SortByPriceDecorator} class.
     */
    public SortByPriceDecorator(IInventoryQuery inner, SortDirection direction) {
        super(inner);
        this.direction = direction == null ? SortDirection.ASCENDING : direction;
    }

    public SortByPriceDecorator(IInventoryQuery inner) {
        this(inner, SortDirection.ASCENDING);
    }

    @Override
    protected String getAppliedDescription() {
        return "Sort: Price (" + direction.name().toLowerCase(Locale.ROOT) + ")";
    }

    @Override
    protected List<InventoryItem> decorate(List<InventoryItem> items) {
        var comparator = Comparator.comparing(InventoryItem::getPrice);
        if (direction == SortDirection.DESCENDING) {
            comparator = comparator.reversed();
        }
        return items.stream().sorted(comparator).collect(Collectors.toUnmodifiableList());
    }
}
