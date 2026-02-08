package assignment2solution.domain.payment;

import assignment2solution.domain.inventory.InventoryItem;

import java.math.BigDecimal;

/**
 * Base class for payment strategies that provides common functionality.
 */
public abstract class PaymentStrategyBase implements IPaymentStrategy {
    /**
     * Computes the total amount for a purchase.
     */
    protected BigDecimal computeTotalAmount(InventoryItem item, int quantity) {
        return item.getPrice().multiply(BigDecimal.valueOf(quantity));
    }
}
