# Payment Strategy Pattern (Java)

This directory contains the implementation of the **Strategy Pattern** for processing payments in the tea shop.

## Implementing a New Payment Strategy

To implement a new payment strategy, follow these steps:

1. **Inherit from `PaymentStrategyBase`**: All concrete payment strategies should extend the `PaymentStrategyBase`
   abstract class. This ensures access to common functionality like total amount computation.
2. **Define a Constructor**: The constructor should capture any necessary payment-specific information (e.g., account
   details, tokens, or signatures) required to process the transaction.
3. **Override `checkout`**: Implement the `checkout` method to define how the payment is processed. Use the
   `computeTotalAmount` method from the base class to calculate the final price and output the transaction details to
   the provided `Writer`.

### Example: Custom Payment Strategy (`domain.payment` package)

```java
public final class GiftCardStrategy extends PaymentStrategyBase {
    private final String giftCardCode;

    public GiftCardStrategy(String giftCardCode) {
        this.giftCardCode = giftCardCode;
    }

    @Override
    public void checkout(InventoryItem item, int quantity, Writer output) {
        var writer = new PrintWriter(output, true);
        var total = computeTotalAmount(item, quantity);
        writer.println("Checking out " + total + " using Gift Card (Code: " + giftCardCode + ").");
    }
}
```

## Strategy Pattern Structure

The following diagram illustrates the relationship between the strategy interface, the base class, and the concrete
implementations:

```mermaid
---

  config:
    class:
      hideEmptyMembersBox: true
---
classDiagram
    namespace Domain_Payment {
        class IPaymentStrategy {
            <<interface>>
            +checkout(InventoryItem item, int quantity, Writer output)
        }

        class PaymentStrategyBase {
            <<abstract>>
            +checkout(InventoryItem item, int quantity, Writer output)*
            #computeTotalAmount(InventoryItem item, int quantity) BigDecimal
        }

        class CreditCardStrategy {
            -String cardNumber
            +checkout(...)
        }

        class ApplePayStrategy {
            -String appleUsername
            +checkout(...)
        }

        class CryptoCurrencyStrategy {
            -String walletAddress
            -String transactionSignature
            +checkout(...)
        }
    }

    IPaymentStrategy <|.. PaymentStrategyBase
    PaymentStrategyBase <|-- CreditCardStrategy
    PaymentStrategyBase <|-- ApplePayStrategy
    PaymentStrategyBase <|-- CryptoCurrencyStrategy
```

### Relationships Explained:

- **`IPaymentStrategy` (Interface)**: Defines the common contract for all payment methods. This allows the application
  to remain decoupled from specific payment implementations.
- **`PaymentStrategyBase` (Abstract Class)**: Provides a base implementation of the interface and includes shared logic,
  such as `computeTotalAmount`, which reduces duplication across concrete strategies.
- **Concrete Strategies**: Classes like `CreditCardStrategy`, `ApplePayStrategy`, and `CryptoCurrencyStrategy` implement
  the specific details of each payment method while adhering to the `IPaymentStrategy` contract.

## More Realistic Generic Architecture

In a production environment, the payment strategy system would be more robust:

- **Asynchronous Processing**: The `checkout` method would be asynchronous to handle network latency and external API
  calls without blocking the application thread.
- **External API Integration**: Instead of just printing to a `Writer`, strategies would interact with payment
  gateways (e.g., Stripe, PayPal, or crypto processors).
- **Result Objects**: Instead of a `void` return type, `checkout` would return a `PaymentResult` object containing
  transaction IDs, status codes, and error messages.
- **Dependency Injection**: Payment strategies would be registered in a DI container, allowing for easier testing and
  configuration of environment-specific credentials.
