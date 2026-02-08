package teashop.unittest.domain.inventory;

import assignment2solution.domain.inventory.InventoryItem;
import assignment2solution.domain.inventory.InventoryRepository;
import assignment2solution.domain.inventory.StarRating;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;
import org.junit.jupiter.params.provider.ValueSource;

import java.math.BigDecimal;
import java.util.UUID;

import static org.junit.jupiter.api.Assertions.*;

public class InventoryTests {
    @Test
    public void inventoryItemInitializationSetsProperties() {
        var id = UUID.randomUUID();
        var name = "Test Tea";
        var price = new BigDecimal("10.50");
        var quantity = 20;
        var rating = new StarRating(4);

        var item = new InventoryItem(id, name, price, quantity, rating);

        assertEquals(id, item.getInventoryItemId());
        assertEquals(name, item.getName());
        assertEquals(price, item.getPrice());
        assertEquals(quantity, item.getQuantity());
        assertEquals(rating, item.getStarRating());
    }

    @ParameterizedTest
    @CsvSource({
        "10,true",
        "1,true",
        "0,false",
        "-1,false"
    })
    public void inventoryItemIsAvailableReturnsExpected(int quantity, boolean expected) {
        var item = new InventoryItem(UUID.randomUUID(), "Tea", new BigDecimal("10.00"), quantity, new StarRating(3));
        assertEquals(expected, item.isAvailable());
    }

    @Test
    public void inventoryItemTotalPriceCalculatesCorrectly() {
        var item = new InventoryItem(UUID.randomUUID(), "Tea", new BigDecimal("10.50"), 3, new StarRating(3));
        assertEquals(new BigDecimal("31.50"), item.getTotalPrice());
    }

    @ParameterizedTest
    @ValueSource(ints = {1, 3, 5})
    public void starRatingValidRatingSetsRating(int ratingValue) {
        var rating = new StarRating(ratingValue);
        assertEquals(ratingValue, rating.getRating());
    }

    @ParameterizedTest
    @ValueSource(ints = {0, 6, -1})
    public void starRatingInvalidRatingThrowsException(int ratingValue) {
        assertThrows(IllegalArgumentException.class, () -> new StarRating(ratingValue));
    }

    @Test
    public void starRatingToStringReturnsFormattedString() {
        var rating = new StarRating(4);
        assertEquals("4****", rating.toString());
    }

    @Test
    public void inventoryRepositoryGetReturnsInitialItems() {
        var repository = new InventoryRepository();
        var items = repository.get();

        assertNotNull(items);
        assertFalse(items.isEmpty());
        assertEquals(50, items.size());
    }

    @Test
    public void inventoryRepositoryUpdateQuantityUpdatesItem() {
        var repository = new InventoryRepository();
        var item = repository.get().get(0);
        var initialQuantity = item.getQuantity();
        var change = 5;

        repository.updateQuantity(item.getInventoryItemId(), change);
        var updatedItem = repository.get().stream()
            .filter(i -> i.getInventoryItemId().equals(item.getInventoryItemId()))
            .findFirst()
            .orElseThrow();

        assertEquals(initialQuantity + change, updatedItem.getQuantity());
    }

    @Test
    public void inventoryRepositoryUpdateQuantityThrowsWhenInsufficient() {
        var repository = new InventoryRepository();
        var item = repository.get().stream()
            .filter(i -> i.getQuantity() > 0)
            .findFirst()
            .orElseThrow();

        assertThrows(IllegalStateException.class,
            () -> repository.updateQuantity(item.getInventoryItemId(), -item.getQuantity() - 1));
    }

    @Test
    public void inventoryRepositoryUpdateQuantityThrowsWhenNotFound() {
        var repository = new InventoryRepository();
        assertThrows(IllegalArgumentException.class,
            () -> repository.updateQuantity(UUID.randomUUID(), 1));
    }
}
