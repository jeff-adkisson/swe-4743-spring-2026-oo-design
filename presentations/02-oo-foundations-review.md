# Lecture 2 Summary - OO Foundations Review

## Example Code from Lecture 2

[https://github.com/jeff-adkisson/swe-4743-spring-2026-oo-design/tree/main/demos/02-foundations-review](https://github.com/jeff-adkisson/swe-4743-spring-2026-oo-design/tree/main/demos/02-foundations-review)


## Why the OO review exists
Even though most students are juniors/seniors and have programmed before, prior experience with OO varies a lot. The goal of this lecture is to get everyone aligned on shared terminology and core mechanics before the class moves into design topics (including UML next).

## The teaching approach: “Program 01 → Program 07”
Jeff built a sequence of demo programs that evolve step-by-step. Each version introduces a new OO concept (or refines one), including:
- Inheritance and abstract base classes
- Polymorphism via base-type references and dynamic dispatch
- Interfaces as contracts
- Encapsulation and controlled access to state
- Organization topics (namespaces/modules) later in the sequence

Most demos are in **C#**, with one converted to **Java** (primarily to show differences in namespacing and related tooling).

---

# Core concepts covered

## 1) Inheritance
**Idea:** One class reuses behavior/state from another, creating an *“is-a”* relationship.

**Demo model:** A `Publication` base type with subclasses like `PaperbackBook` and `Scroll`.

Key points emphasized:
- **Abstract vs concrete**
  - An **abstract class** can’t be instantiated directly (`new Publication()` fails).
  - A **concrete class** can be instantiated (`new PaperbackBook(...)` works).
- **Shared implementation in an abstract class**
  - An abstract base can still contain **fields/properties** and **implemented methods** (e.g., a shared `GetDetails()` in an early version).
- **Constructor chaining**
  - C#: `: base(title)`  
  - Java: `super(title)`
- Design caution: **deep inheritance chains become fragile** (“favor composition over inheritance” is previewed as a later design theme).

## 2) Polymorphism
**Idea:** Code works with many related types through a shared abstract type (abstract class or interface).

A recurring example:
- A list/collection of the **base type** (`Publication` or later `IPublication`) can hold different concrete types.
- Client code calls methods (like `GetDetails`) through the base type, and the runtime chooses the right implementation.

Vocabulary introduced:
- **Dynamic dispatch**: calling a method through a base reference and letting the runtime resolve the concrete implementation.
- **Generics**: e.g., `List<Publication>` (or `List<IPublication>`) — a typed collection.
- C# note: `var` can reduce verbosity; it infers the type from the right-hand side.

## 3) Abstract methods and overriding (Polymorphism “Evolution 2”)
The lecture evolved `GetDetails()` from a shared base implementation to an **abstract method**:
- The base class defines the **required shape** (“contract”) but not the implementation.
- Each subclass must provide its own `GetDetails()` to include its own details (e.g., pages vs scroll length).

## 4) Interfaces (Polymorphism “Evolution 3”)
**Idea:** An interface is a pure contract: “if you implement this, you guarantee these members exist.”

In the demo, the interface (e.g., `IPublication`) included members like:
- `Title`
- `GetDetails()`
- `EstimatedReadingMinutes()`

Emphasis:
- Interfaces help keep client code independent of implementation details.
- Good interfaces are typically **small and stable**. Jeff connected this to Ousterhout’s advice and noted that an interface that “hardly changes” is a strong signal it was designed well.
- Naming convention difference:
  - C#: interfaces typically start with `I` (`IPublication`)
  - Java: no `I` prefix is conventional

## 5) Moving logic out of `Main` and using helpers (Program 3)
As the demo grew, Jeff refactored the program to reduce an oversized `Main` method:
- Split initialization and reporting into static helper methods such as:
  - `InitializePublications(...)`
  - `PrintCollection(...)`
  - `PrintReadingPlan(...)`
  - `PrintShortReads(...)`

Also introduced:
- **Static methods** as organizational tools (with a caution that static state can cause real-world concurrency bugs if misused).
- **LINQ** (C#) as a concise way to query collections (e.g., min/max/sum/filter) — conceptually similar to loops but more expressive.

## 6) Interface inheritance and “collection items” (Polymorphism “Evolution 4”)
The model expanded beyond “things you read” to “things you collect”:
- A new interface (e.g., `ICollectionItem`) captured the most general shape:
  - `Title`
  - `GetDetails()`

Then two branches specialized:
- `IPublication` adds reading-related behavior (e.g., `EstimatedReadingMinutes()`).
- `IArtifact` adds artifact-related behavior (e.g., `Culture`, `Material`).

This allowed a single collection (e.g., `List<ICollectionItem>`) to hold both publications **and** artifacts, while still letting each category add its own specialized behavior.

## 7) Encapsulation, information hiding, and downcasting (Program 5)
Encapsulation was framed as:
- A class controls its own data.
- State is accessed and mutated through controlled methods/properties.
- Access modifiers (`private`, `protected`, etc.) help protect invariants and internal consistency.

A concrete example:
- Reading progress (like “percent read”) exists only for publications, not artifacts.
- If an object is stored as a higher-level type (e.g., `ICollectionItem`), you cannot directly access publication-only members.
- **Downcasting** (casting back to `IPublication`) can expose the more specific API when you *know* the runtime type supports it.

Why *not* put reading-related members in `ICollectionItem`?
- Because artifacts aren’t readable; forcing that API upward would be a design smell. Jeff briefly named this as a **Liskov violation** (to be covered later in SOLID).

---

# What students should take away
- Know the difference between **concrete**, **abstract class**, and **interface** types.
- Be able to explain **polymorphism** as “code that targets a stable abstract shape while behavior varies by concrete type.”
- Understand why **interfaces** are contracts and why small/stable interfaces are desirable.
- Recognize **encapsulation** as information hiding + controlled mutation (using access modifiers and well-chosen methods).
- Understand the tradeoff of **downcasting**: sometimes necessary, but often a cue that you should reconsider how responsibilities and interfaces are structured.

## Assigned reading reminder
Jeff ended by pointing to the assigned reading on the last slide and emphasized Ousterhout’s discussion of complexity (“higher complexity → higher risk and harder maintenance”). Please refer to the final slide in the deck for the exact reading items.
