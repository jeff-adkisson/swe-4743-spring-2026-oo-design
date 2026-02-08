package assignment2solution.domain.inventoryquery;

import assignment2solution.domain.inventory.InventoryItem;
import assignment2solution.domain.inventory.InventoryRepository;

import java.util.List;
import java.util.Objects;

/**
 * The base query that retrieves all items from the inventory repository.
 */
public final class AllInventoryQuery implements IInventoryQuery {
    private final InventoryRepository repository;

    /**
     * Initializes a new instance of the {@link AllInventoryQuery} class.
     */
    public AllInventoryQuery(InventoryRepository repository) {
        this.repository = Objects.requireNonNull(repository, "repository");
    }

    @Override
    public List<InventoryItem> execute() {
        return repository.get();
    }

    @Override
    public List<String> getAppliedFiltersAndSorts() {
        return List.of();
    }
}
