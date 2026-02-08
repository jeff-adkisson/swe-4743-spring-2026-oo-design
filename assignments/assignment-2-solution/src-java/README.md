# Tea Shop Application (Java)

## Description

The Tea Shop Application is a console-based Java application
that allows users to browse, search, and purchase various types of tea.
It features a basic inventory management system with
filtering and sorting capabilities, and supports multiple
(fake/demo) payment methods (Credit Card, Apple Pay, and CryptoCurrency).

## Screenshot

![image-20260201194309911](README.assets/screenshot.png)

## CLI Execution Instructions

To run the application directly from your terminal, ensure you have a Java 17+ JDK and Maven installed.

1. Navigate to the root directory of the solution.
2. Build the application:
   ```bash
   mvn -q -DskipTests package
   ```
3. Run the application:
   ```bash
   java -jar target/tea-shop.jar
   ```

## Docker Build Instructions

To build and run the application using Docker, you can use the following commands.

**Note:** Ensure you have Docker installed and a `Dockerfile` present in the root directory (or use the one provided
below as a template).

### 1. Build the Docker Image

```bash
docker build -t teashop-java .
```

### 2. Run the Docker Container

Since this is an interactive console application, you need to run it with the `-it` flags:

```bash
docker run -it --rm teashop-java
```

## Unit Test Execution Instructions

The solution includes a comprehensive suite of unit tests using JUnit 5.

To execute all tests, run the following command from the root directory:

```bash
mvn test
```

## Architecture Overview

The project is divided into two main packages:

- **Domain Package**: Contains the core business logic, entities, and rules that define the tea shop's operations.
  Detailed documentation can be found in
  the [Inventory](src/main/java/assignment2solution/domain/inventory/README.md),
  [Payment Strategy](src/main/java/assignment2solution/domain/payment/README.md),
  and [Inventory Query](src/main/java/assignment2solution/domain/inventoryquery/README.md) sub-packages.
- **[UserInterface Package](src/main/java/assignment2solution/userinterface/README.md)**: Manages user interactions,
  console I/O, and coordinates the application's flow.

The project demonstrates object-oriented design patterns such as:

- **Strategy Pattern**: Used for different payment methods. See
  the [Payment Domain README](src/main/java/assignment2solution/domain/payment/README.md) for more details.
- **Decorator Pattern**: Used for building complex inventory queries with filters and sorts. See
  the [Inventory Query Domain README](src/main/java/assignment2solution/domain/inventoryquery/README.md) for more details.
- **Repository Pattern**: Naive implementation to query and modify the tea inventory. See
  the [Inventory Domain README](src/main/java/assignment2solution/domain/inventory/README.md) for more details.
- **Polymorphism and Dependency Injection**: For I/O handling and testability.

## SOLID Principles

- **Single Responsibility Principle (SRP)**: Each class has a well-defined purpose. For example, the `Domain` logic is
  strictly separated from the `UserInterface`, and specific tasks like query construction (`InventoryQueryBuilder` in
  `userinterface.querybuilder`), data
  access (`InventoryRepository`), and output formatting (`InventoryQueryOutputWriter` in
  `userinterface.querybuilder`)
  are handled by dedicated classes.
- **Open/Closed Principle (OCP)**: The system is designed to be easily extendable without modifying existing core logic.
  New payment methods can be added by implementing `IPaymentBuilder`, and new inventory filters can be added by
  creating new `InventoryQueryDecoratorBase` subclasses, both without changing the `Application` or
  `InventoryRepository`
  classes.

## Design Patterns Summary

| Pattern                  | Purpose in this Project                                                   | Key Classes                                                                   |
|:-------------------------|:--------------------------------------------------------------------------|:------------------------------------------------------------------------------|
| **Strategy**             | Decouples payment processing logic from the user interface.               | `IPaymentStrategy`, `CreditCardStrategy`, `IPaymentBuilder`                   |
| **Decorator**            | Dynamically composes complex inventory queries at runtime.                | `IInventoryQuery`, `InventoryQueryDecoratorBase`, `PriceRangeFilterDecorator` |
| **Repository**           | Provides a clean API for data access, hiding the data source.             | `InventoryRepository` <br>(naive/simplistic implementation)                   |
| **Dependency Injection** | Injects `Reader`/`Writer` and Repositories to enable testability.         | `InventoryQueryBuilder`, `Application`                                        |

## What Goes in the Domain Package?

The `domain` package is the heart of the application. It contains the **Domain Logic** (also known as Business
Logic) — the rules, behaviors, and data structures that define how the tea shop operates, independent of how it is
presented to the user or how data is stored.

#### Determining What Belongs in `domain`:

