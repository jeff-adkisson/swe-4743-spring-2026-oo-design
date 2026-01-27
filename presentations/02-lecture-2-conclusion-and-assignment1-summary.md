# SWE 4743 — *OO Foundations Review (wrap-up) + Assignment 1 walkthrough*   
**Reference links**  

- Slides (PDF): https://jeff-adkisson.github.io/swe-4743-spring-2026-oo-design/presentations/02-oo-foundations-review.pdf  
- Assignment 1: https://jeff-adkisson.github.io/swe-4743-spring-2026-oo-design/assignments/assignment-1.html  

---

## OO foundations review — final topics

> This concludes the content summarized here:
> [Lecture 2 Summary](https://jeff-adkisson.github.io/swe-4743-spring-2026-oo-design/presentations/02-oo-foundations-review.html)

### Encapsulation as “owning state”
The instructor emphasized that classes should **control their own state** and expose information through **intentional methods/properties**, rather than allowing outside code to freely mutate fields or collections.

Key ideas reinforced:

- **Access modifiers matter**: `private`/`protected` are tools to protect invariants and reduce accidental coupling.
- **Abstract classes + interfaces** work together:
  - *Interfaces* define a **contract** (“what you can do”).
  - *Abstract base classes* share common behavior/state (“how we help implementations”).
  - *Concrete classes* provide specific behavior for real types.

### Polymorphism and dynamic dispatch
The discussion returned to a central OO idea: if you program to an interface/base type, the system can call the correct implementation at runtime.

- **Dynamic dispatch**: when calling a method through an abstraction, the runtime selects the most specific implementation (based on the object’s actual type).
- This enables “collection-of-base-types” design (e.g., a list of base types whose concrete types vary).

### Avoid deep inheritance chains (favor composition)
Inheritance can become rigid and awkward as variation increases (“what if a new type doesn’t fit cleanly into the hierarchy?”).  
The instructor previewed an ongoing theme: **favor composition over inheritance** to avoid brittle hierarchies.

### Namespaces and organization
A short wrap-up on how real codebases require structure:

- Namespaces/packages organize a non-trivial codebase into coherent areas.
- Modularization impacts testability (hard-to-test code often correlates with poor structure).

---

## Assignment 1 — what it’s really about

Assignment 1 is presented as a **design-focused warm-up** (even if it feels large). It uses a simplified Crazy Eights console game mainly as a vehicle to practice OO fundamentals.

What students are *actually* being evaluated on:

- **Intentional design choices**, not game sophistication.
- Clear responsibilities, good naming, clean boundaries.
- Demonstrating OO mechanisms correctly and deliberately:
  - interfaces, abstract classes, concrete classes  
  - polymorphism + dynamic dispatch  
  - encapsulation  

[(See the assignment overview and learning objectives.)]( https://jeff-adkisson.github.io/swe-4743-spring-2026-oo-design/assignments/assignment-1.html)

---

## Design guidance emphasized during the walkthrough

### A) “Design around abstractions”
- Use collections of abstractions (e.g., a list of `IPlayer`/`Player`) and let polymorphism drive behavior.
- Avoid type checks like `if (player is HumanPlayer)` / `instanceof` and other “branch-on-type” logic.

### B) Keep interfaces thin (and role-based)
- Interfaces should include only what *every* implementation needs.
- If a method doesn’t apply to all implementations, it likely violates substitutability (previewing the **Liskov Substitution Principle**).

### C) Encapsulation: protect lists, state, and invariants
- Avoid public fields and avoid exposing mutable collections.
- Prefer immutable/read-only views when data must be exposed.
- Ensure state transitions happen through methods (draw, play, discard), not external list mutation.

### D) Composition Root: keep `Main` short and boring
Students were warned about “mile-long main methods.”

- Put wiring/bootstrapping in a clear **composition root** so the rest of the codebase stays testable and maintainable.
- This also reduces “procedural glue code” that spreads game logic into `Main`.

### E) Context objects to avoid long parameter lists
If a method’s parameter list keeps growing, bundle related values into a **context object** (similar to a DTO in some contexts).

### F) Random seeds for repeatability
Passing an explicit random seed enables reproducible behavior, which is valuable for debugging and unit tests.

---

## Repository hygiene and professional habits

- **Use GitHub (source control) early and consistently**; don’t email zip files.
- Add a correct **`.gitignore`** so build artifacts aren’t committed.
- Keep files small and coherent:
  - “No God files” (one huge file with everything).
  - Prefer one class per file (especially for teamwork and merge conflict reduction).
- Write a real **README** with run instructions and include a screenshot.

---

## Docker (why it matters even for a small assignment)

Docker was highlighted as a practical industry tool:

- A Dockerfile can capture the runtime/build environment and make your project runnable for anyone (even if they don’t have Java/.NET installed).
- It helps instructors/testers (and future employers) run your work reliably.
- This reduces “it works on my machine” issues caused by mismatched language/tool versions.

---

## Looking ahead: UML class diagrams

The instructor previewed the next lecture topic and emphasized:

- UML is not mainly about “drawing the arrows.”  
- It’s about **communicating intent**, clarifying responsibilities, and exploring alternatives quickly.
- Class diagrams should focus on a **small set of important classes**, not giant diagrams.
- Terms to pay attention to include:
  - inheritance vs interface realization
  - dependency vs structural relationships
  - association vs aggregation vs composition

---

# Study guide

## Core OO concepts

- **Class / Object**: a blueprint vs a runtime instance.
- **Encapsulation**: controlling access to state; invariants should be protected.
- **Abstraction**: exposing essential behaviors while hiding implementation detail.
- **Interface (contract)**: defines required behaviors; enables substitution and polymorphism.
- **Abstract base class**: partial implementation + shared state/behavior for subclasses.
- **Concrete class**: a final, instantiable implementation.
- **Inheritance**: “is-a” reuse mechanism; can become rigid if overused.
- **Composition**: “has-a” assembly of behavior; often preferred for flexibility.
- **Polymorphism**: using a base type/interface to work with many concrete types.
- **Dynamic dispatch**: runtime selection of the correct overridden method.

## Design principles and practices

- **Single Responsibility Principle (SRP)**: each class should have one reason to change.
- **Open–Closed Principle (OCP)**: open to extension, closed to modification (avoids scattered edits).
- **Liskov Substitution Principle (LSP)**: subtypes must be safely substitutable for base types.
- **Avoid branch-on-type**: `if/else` chains and `instanceof` checks are a design smell when used for behavior selection.
- **Composition Root**: keep `Main` focused on wiring; keep behavior in domain objects.
- **Context object / DTO**: bundle related values instead of growing parameter lists.

## Code smells called out (and why they hurt)

- **God classes / God files**: too much responsibility; hard to understand, test, and merge.
- **Huge `Main` / procedural glue**: pushes behavior outside OO types; reduces testability.
- **Deep inheritance chains**: brittle, hard to extend, encourages awkward hierarchies.
- **Public mutable state** (public fields, setters everywhere, exposed `List`s): makes bugs easier and invariants harder.
- **Switch/if logic for behavior selection**: tends to grow without bound; pushes you away from polymorphism.

## Practical tooling

- **Git + GitHub**: frequent commits, diffs for review, easier collaboration, safer experimentation.
- **`.gitignore`**: prevents committing build artifacts.
- **Docker**: reproducible builds and execution across machines and language stacks.
- **Random seed**: repeatable “random” execution for debugging/tests.

