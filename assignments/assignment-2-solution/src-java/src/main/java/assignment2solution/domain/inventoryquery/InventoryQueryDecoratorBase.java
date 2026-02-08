package assignment2solution.domain.inventoryquery;

import assignment2solution.domain.inventory.InventoryItem;

import java.util.ArrayList;
import java.util.List;
import java.util.Objects;

/**
 * Base class for decorators that add filtering or sorting behavior to an {@link IInventoryQuery}.
 */
public abstract class InventoryQueryDecoratorBase implements IInventoryQuery {
    private final IInventoryQuery inner;

    /**
     * Initializes a new instance of the {@link InventoryQueryDecoratorBase} class.
     *
     * @param inner The inner query to decorate.
     */
    protected InventoryQueryDecoratorBase(IInventoryQuery inner) {
        this.inner = Objects.requireNonNull(inner, "inner");
    }

    /**
     * Gets the description of the filter or sort applied by this decorator.
     */
    protected String getAppliedDescription() {
        return null;
    }

    @Override
    public List<InventoryItem> execute() {
        var items = inner.execute();
        return decorate(items);
    }

    @Override
    public List<String> getAppliedFiltersAndSorts() {
        var description = getAppliedDescription();
        if (description == null) {
            return inner.getAppliedFiltersAndSorts();
        }

        var combined = new ArrayList<String>(inner.getAppliedFiltersAndSorts().size() + 1);
        combined.addAll(inner.getAppliedFiltersAndSorts());
        combined.add(description);
        return List.copyOf(combined);
    }

    /**
     * Applies the decorator's specific logic (filtering or sorting) to the results of the inner query.
     *
     * @param items The items returned by the inner query.
     * @return the decorated (filtered or sorted) list of items.
     */
    protected abstract List<InventoryItem> decorate(List<InventoryItem> items);
}