* **Domain Logic (Inside `domain`):**
    * **Core Entities:** Objects like `InventoryItem` that represent real-world concepts.
    * **Business Rules:** Logic that governs how the business works (e.g., how to filter teas, how to calculate prices,
      or the rules for processing a payment strategy).
    * **Abstractions of Core Operations:** Interfaces and base classes for core behaviors, such as `IInventoryQuery` or
      `IPaymentStrategy`.
    * **Stateless Rules:** Pure logic that doesn't depend on specific UI frameworks or external I/O.
    * **Invariants:** Critical business rules that must always be true for an object to be considered valid. For example,
      `StarRating` ensures that a tea's rating can never be outside
      the 1–5 range, preventing "impossible" data from entering the system.

* **Non-Domain Logic (Outside `domain`):**
    * **User Interface (`userinterface`, `userinterface.querybuilder`, `userinterface.paymentbuilder`):** Anything
      related to how the user interacts with the system. This includes
      `System.out.println` calls, parsing user input strings, and managing the flow of the CLI application (e.g.,
      `Application`, `InventoryQueryBuilder`).
    * **Infrastructure/External Concerns:** Code that deals with specific databases, file systems, or network protocols.
      While this project uses a simple in-memory `InventoryRepository`, in a larger system, the implementation details
      of data persistence would live in an `infrastructure` layer.

> **The Golden Rule:** If you changed the application from a Console app to a Web app or a Mobile app, *everything* in
> the `domain` package should remain **unchanged**, while the `userinterface` package would be completely replaced.
> Here the concept is demonstrated by abstracting away all user interaction from the domain logic via `Reader`/
`Writer` and interfaces.

## Extension Points: "How do I add..."

The architecture is designed to be "Open for Extension, but Closed for Modification" (the Open/Closed Principle). For

Detailed instructions on extending the system, please refer to the following guides:

