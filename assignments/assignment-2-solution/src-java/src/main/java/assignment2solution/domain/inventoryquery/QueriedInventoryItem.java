package assignment2solution.domain.inventoryquery;

import assignment2solution.domain.inventory.InventoryItem;
import assignment2solution.domain.inventory.StarRating;

import java.math.BigDecimal;
import java.util.Objects;
import java.util.UUID;

/**
 * Represents an inventory item that has been returned from a query, including its display index.
 */
public final class QueriedInventoryItem extends InventoryItem {
    private final int index;

    public QueriedInventoryItem(int index, UUID inventoryItemId, String name, BigDecimal price, int quantity,
                                StarRating starRating) {
        super(inventoryItemId, name, price, quantity, starRating);
        this.index = index;
    }

    /**
     * Initializes a new instance of the {@link QueriedInventoryItem} class based on an existing
     * {@link InventoryItem}.
     */
    public QueriedInventoryItem(int index, InventoryItem item) {
        this(index,
            Objects.requireNonNull(item, "item").getInventoryItemId(),
            item.getName(),
            item.getPrice(),
            item.getQuantity(),
            item.getStarRating());
    }

    /**
     * Gets the display index of the item (1-based).
     */
    public int getIndex() {
        return index;
    }
}
