package teashop.unittest.domain.inventoryquery;

import assignment2solution.domain.inventory.InventoryItem;
import assignment2solution.domain.inventory.InventoryRepository;
import assignment2solution.domain.inventory.StarRating;
import assignment2solution.domain.inventoryquery.AllInventoryQuery;
import assignment2solution.domain.inventoryquery.IInventoryQuery;
import assignment2solution.domain.inventoryquery.filters.AvailabilityFilterDecorator;
import assignment2solution.domain.inventoryquery.filters.NameContainsFilterDecorator;
import assignment2solution.domain.inventoryquery.filters.PriceRangeFilterDecorator;
import assignment2solution.domain.inventoryquery.sorts.SortByPriceDecorator;
import assignment2solution.domain.inventoryquery.sorts.SortDirection;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;

import java.math.BigDecimal;
import java.util.List;
import java.util.UUID;

import static org.junit.jupiter.api.Assertions.*;

public class QueryTests {
    private final List<InventoryItem> testItems = List.of(
        new InventoryItem(UUID.randomUUID(), "Green Tea", new BigDecimal("15.00"), 10, new StarRating(4)),
        new InventoryItem(UUID.randomUUID(), "Black Tea", new BigDecimal("10.00"), 0, new StarRating(5)),
        new InventoryItem(UUID.randomUUID(), "Oolong Tea", new BigDecimal("20.00"), 5, new StarRating(3)),
        new InventoryItem(UUID.randomUUID(), "Matcha", new BigDecimal("30.00"), 2, new StarRating(5))
    );

    private IInventoryQuery getStubQuery() {
        return new IInventoryQuery() {
            @Override
            public List<String> getAppliedFiltersAndSorts() {
                return List.of();
            }

            @Override
            public List<InventoryItem> execute() {
                return testItems;
            }
        };
    }

    @Test
    public void allInventoryQueryExecuteReturnsAllItems() {
        var repository = new InventoryRepository();
        var query = new AllInventoryQuery(repository);

        var result = query.execute();

        assertEquals(repository.get().size(), result.size());
    }

    @ParameterizedTest
    @CsvSource(value = {
        "true,3",
        "false,1",
        "null,4"
    }, nullValues = "null")
    public void availabilityFilterExecuteFiltersCorrectly(Boolean isAvailable, int expectedCount) {
        var mockQuery = getStubQuery();
        var filter = new AvailabilityFilterDecorator(mockQuery, isAvailable);

        var result = filter.execute();

        assertEquals(expectedCount, result.size());
    }

    @Test
    public void priceRangeFilterExecuteFiltersCorrectly() {
        var mockQuery = getStubQuery();
        var filter = new PriceRangeFilterDecorator(mockQuery, new BigDecimal("12.00"), new BigDecimal("25.00"));

        var result = filter.execute();

        assertEquals(2, result.size());
        assertTrue(result.stream().allMatch(i ->
            i.getPrice().compareTo(new BigDecimal("12.00")) >= 0
                && i.getPrice().compareTo(new BigDecimal("25.00")) <= 0));
    }

    @Test
    public void nameContainsFilterExecuteFiltersCorrectly() {
        var mockQuery = getStubQuery();
        var filter = new NameContainsFilterDecorator(mockQuery, "Green");

        var result = filter.execute();

        assertEquals(1, result.size());
        assertEquals("Green Tea", result.get(0).getName());
    }

    @Test
    public void sortByPriceExecuteSortsDescendingCorrectly() {
        var mockQuery = getStubQuery();
        var sort = new SortByPriceDecorator(mockQuery, SortDirection.DESCENDING);

        var result = sort.execute();

        assertEquals(new BigDecimal("30.00"), result.get(0).getPrice());
        assertEquals(new BigDecimal("20.00"), result.get(1).getPrice());
        assertEquals(new BigDecimal("15.00"), result.get(2).getPrice());
        assertEquals(new BigDecimal("10.00"), result.get(3).getPrice());
    }

    @Test
    public void combinedFiltersAndSortsWorksCorrectly() {
        var mockQuery = getStubQuery();
        var availabilityFilter = new AvailabilityFilterDecorator(mockQuery, true);
        var priceFilter = new PriceRangeFilterDecorator(availabilityFilter, new BigDecimal("10.00"), new BigDecimal("20.00"));
        var sort = new SortByPriceDecorator(priceFilter);

        var result = sort.execute();

        assertEquals(2, result.size());
        assertEquals("Green Tea", result.get(0).getName());
        assertEquals("Oolong Tea", result.get(1).getName());
        assertEquals(3, sort.getAppliedFiltersAndSorts().size());
    }
}
