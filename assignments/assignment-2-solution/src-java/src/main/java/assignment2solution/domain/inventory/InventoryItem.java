package assignment2solution.domain.inventory;

import java.math.BigDecimal;
import java.util.Objects;
import java.util.UUID;

/**
 * Represents an item in the tea shop inventory.
 */
public class InventoryItem {
    private final UUID inventoryItemId;
    private final String name;
    private final BigDecimal price;
    private final int quantity;
    private final StarRating starRating;

    /**
     * Initializes a new instance of the {@link InventoryItem} class.
     */
    public InventoryItem(UUID inventoryItemId, String name, BigDecimal price, int quantity, StarRating starRating) {
        this.inventoryItemId = Objects.requireNonNull(inventoryItemId, "inventoryItemId");
        this.name = Objects.requireNonNull(name, "name");
        this.price = Objects.requireNonNull(price, "price");
        this.quantity = quantity;
        this.starRating = Objects.requireNonNull(starRating, "starRating");
    }

    public UUID getInventoryItemId() {
        return inventoryItemId;
    }

    public String getName() {
        return name;
    }

    public BigDecimal getPrice() {
        return price;
    }

    public int getQuantity() {
        return quantity;
    }

    public StarRating getStarRating() {
        return starRating;
    }

    /**
     * Gets a value indicating whether the item is in stock.
     */
    public boolean isAvailable() {
        return quantity > 0;
    }

    /**
     * Gets the total value of the inventory for this item.
     */
    public BigDecimal getTotalPrice() {
        return price.multiply(BigDecimal.valueOf(quantity));
    }
}
