package assignment2solution.domain.inventoryquery;

import assignment2solution.domain.inventory.InventoryItem;

import java.util.List;

/**
 * Defines a query for retrieving and filtering inventory items.
 */
public interface IInventoryQuery {
    /**
     * Gets a list of descriptions for the filters and sorts applied to this query.
     */
    List<String> getAppliedFiltersAndSorts();

    /**
     * Executes the query and returns the filtered and sorted items.
     */
    List<InventoryItem> execute();
}
