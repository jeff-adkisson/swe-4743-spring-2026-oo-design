package assignment2solution.userinterface.querybuilder;

import assignment2solution.domain.inventoryquery.IInventoryQuery;
import assignment2solution.domain.inventoryquery.QueriedInventoryItem;

import java.util.List;
import java.util.Objects;

/**
 * Represents the result of an inventory query, ready for display.
 */
public record InventoryQueryOutput(
    List<QueriedInventoryItem> items,
    List<String> appliedFiltersAndSorts
) {
    /**
     * Creates an {@link InventoryQueryOutput} from an {@link IInventoryQuery}.
     */
    public static InventoryQueryOutput from(IInventoryQuery query) {
        Objects.requireNonNull(query, "query");

        var results = query.execute();
        var indexed = new java.util.ArrayList<QueriedInventoryItem>(results.size());
        for (int i = 0; i < results.size(); i++) {
            indexed.add(new QueriedInventoryItem(i + 1, results.get(i)));
        }

        return new InventoryQueryOutput(List.copyOf(indexed), List.copyOf(query.getAppliedFiltersAndSorts()));
    }
}
