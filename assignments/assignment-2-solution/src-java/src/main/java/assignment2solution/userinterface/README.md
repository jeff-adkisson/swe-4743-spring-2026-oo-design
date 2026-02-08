# User Interface Layer (Java)

This directory contains the classes and interfaces responsible for managing the user's interaction with the tea shop
application. It coordinates the domain logic (inventory, queries, and payments) and provides a text-based console
interface.

## Adding a New Payment Method

The application uses a factory-like approach to dynamically create payment strategies based on user input. To add a new
payment method:

1. **Implement `IPaymentBuilder`**: Create a new class in the `paymentbuilder` package that implements the
   `IPaymentBuilder` interface.
2. **Implement `getName()`**: Provide a display name for the payment method (e.g., "PayPal").
3. **Implement `createStrategy(...)`**: This method should handle the UI-specific logic for collecting payment details
   from the user (via `Reader`/`Writer`) and then return a concrete instance of an `IPaymentStrategy`.
4. **Register the Builder**: Add your new builder class to the list in `PaymentBuilderListFactory.get()`.

### Example: New Payment Method Builder

```java
public final class PayPalPaymentBuilder implements IPaymentBuilder {
    @Override
    public String getName() {
        return "PayPal";
    }

    @Override
    public IPaymentStrategy createStrategy(Reader input, Writer output) {
        var reader = new BufferedReader(input);
        var writer = new PrintWriter(output, true);
        writer.print("Enter PayPal Email: ");
        var email = reader.readLine();
        // In a real app, you'd validate the email here
        return new PayPalStrategy(email); // Returns a Domain strategy
    }
}
```

## UI Architecture

The following diagram illustrates the relationship between the main application controller, the query builder, and the
payment strategy builders:

```mermaid
---
  config:
    class:
      hideEmptyMembersBox: true
---
classDiagram
    class Application {
        -InventoryRepository repository
        -InventoryQueryBuilder inventoryQueryBuilder
        -List~IPaymentBuilder~ paymentMethods
        +run()
    }

    class InventoryQueryBuilder {
        -InventoryRepository repository
        +build() IInventoryQuery
    }

    class IPaymentBuilder {
        <<interface>>
        +getName() String
        +createStrategy(input, output) IPaymentStrategy
    }

    class PaymentBuilderListFactory {
        <<static>>
        +get() List~IPaymentBuilder~
    }

    Application *-- InventoryQueryBuilder
    Application *-- IPaymentBuilder
    Application ..> PaymentBuilderListFactory : uses
    IPaymentBuilder <|.. CreditCardPaymentBuilder
    IPaymentBuilder <|.. ApplePayPaymentBuilder
```

### Key Components:

- **`Application`**: The central controller that manages the main execution loop, prompts the user to search for items,
  and coordinates the checkout process.
- **`InventoryQueryBuilder`**: Decouples the UI logic for gathering search criteria from the domain's query execution.
  It progressively wraps the `IInventoryQuery` with decorators based on user input.
- **`IPaymentBuilder`**: Bridges the UI and Domain layers. It captures UI-specific input needed for a payment
  method and instantiates the corresponding Domain-level `IPaymentStrategy`.

## UI vs. Domain Separation: `IPaymentBuilder`

A key design goal of this application is the strict separation of **User Interface concerns** from **Domain concerns**.
This is primarily achieved through the `IPaymentBuilder` interface.

### The "Why": Clean Domain Logic

In a well-architected system, the Domain layer (Business Logic) should be "pure." It should not know about:

- How to prompt a user for input (`System.in`).
- How to display messages to a user (`System.out`).
- Which specific UI framework is being used (CLI, Web, Mobile).

If we put UI logic (like asking for a credit card number) directly inside a `CreditCardStrategy.checkout()` method, the
Domain would become tethered to the Console. This makes the code harder to test, reuse in a web application, or modify
without breaking business rules.

### The "How": The Builder Pattern

The `IPaymentBuilder` acts as a **bridge**:

1. **UI Layer (The Builder)**: Classes like `CreditCardPaymentBuilder` live in the `userinterface` layer. They handle
   all the messy details of talking to the user, validating input formats, and re-prompting if necessary.
2. **Handoff**: Once the builder has collected all necessary data (e.g., a validated card number), it instantiates a
   concrete **Domain Strategy**.
3. **Domain Layer (The Strategy)**: The resulting `IPaymentStrategy` (e.g., `CreditCardStrategy`) lives in the `domain`
   layer. It is a "pure" object that contains the logic for processing the payment using the data it was given.

### Architecture Diagram

The following diagram shows how the Builder pattern maintains a clear boundary between the UI and Domain layers:

```mermaid
---
  config:
    class:
      hideEmptyMembersBox: true
---
classDiagram
    namespace UserInterface_PaymentBuilder {
        class Application
        class IPaymentBuilder {
            <<interface>>
            +getName() String
            +createStrategy(input, output) IPaymentStrategy
        }
        class CreditCardPaymentBuilder
    }

    namespace Domain_Payment {
        class IPaymentStrategy {
            <<interface>>
            +checkout(item, quantity, output)
        }
        class CreditCardStrategy
    }

    %% Relationships
    Application --> IPaymentBuilder : asks to create strategy
    Application --> IPaymentStrategy : executes checkout

    CreditCardPaymentBuilder ..|> IPaymentBuilder : implements
    CreditCardPaymentBuilder ..> CreditCardStrategy : creates

    CreditCardStrategy ..|> IPaymentStrategy : implements

    %% Explicit Layering
    note for IPaymentBuilder "Handles I/O and user interaction"
    note for IPaymentStrategy "Handles business logic and invariants"
```

## More Realistic Generic Architecture

In a production-ready application, the User Interface layer would likely be more complex:

- **Separation of Concerns (MVC/MVVM)**: Instead of a single `Application` class handling everything, the UI would be
  split into Controllers/ViewModels and Views.
- **Dependency Injection**: Builders, repositories, and query builders would be injected via
  a DI container, improving testability and flexibility.
- **Pluggable UI**: The domain layer would be entirely independent of the UI. This would allow the same domain logic to
  be used across different UI implementations, such as a Web API (Spring Boot), a Mobile App, or a Desktop App.
- **Validation Layer**: A dedicated validation system would ensure that all user input is sanitized and validated
  against domain rules before reaching the business logic.
