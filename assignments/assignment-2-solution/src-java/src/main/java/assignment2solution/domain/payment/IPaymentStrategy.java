package assignment2solution.domain.payment;

import assignment2solution.domain.inventory.InventoryItem;

import java.io.Writer;

/**
 * Defines a strategy for processing payments for inventory items.
 */
public interface IPaymentStrategy {
    /**
     * Processes the checkout for a specific item and quantity.
     */
    void checkout(InventoryItem item, int quantity, Writer output);
}