- **[Adding a New Inventory Filter or Sort](src/main/java/assignment2solution/domain/inventoryquery/README.md#implementing-a-new-filter-or-sort-decorator)**:
  Learn how to create new query decorators to extend search capabilities.
- **[Adding a New Payment Method](src/main/java/assignment2solution/userinterface/README.md#adding-a-new-payment-method)**:
  Follow this guide to implement both the UI and Domain components for new payment strategies.
- **[Implementing a New Payment Strategy](src/main/java/assignment2solution/domain/payment/README.md#implementing-a-new-payment-strategy)**:
  Detailed domain-level instructions for the Strategy Pattern.
- **[Evolving the Data Source](src/main/java/assignment2solution/domain/inventory/README.md#inventory-repository-notes)**:
  Considerations for moving beyond the simple in-memory repository.

## Class Diagrams

#### Application Structure

The `Program` class serves as the entry point, while `Application` coordinates the interactions between the user, the
repository, and various builders.

```mermaid
classDiagram
    class Program {
        +main(String[])
    }
    class Application {
        -BufferedReader input
        -PrintWriter output
        -InventoryRepository repository
        -InventoryQueryBuilder inventoryQueryBuilder
        -InventoryQueryOutputWriter inventoryQueryOutputWriter
        -List~IPaymentBuilder~ paymentMethods
        +run()
    }
    class InventoryRepository {
        +get() List~InventoryItem~
        +updateQuantity(UUID, int)
    }
    class InventoryQueryBuilder {
        +build() IInventoryQuery
    }
    class InventoryQueryOutputWriter {
        +write(InventoryQueryOutput)
    }

    Program ..> Application : instantiates
    Application --> InventoryRepository
    Application --> InventoryQueryBuilder
    Application --> InventoryQueryOutputWriter
```

#### Query Decorator

The query system utilizes the Decorator pattern to dynamically compose filtering and sorting logic at runtime. Similar
to the payment strategy, the construction of these queries is separated from the core domain logic:

1. **`InventoryQueryBuilder` (UI Layer - `userinterface.querybuilder`)**: This class handles the user-interactive
   process of gathering search criteria (name,
   price range, etc.) via `Reader`/`Writer`. It then "decorates" a base `AllInventoryQuery` with multiple filter
   and sort decorators based on the user's input.
2. **`IInventoryQuery` (Domain Layer - `domain.inventoryquery`)**: The core interface and its implementations
   (decorators) handle the actual
   execution of the query against the inventory data.

```mermaid
---
config:
  class:
    hideEmptyMembersBox: true
---
classDiagram
    direction TB
    namespace UserInterface_QueryBuilder {
        class InventoryQueryBuilder {
            +build() IInventoryQuery
        }
    }

    namespace Domain_InventoryQuery {
        class IInventoryQuery {
            <<interface>>
            +appliedFiltersAndSorts : List~String~
            +execute() List~InventoryItem~
        }
        class AllInventoryQuery {
            +execute()
        }
        class InventoryQueryDecoratorBase {
            <<abstract>>
            -IInventoryQuery inner
            +execute()
            #decorate()
        }
        class NameContainsFilterDecorator { }
        class PriceRangeFilterDecorator { }
        class SortByPriceDecorator { }
        class AvailabilityFilterDecorator { }
        class SortByStarRatingDecorator { }
        class StarRatingRangeFilterDecorator { }
    }

    InventoryQueryBuilder ..> IInventoryQuery : creates
    IInventoryQuery <|.. AllInventoryQuery
    IInventoryQuery <|.. InventoryQueryDecoratorBase
    InventoryQueryDecoratorBase o-- IInventoryQuery : wraps

    InventoryQueryDecoratorBase <|-- NameContainsFilterDecorator
    InventoryQueryDecoratorBase <|-- PriceRangeFilterDecorator
    InventoryQueryDecoratorBase <|-- AvailabilityFilterDecorator
    InventoryQueryDecoratorBase <|-- SortByPriceDecorator
    InventoryQueryDecoratorBase <|-- SortByStarRatingDecorator
    InventoryQueryDecoratorBase <|-- StarRatingRangeFilterDecorator
```

#### Payment Method Strategy

The application uses the Strategy pattern to decouple the payment processing logic from the user interface.

To maintain a clean separation of concerns, the design uses a "Bridge" of sorts between the User Interface and the
Domain logic:

1. **`IPaymentBuilder` (UI Layer - `userinterface.paymentbuilder`)**: This interface is responsible for the
   user-interactive part of
   selecting a payment method. It handles prompting the user for details (like credit card numbers or wallet addresses)
   via `Reader`/`Writer`. Once the data is collected, it instantiates the appropriate domain strategy.
2. **`IPaymentStrategy` (Domain Layer - `domain.payment`)**: This interface defines the actual execution of the
   payment (the `checkout`
   method). It is "pure" in the sense that it doesn't know how the payment details were gathered; it only knows how to
   process the transaction with the data it was given at construction.

This separation ensures that the domain logic for "how a payment is processed" is not cluttered with "how we ask the
user for their credit card number."

```mermaid
---
config:
  class:
    hideEmptyMembersBox: true
---
classDiagram
    direction LR
    namespace UserInterface_PaymentBuilder {
        class IPaymentBuilder {
            <<interface>>
            +name : String
            +createStrategy(Reader, Writer) IPaymentStrategy
        }
        class CreditCardPaymentBuilder
        class ApplePayPaymentBuilder
        class CryptoCurrencyPaymentBuilder
    }

    namespace Domain_Payment {
        class IPaymentStrategy {
            <<interface>>
            +checkout(InventoryItem, int, Writer)
        }
        class PaymentStrategyBase { <<abstract>> }
        class CreditCardStrategy
        class ApplePayStrategy
        class CryptoCurrencyStrategy
    }

    IPaymentBuilder <|.. CreditCardPaymentBuilder
    IPaymentBuilder <|.. ApplePayPaymentBuilder
    IPaymentBuilder <|.. CryptoCurrencyPaymentBuilder

    IPaymentStrategy <|.. PaymentStrategyBase
    PaymentStrategyBase <|-- CreditCardStrategy
    PaymentStrategyBase <|-- ApplePayStrategy
    PaymentStrategyBase <|-- CryptoCurrencyStrategy

    CreditCardPaymentBuilder ..> CreditCardStrategy : creates
    ApplePayPaymentBuilder ..> ApplePayStrategy : creates
    CryptoCurrencyPaymentBuilder ..> CryptoCurrencyStrategy : creates
```

## I/O Abstraction for Testing and Polymorphism

The application demonstrates a powerful technique for handling User Interface I/O by injecting `Reader` and
`Writer` abstractions instead of relying directly on `System.in` and `System.out`.

This approach provides several key benefits:

1. **Polymorphic Behavior**: At runtime, the application passes `System.in`/`System.out` to the UI components.
   However, the components themselves only care that they are working with *any* `Reader` or `Writer`.
2. **Support for Unit Testing**: By abstracting I/O, we can easily unit test interactive console logic. In tests, we
   inject `StringReader` to simulate user input and `StringWriter` to capture and verify the application's output, all
   without needing to interact with the actual system console.
3. **Separation of Concerns**: The domain logic and UI builders remain agnostic of the specific I/O device, making the
   code more flexible and easier to maintain.

#### Example: Injecting I/O in `InventoryQueryBuilder`

```java
public final class InventoryQueryBuilder {
    private final BufferedReader input;
    private final PrintWriter output;

    public InventoryQueryBuilder(InventoryRepository repository, Reader input, Writer output) {
        this.repository = repository;
        this.input = new BufferedReader(input);
        this.output = new PrintWriter(output, true);
    }

    public IInventoryQuery build() {
        output.print("* Tea name contains: ");
        var name = input.readLine();
        // ... build query ...
    }
}
```

#### Example: Unit Testing with `StringReader`/`StringWriter`

```java
@Test
public void inventoryQueryBuilderBuildReturnsConfiguredQuery() {
    var input = new StringReader("Green\nY\n10\n20\n4\n5\nD\nA");
    var output = new StringWriter();
    var builder = new InventoryQueryBuilder(repository, input, output);

    var query = builder.build();

    assertTrue(query.getAppliedFiltersAndSorts().get(0).contains("Green"));
    assertTrue(output.toString().contains("Tea name contains"));
}
```
