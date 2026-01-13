# Object-Oriented Design – Core Terminology

The following terms represent concepts, patterns, and principles that students are expected to **understand, explain, and apply** by the end of the semester. Each definition is intentionally concise and focused on design intent rather than syntax.

---

- **Abstraction** - Exposing essential behavior while hiding implementation details to reduce complexity.
- **Access Modifiers** - Language constructs that control visibility and encapsulation boundaries (e.g., public, private).
- **Active Record Pattern** - A pattern where domain objects are responsible for their own persistence logic.
- **Adapter Pattern** - Converts one interface into another expected by a client without changing existing code.
- **Aggregate** - A cluster of related domain objects treated as a single consistency boundary.
- **Architecture Decision Record (ADR)** - A short document capturing an important architectural decision and its rationale.
- **Association (UML)** - A relationship between classes indicating interaction or usage.
- **Behavioral Contract** - The expectations a type guarantees regarding valid inputs, outputs, and side effects.
- **Builder Pattern** - Separates complex object construction from its representation.
- **Cohesion** - A measure of how closely related the responsibilities of a module or class are.
- **Command Pattern** - Encapsulates a request as an object, enabling parameterization and decoupling.
- **Composition (over Inheritance)** - Favoring object composition to achieve reuse instead of class inheritance.
- **Coupling** - The degree to which one module depends on another.
- **Cross-Cutting Concern** - A concern that affects multiple parts of a system (e.g., logging, security).
- **Decorator Pattern** - Adds behavior to an object dynamically without modifying its class.
- **Dependency Direction** - The intended flow of dependencies from stable abstractions to volatile details.
- **Dependency Injection (DI)** - Supplying dependencies from the outside rather than creating them internally.
- **Dependency Inversion Principle (DIP)** - High-level modules should not depend on low-level modules; both depend on abstractions.
- **Design Tradeoff** - A conscious decision balancing competing design forces.
- **Domain Event** - A representation of something meaningful that occurred within the domain.
- **Encapsulation** - Bundling data and behavior while restricting direct access to internals.
- **Entity** - A domain object defined by identity rather than by value.
- **Facade Pattern** - Provides a simplified interface to a complex subsystem.
- **Factory Method Pattern** - Defines an interface for object creation while allowing subclasses to decide the concrete type.
- **Framework as a Detail** - The principle that frameworks should adapt to the domain, not define it.
- **Immutability** - The property of an object whose state cannot change after creation.
- **Information Hiding** - Concealing design decisions likely to change behind stable interfaces.
- **Inheritance** - A mechanism for creating new types by extending existing ones.
- **Interface** - A contract specifying behavior without implementation.
- **Interface Segregation Principle (ISP)** - Clients should not depend on interfaces they do not use.
- **Invariant** - A condition that must always hold true for an object or system.
- **Liskov Substitution Principle (LSP)** - Subtypes must be substitutable for their base types without altering correctness.
- **Model–View–Controller (MVC)** - An architectural pattern separating domain logic, presentation, and input handling.
- **Module** - A cohesive unit of code with a clear responsibility boundary.
- **Observer Pattern** - Defines a one-to-many dependency so dependents are notified of state changes.
- **Open/Closed Principle (OCP)** - Software entities should be open for extension but closed for modification.
- **ORM (Object-Relational Mapping)** - A technique for mapping objects to relational database tables.
- **Orthogonality** - System components should be independent and decoupled, so changes in one concern do not affect unrelated parts of the object-oriented design.
- **Persistence Boundary** - The separation between domain logic and data storage concerns.
- **Polymorphism** - Treating related types uniformly through a common interface.
- **Primitive Obsession** - Overuse of primitive types instead of meaningful domain abstractions.
- **Proxy Pattern** - Controls access to another object, often adding behavior such as caching or laziness.
- **Refactoring** - Improving internal structure without changing external behavior.
- **Repository Pattern** - Provides a domain-facing abstraction over persistence mechanisms.
- **Responsibility** - A specific reason for a module or class to change.
- **Single Responsibility Principle (SRP)** - A module or class should have only one reason to change.
- **Singleton Pattern** - Ensures a class has only one instance and provides global access to it.
- **SOLID Principles** - A set of five principles guiding maintainable object-oriented design.
- **State Pattern** - Encapsulates state-dependent behavior into separate state objects.
- **Strategy Pattern** - Defines interchangeable algorithms behind a common interface.
- **Template Method Pattern** - Defines an algorithm skeleton while allowing subclasses to override steps.
- **Temporal Coupling** - A dependency based on timing or order of operations.
- **Testability** - The ease with which code can be verified through automated tests.
- **UML Class Diagram** - A diagram representing classes, attributes, methods, and relationships.
- **Value Object** - An immutable object defined solely by its values, not identity.
- **Visibility Boundary** - A deliberate limit on what parts of a system can access others.
- **Volatility** - The likelihood that a part of the system will change over time.
- **Web Controller (as Adapter)** - A component that translates HTTP concerns into application-level intentions.

---

Students should be able to:
- Define each term in their own words
- Recognize correct and incorrect usage in code
- Explain *why* each concept exists
- Apply these concepts appropriately in design decisions
