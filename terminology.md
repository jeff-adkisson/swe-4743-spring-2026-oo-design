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
- **Dynamic Dispatch** - Calling a method through a base reference and letting the runtime resolve the concrete implementation.
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
- **Module** - A cohesive unit of code with a clear responsibility boundary.
- **Open/Closed Principle (OCP)** - Software entities should be open for extension but closed for modification.
- **Polymorphism** - Treating related types uniformly through a common interface.
- **Primitive Obsession** - Overuse of primitive types instead of meaningful domain abstractions.
- **Refactoring** - Improving internal structure without changing external behavior.
- **Responsibility** - A specific reason for a module or class to change.
- **Single Responsibility Principle (SRP)** - A module or class should have only one reason to change.

---

## OO Foundations Terms (Lecture 2)

- **Abstract Class** - A class that cannot be instantiated directly and is intended to be subclassed.
- **Abstract Method** - A method without an implementation that must be overridden by subclasses.
- **Base Type Reference** - A variable typed as an abstract class or interface that refers to a concrete object at runtime.
- **Concrete Class** - A fully implemented class that can be instantiated.
- **Constructor Chaining** - Invoking a base class constructor from a subclass constructor.
- **Downcasting** - Casting from a general type to a more specific type to access additional behavior.
- **Generics** - Type parameters enabling type-safe reuse (e.g., List<T>).
- **Namespace** - A logical grouping mechanism used to organize types.
- **Override** - Providing a subclass-specific implementation of a base method.
- **Runtime Type** - The actual concrete type of an object during execution.
- **Static Method** - A method associated with a class rather than an instance.
- **Type Inference (`var`)** - Compiler inference of a variable’s static type from its initializer.

---

Students should be able to:
- Define each term in their own words
- Recognize correct and incorrect usage in code
- Explain *why* each concept exists
- Apply these concepts appropriately in design decisions
