# Inventory Repository Notes

The `InventoryRepository` in this namespace is intentionally simple for this teaching solution.

In a real-world implementation, it would typically:

- Implement full CRUD (Create, Read, Update, Delete) methods.
- Have a generic interface (e.g., `IRepository<T>`) to allow for additional repositories (e.g., database-backed,
  external API, etc.).
- Use interfaces for domain types (e.g., `IRepositoryItem`) to make operations on the repository interface simple and
  decoupled.
- Provide a base abstract class (e.g., `RepositoryItemBase`) that implements the `IRepositoryItem` interface and handles
  the initialization of `ItemId` via constructor chaining.

### More Realistic Generic Architecture

The following diagram demonstrates a more realistic implementation of a repository
using a generic interface (`IRepository<T>`) with a constraint that `T` must implement `IRepositoryItem`. It also shows
the use of an abstract base class `RepositoryItemBase` to provide a common implementation for all repository items.

```mermaid
---
 
  config:
    class:
      hideEmptyMembersBox: true
---
classDiagram
    class IRepository_TItem_ {
        <<interface>>
        +Get(Guid itemId) TItem
        +GetAll() IReadOnlyList~TItem~
        +Add(TItem item)
        +Update(TItem item)
        +Delete(Guid itemId)
    }

    class IRepositoryItem {
        <<interface>>
        +Guid ItemId
    }

    class RepositoryItemBase {
        <<abstract>>
        +Guid ItemId
        #RepositoryItemBase(Guid itemId)
    }

    class InventoryRepository {
        -List~InventoryItem~ _items
        +Get(Guid id) InventoryItem
        +GetAll() IReadOnlyList~InventoryItem~
        +Add(InventoryItem item)
        +Update(InventoryItem item)
        +Delete(Guid id)
        +UpdateQuantity(Guid id, int change)
    }

    class InventoryItem {
        +Guid ItemId
        +string Name
        +decimal Price
        +int Quantity
        +StarRating StarRating
        +bool IsAvailable
        +decimal TotalPrice
        +InventoryItem(Guid itemId, ...)
    }

    %% Generic constraint
    note for IRepository_TItem_ "Constraint:\nTItem must implement IRepositoryItem\n(C# where T : IRepositoryItem)"

    %% Relationships
    IRepository_TItem_ <|.. InventoryRepository
    IRepositoryItem <|.. RepositoryItemBase
    RepositoryItemBase <|-- InventoryItem
    IRepository_TItem_ ..> IRepositoryItem : generic constraint
    InventoryRepository *-- IRepositoryItem
   ```

### Relationships Explained:

- **`IRepository<TItem>` (Interface)**: Defines the standard CRUD contract for any repository.
- **`IRepositoryItem` (Interface)**: A marker/base interface that ensures any entity used in a repository has a unique
  `ItemId`.
- **`RepositoryItemBase` (Abstract Class)**: Implements `IRepositoryItem` and provides a base for all domain entities.
  It uses constructor chaining to ensure that `ItemId` is properly initialized upon creation.
- **`InventoryRepository` (Concrete Class)**:
    - Implements `IRepository<InventoryItem>`, specializing it for inventory management.
    - **Strong Composition (`*--`)**: The repository has a strong composition relationship with `IRepositoryItem` (via
      `InventoryItem`). This indicates that the **repository controls the lifespan of the entities** it contains.
      Entities are typically created, retrieved, updated, and deleted through the repository, ensuring data integrity
      and centralized management.
- **`InventoryItem` (Concrete Class)**: Inherits from `RepositoryItemBase`, gaining the `ItemId` property and ensuring
  it satisfies the generic constraint of the repository.
- **Generic Constraint (`..>`)**: The `IRepository<TItem>` interface depends on `IRepositoryItem` as a type constraint (
  `where TItem : IRepositoryItem`), ensuring type safety across all repository implementations.
