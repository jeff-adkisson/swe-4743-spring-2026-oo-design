package assignment2solution.domain.inventoryquery.filters;

import assignment2solution.domain.inventory.InventoryItem;
import assignment2solution.domain.inventoryquery.IInventoryQuery;
import assignment2solution.domain.inventoryquery.InventoryQueryDecoratorBase;

import java.util.List;
import java.util.Locale;
import java.util.Objects;
import java.util.stream.Collectors;

/**
 * A filter that filters inventory items by checking if their name contains a substring.
 */
public final class NameContainsFilterDecorator extends InventoryQueryDecoratorBase {
    private final String substring;

    /**
     * Initializes a new instance of the {@link NameContainsFilterDecorator} class.
     */
    public NameContainsFilterDecorator(IInventoryQuery inner, String substring) {
        super(inner);
        this.substring = Objects.requireNonNull(substring, "substring");
    }

    @Override
    protected String getAppliedDescription() {
        if (substring.trim().isEmpty()) {
            return null;
        }
        return "Filter: Name contains \"" + substring + "\"";
    }

    @Override
    protected List<InventoryItem> decorate(List<InventoryItem> items) {
        if (substring.trim().isEmpty()) {
            return items;
        }

        var lowered = substring.toLowerCase(Locale.ROOT);
        return items.stream()
            .filter(i -> i.getName().toLowerCase(Locale.ROOT).contains(lowered))
            .collect(Collectors.toUnmodifiableList());
    }
}
