# UML Class Diagramming – Lecture Summary

This summary distills the key ideas from the UML Class Diagramming lecture.  
It is intended as a **study and review guide**, not a replacement for the full materials.

**Course:** SWE 4743 – Object-Oriented Design  
**Instructor:** Jeff Adkisson  

## Primary References

- **Slides (PDF):**  
  [https://jeff-adkisson.github.io/swe-4743-spring-2026-oo-design/presentations/03-uml-class-diagramming.pdf](https://jeff-adkisson.github.io/swe-4743-spring-2026-oo-design/presentations/03-uml-class-diagramming.pdf)

- **Lecture Notes (HTML / Markdown):**  
  [https://jeff-adkisson.github.io/swe-4743-spring-2026-oo-design/presentations/03-uml-class-diagramming.html](https://jeff-adkisson.github.io/swe-4743-spring-2026-oo-design/presentations/03-uml-class-diagramming.html)
  
- **Loom Video (Jan 26, 2026):**  
  [https://www.loom.com/share/18b3c590eeb2499093f4d1d29dec254e](https://www.loom.com/share/18b3c590eeb2499093f4d1d29dec254e)

---

## Big Picture: What UML Class Diagrams Are (and Are Not)

UML class diagrams are a **design and communication tool**, not an execution model.

They:
- Describe **static structure**, not runtime behavior
- Show **classes, interfaces, and relationships**
- Capture a **snapshot of design intent**
- Help teams reason about structure *before code hardens*

They do **not**:
- Show program flow
- Represent execution order
- Replace code or detailed documentation

> If you are thinking about *what runs first*, you are using the wrong diagram.

---

## Why We Diagram Before (and Alongside) Code

Class diagrams are cheaper to change than code and easier to reason about in groups.

They help you:
- Make ownership and dependency decisions explicit
- Reveal responsibilities early
- Explore alternatives without refactoring
- Communicate intent with less noise than code

Diagrams are especially useful for:
- Design discussions
- Code reviews and pull requests
- Teaching and explaining abstractions
- Preserving architectural intent over time

---

## Structural vs. Non-Structural Relationships

A core theme of the lecture is **understanding what kind of relationship you are modeling**.

### Non-Structural (Usage / Temporal)

These relationships:
- Exist only during method execution
- Live on the call stack
- Do not affect object state

**UML representation:** Dependency  
**Code signal:** Method parameters, local variables

> Mental model: *“Used briefly.”*

---

### Structural Relationships

Structural relationships define the *shape* of the system.

#### Type-Level Structural
- Affect the type system
- Exist at compile time
- Do not imply ownership or lifetime

Examples:
- Inheritance (is-a)
- Interface realization (implements)

> Mental model: *“Part of what this system is.”*

---

#### State-Level Structural
- Affect runtime object state
- Represented by fields
- Survive across method calls

Examples:
- Association
- Aggregation
- Composition

> Mental model: *“Part of what this object owns or retains.”*

---

## Understanding Associations, Aggregation, and Composition

These are the **most misunderstood UML relationships**.

### Association
- Objects know about each other
- No ownership implied
- Lifetime is independent

Use when ownership is not central to the discussion.

---

### Aggregation
- Weak ownership / borrowed relationship
- Object is supplied from outside
- Lifetimes are independent

> Mental model: *“I borrow it.”*

---

### Composition
- Strong ownership
- Object is created internally
- Lifetimes are bound together

> Mental model: *“If the owner dies, the part dies too.”*

---

### Practical Classification Guide

Ask these questions in order:

1. Is it only used as a method parameter?  
   → **Dependency**

2. Is it stored as a field?  
   → **Association**

3. Was it created internally?  
   → **Composition**

4. Was it supplied externally but retained?  
   → **Aggregation**

---

## Interfaces vs. Concrete Dependencies

Interfaces express **capability without commitment**.

Good design favors:
- Depending on abstractions
- Making interfaces visible in diagrams
- Showing multiple realizations where appropriate

This makes patterns like **Strategy** and **Observer** much easier to reason about visually.

---

## Multiplicity Is a Design Constraint

Multiplicity is **not documentation trivia**.

Key rule:
> UML multiplicity means nothing unless it is enforced in code.

- Collections do not enforce multiplicity
- Constructors and guard clauses do

Multiplicity communicates **intent**, but code must enforce **reality**.

---

## Responsibility Boundaries

Class diagrams make responsibility problems visible.

Red flags include:
- Classes with many relationships
- God classes that “do everything”
- Dense, tangled diagrams

Each class box invites questions:
- What does this class know?
- What does it do?
- What should it *not* do?

Diagrams often expose responsibility issues before code reviewers notice them.

---

## Focused Diagrams Beat Giant Diagrams

Good UML diagrams are:
- Selective
- Purpose-driven
- Limited to ~5–10 related classes

Create multiple diagrams for different concerns:
- Core domain logic
- Player behavior
- Infrastructure or setup

> UML is a conversation aid, not a blueprint.

---

## UML in Practice (Industry Reality)

- UML diagrams are rarely implemented *exactly*
- Fine-grained distinctions (especially aggregation vs. composition) are often ignored
- Code evolves faster than diagrams

Ways to reduce mismatch:
- Prefer plain association unless lifetime matters
- Add notes when ownership is important
- Enforce decisions in code
- Keep diagrams small and updated

---

## Why Mermaid Is Used in This Course

Mermaid diagrams:
- Are plain text and versionable
- Live alongside code
- Render directly in GitHub Pages
- Are readable by both humans and AI
- Support diffing and code review

> If a design cannot live in Git, it is unlikely to stay current.

---

## Key Takeaways

- UML class diagrams support **design thinking**
- They expose dependencies and abstractions
- They improve design discussions
- They make bad design obvious early
- Clarity matters more than completeness

---

# Study Guide: Terminology to Know

### Core Concepts
- Class diagram
- Static vs. behavioral diagrams
- Design intent
- Responsibility
- Abstraction

### Type-Level Relationships
- Inheritance (is-a)
- Interface
- Interface realization
- Polymorphism
- Substitutability

### State-Level Relationships
- Association
- Aggregation
- Composition
- Ownership
- Object lifetime

### Non-Structural Relationships
- Dependency
- Temporal usage
- Call stack vs. object state

### UML Notation
- Solid line vs. dashed line
- Hollow triangle
- Open diamond
- Filled diamond
- Access modifiers (+, -, #)
- Multiplicity (0..*, 1, 1..n)

### Design Principles Reinforced
- Single Responsibility Principle
- Program to an interface
- Prefer composition over inheritance (when appropriate)
- Enforce design constraints in code

---

## Final Advice

If your diagram requires a long explanation, simplify it.

A good UML class diagram should make:
- Structure obvious
- Responsibilities clear
- Ownership decisions visible

Used well, UML leads to **better designs with fewer refactors**.
