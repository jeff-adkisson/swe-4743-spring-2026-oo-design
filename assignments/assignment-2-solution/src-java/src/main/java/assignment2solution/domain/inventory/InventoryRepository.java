package assignment2solution.domain.inventory;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.UUID;

/**
 * A very simple repository for managing the tea shop inventory.
 */
public final class InventoryRepository {
    private final List<InventoryItem> items;

    /**
     * Initializes a new instance of the {@link InventoryRepository} class with default items.
     */
    public InventoryRepository() {
        items = new ArrayList<>(List.of(
            new InventoryItem(UUID.randomUUID(), "Green Tea", new BigDecimal("15.99"), 50, new StarRating(4)),
            new InventoryItem(UUID.randomUUID(), "Black Tea", new BigDecimal("12.49"), 75, new StarRating(5)),
            new InventoryItem(UUID.randomUUID(), "Herbal Tea", new BigDecimal("14.29"), 30, new StarRating(3)),
            new InventoryItem(UUID.randomUUID(), "Oolong Tea", new BigDecimal("18.00"), 10, new StarRating(5)),
            new InventoryItem(UUID.randomUUID(), "Matcha", new BigDecimal("29.99"), 0, new StarRating(4)),
            new InventoryItem(UUID.randomUUID(), "White Tea", new BigDecimal("22.50"), 25, new StarRating(4)),
            new InventoryItem(UUID.randomUUID(), "Chai Tea", new BigDecimal("10.99"), 60, new StarRating(3)),
            new InventoryItem(UUID.randomUUID(), "Earl Grey", new BigDecimal("13.99"), 45, new StarRating(5)),
            new InventoryItem(UUID.randomUUID(), "Rooibos", new BigDecimal("17.10"), 0, new StarRating(5)),
            new InventoryItem(UUID.randomUUID(), "Mint Tea", new BigDecimal("11.89"), 80, new StarRating(1)),
            new InventoryItem(UUID.randomUUID(), "Jasmine Green", new BigDecimal("16.75"), 35, new StarRating(4)),
            new InventoryItem(UUID.randomUUID(), "Genmaicha", new BigDecimal("14.10"), 28, new StarRating(3)),
            new InventoryItem(UUID.randomUUID(), "Sencha", new BigDecimal("19.25"), 40, new StarRating(4)),
            new InventoryItem(UUID.randomUUID(), "Darjeeling", new BigDecimal("21.60"), 18, new StarRating(5)),
            new InventoryItem(UUID.randomUUID(), "Assam", new BigDecimal("13.40"), 55, new StarRating(4)),
            new InventoryItem(UUID.randomUUID(), "Ceylon", new BigDecimal("12.90"), 62, new StarRating(3)),
            new InventoryItem(UUID.randomUUID(), "Lapsang Souchong", new BigDecimal("20.75"), 12, new StarRating(2)),
            new InventoryItem(UUID.randomUUID(), "Keemun", new BigDecimal("17.35"), 22, new StarRating(4)),
            new InventoryItem(UUID.randomUUID(), "Pu-erh", new BigDecimal("26.80"), 15, new StarRating(5)),
            new InventoryItem(UUID.randomUUID(), "Hojicha", new BigDecimal("15.20"), 48, new StarRating(3)),
            new InventoryItem(UUID.randomUUID(), "Gyokuro", new BigDecimal("32.50"), 8, new StarRating(5)),
            new InventoryItem(UUID.randomUUID(), "Bancha", new BigDecimal("9.95"), 90, new StarRating(2)),
            new InventoryItem(UUID.randomUUID(), "Yerba Mate", new BigDecimal("11.50"), 70, new StarRating(3)),
            new InventoryItem(UUID.randomUUID(), "Tulsi", new BigDecimal("13.25"), 33, new StarRating(4)),
            new InventoryItem(UUID.randomUUID(), "Chamomile", new BigDecimal("8.75"), 120, new StarRating(2)),
            new InventoryItem(UUID.randomUUID(), "Lavender", new BigDecimal("9.60"), 44, new StarRating(2)),
            new InventoryItem(UUID.randomUUID(), "Lemongrass", new BigDecimal("10.40"), 52, new StarRating(3)),
            new InventoryItem(UUID.randomUUID(), "Peppermint", new BigDecimal("9.25"), 0, new StarRating(1)),
            new InventoryItem(UUID.randomUUID(), "Spearmint", new BigDecimal("9.10"), 66, new StarRating(2)),
            new InventoryItem(UUID.randomUUID(), "Ginger Tea", new BigDecimal("12.15"), 58, new StarRating(3)),
            new InventoryItem(UUID.randomUUID(), "Lemon Ginger", new BigDecimal("11.80"), 47, new StarRating(3)),
            new InventoryItem(UUID.randomUUID(), "Turmeric Tea", new BigDecimal("13.95"), 38, new StarRating(4)),
            new InventoryItem(UUID.randomUUID(), "Hibiscus", new BigDecimal("10.25"), 41, new StarRating(2)),
            new InventoryItem(UUID.randomUUID(), "Rosehip", new BigDecimal("10.55"), 29, new StarRating(3)),
            new InventoryItem(UUID.randomUUID(), "Berry Blend", new BigDecimal("12.05"), 34, new StarRating(4)),
            new InventoryItem(UUID.randomUUID(), "Cinnamon Spice", new BigDecimal("11.35"), 57, new StarRating(3)),
            new InventoryItem(UUID.randomUUID(), "Vanilla Chai", new BigDecimal("14.85"), 26, new StarRating(4)),
            new InventoryItem(UUID.randomUUID(), "Masala Chai", new BigDecimal("15.45"), 21, new StarRating(5)),
            new InventoryItem(UUID.randomUUID(), "Kashmiri Chai", new BigDecimal("18.90"), 9, new StarRating(4)),
            new InventoryItem(UUID.randomUUID(), "London Fog", new BigDecimal("13.70"), 31, new StarRating(3)),
            new InventoryItem(UUID.randomUUID(), "Breakfast Blend", new BigDecimal("12.20"), 63, new StarRating(4)),
            new InventoryItem(UUID.randomUUID(), "English Breakfast", new BigDecimal("11.95"), 77, new StarRating(4)),
            new InventoryItem(UUID.randomUUID(), "Irish Breakfast", new BigDecimal("12.65"), 54, new StarRating(3)),
            new InventoryItem(UUID.randomUUID(), "Scottish Breakfast", new BigDecimal("13.15"), 0, new StarRating(2)),
            new InventoryItem(UUID.randomUUID(), "Smoky Earl Grey", new BigDecimal("14.55"), 24, new StarRating(5)),
            new InventoryItem(UUID.randomUUID(), "Orange Pekoe", new BigDecimal("10.85"), 68, new StarRating(3)),
            new InventoryItem(UUID.randomUUID(), "Lemon Zest", new BigDecimal("9.75"), 83, new StarRating(2)),
            new InventoryItem(UUID.randomUUID(), "Peach Oolong", new BigDecimal("17.90"), 14, new StarRating(4)),
            new InventoryItem(UUID.randomUUID(), "Coconut Green", new BigDecimal("16.40"), 0, new StarRating(3)),
            new InventoryItem(UUID.randomUUID(), "Caramel Rooibos", new BigDecimal("18.35"), 19, new StarRating(4))
        ));
    }

    /**
     * Gets all inventory items.
     */
    public List<InventoryItem> get() {
        return Collections.unmodifiableList(items);
    }

    /**
     * Updates the quantity of a specific inventory item.
     *
     * @param inventoryItemId The ID of the item to update.
     * @param quantityChange  The amount to change the quantity by (positive or negative).
     * @throws IllegalArgumentException when the item is not found.
     * @throws IllegalStateException    when the resulting quantity would be negative.
     */
    public void updateQuantity(UUID inventoryItemId, int quantityChange) {
        int index = -1;
        for (int i = 0; i < items.size(); i++) {
            if (items.get(i).getInventoryItemId().equals(inventoryItemId)) {
                index = i;
                break;
            }
        }

        if (index == -1) {
            throw new IllegalArgumentException("Item not found");
        }

        var current = items.get(index);
        var newQuantity = current.getQuantity() + quantityChange;

        // invariant: quantity cannot be negative
        if (newQuantity < 0) {
            throw new IllegalStateException("Insufficient inventory.");
        }

        items.set(index, new InventoryItem(
            current.getInventoryItemId(),
            current.getName(),
            current.getPrice(),
            newQuantity,
            current.getStarRating()
        ));
    }
}
