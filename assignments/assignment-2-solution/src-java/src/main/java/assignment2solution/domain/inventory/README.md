# Inventory Repository Notes (Java)

The `InventoryRepository` in this package is intentionally simple for this teaching solution.

In a real-world implementation, it would typically:

- Implement full CRUD (Create, Read, Update, Delete) methods.
- Have a generic interface (e.g., `Repository<T>`) to allow for additional repositories (e.g., database-backed,
  external API, etc.).
- Use interfaces for domain types (e.g., `RepositoryItem`) to make operations on the repository interface simple and
  decoupled.
- Provide a base abstract class (e.g., `RepositoryItemBase`) that implements the `RepositoryItem` interface and handles
  the initialization of `itemId` via constructor chaining.

### More Realistic Generic Architecture

The following diagram demonstrates a more realistic implementation of a repository
using a generic interface (`Repository<T>`) with a constraint that `T` must implement `RepositoryItem`. It also shows
the use of an abstract base class `RepositoryItemBase` to provide a common implementation for all repository items.

```mermaid
---

  config:
    class:
      hideEmptyMembersBox: true
---
classDiagram
    class Repository_TItem_ {
        <<interface>>
        +get(UUID itemId) TItem
        +getAll() List~TItem~
        +add(TItem item)
        +update(TItem item)
        +delete(UUID itemId)
    }

    class RepositoryItem {
        <<interface>>
        +UUID itemId
    }

    class RepositoryItemBase {
        <<abstract>>
        +UUID itemId
        #RepositoryItemBase(UUID itemId)
    }

    class InventoryRepository {
        -List~InventoryItem~ items
        +get(UUID id) InventoryItem
        +getAll() List~InventoryItem~
        +add(InventoryItem item)
        +update(InventoryItem item)
        +delete(UUID id)
        +updateQuantity(UUID id, int change)
    }

    class InventoryItem {
        +UUID itemId
        +String name
        +BigDecimal price
        +int quantity
        +StarRating starRating
        +boolean isAvailable
        +BigDecimal totalPrice
        +InventoryItem(UUID itemId, ...)
    }

    %% Generic constraint
    note for Repository_TItem_ "Constraint:\nTItem must implement RepositoryItem"

    %% Relationships
    Repository_TItem_ <|.. InventoryRepository
    RepositoryItem <|.. RepositoryItemBase
    RepositoryItemBase <|-- InventoryItem
    Repository_TItem_ ..> RepositoryItem : generic constraint
    InventoryRepository *-- RepositoryItem
   ```

### Relationships Explained:

- **`Repository<TItem>` (Interface)**: Defines the standard CRUD contract for any repository.
- **`RepositoryItem` (Interface)**: A marker/base interface that ensures any entity used in a repository has a unique
  `itemId`.
- **`RepositoryItemBase` (Abstract Class)**: Implements `RepositoryItem` and provides a base for all domain entities.
  It uses constructor chaining to ensure that `itemId` is properly initialized upon creation.
- **`InventoryRepository` (Concrete Class)**:
    - Implements `Repository<InventoryItem>`, specializing it for inventory management.
    - **Strong Composition (`*--`)**: The repository has a strong composition relationship with `RepositoryItem` (via
      `InventoryItem`). This indicates that the **repository controls the lifespan of the entities** it contains.
      Entities are typically created, retrieved, updated, and deleted through the repository, ensuring data integrity
      and centralized management.
- **`InventoryItem` (Concrete Class)**: Inherits from `RepositoryItemBase`, gaining the `itemId` property and ensuring
  it satisfies the generic constraint of the repository.
- **Generic Constraint (`..>`)**: The `Repository<TItem>` interface depends on `RepositoryItem` as a type constraint,
  ensuring type safety across all repository implementations.
