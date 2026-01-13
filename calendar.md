# SWE 4743: Object Oriented Design Calendar 

#### Kennesaw State University College of Computing and Software Engineering Software Engineering

## Week 1

### Monday, January 12

**Syllabus Review**

### Wednesday, January 14

### Lecture 1 – OO Foundations Review

- Polymorphism, encapsulation, inheritance, interfaces
- Access modifiers and visibility boundaries
- Classes, fields, methods, properties
- Primitive vs abstract types and early warning signs of primitive obsession
- Namespaces, modules, and basic organizational structure

**Assignment 1 – OO Foundations Practice**  

**Reading for Week 1**

- *A Philosophy of Software Design* - Ousterhout  
  - Preface  
  - Chapter 1: Introduction  
  - Chapter 2: The Nature of Complexity

---

## Week 2

### Monday, January 19

**Holiday**

### Wednesday, January 21

### Lecture 2 – UML Class Design with Mermaid

- Reading UML diagrams for understanding designs and patterns
- Associations, inheritance, composition, interfaces
- Mermaid syntax embedded in Markdown
- Why large, detailed UML diagrams are rarely useful in industry

**Reading for Week 2**

- *A Philosophy of Software Design* - Ousterhout  
  - Chapter 3: Working Code Is Not Enough

---

## Week 3

### Monday, January 26

### Lecture 3 – SOLID Overview + Single Responsibility Principle

- Responsibilities vs reasons-to-change
- SRP (single-responsibility principle) as a cohesion rule
- How SRP violations lead to cascading changes
- Preview of value objects as a cohesion tool
- Orthogonality: designing responsibilities that do not overlap or interfere

### Wednesday, January 28

### Lecture 4 – Open/Closed Principle + Strategy Pattern

- Designing for extension rather than modification
- Strategy as the primary OCP mechanism
- Eliminating flag arguments and conditional logic
- Identifying variation points early

**Assignment 2 – SRP & Strategy Refactor**  
Students refactor a class that violates SRP and OCP by introducing one or more strategies, focusing on design reasoning and tradeoffs.

**Reading for Week 3**

- *A Philosophy of Software Design* - Ousterhout  
  - Chapter 4: Modules Should Be Deep  
  - Chapter 5: Information Hiding (and Leakage)
- *Head First Design Patterns*  
  - Strategy

---

## Week 4

### Monday, February 2

### Lecture 5 – Liskov Substitution Principle

- Behavioral contracts and substitutability
- How inheritance breaks silently
- Preconditions, postconditions, and exception contracts
- Composition as a safer alternative

### Wednesday, February 4

### Lecture 6 – Interface Segregation + Deep Modules

- Client-specific interfaces
- Avoiding fat service abstractions
- Deep modules with simple public surfaces
- Relating ISP to information hiding

**Reading for Week 4**

- *A Philosophy of Software Design* - Ousterhout  
  - Chapter 6: General-Purpose Modules Are Deeper  
  - Chapter 7: Different Layer, Different Abstraction  
  - Chapter 8: Pull Complexity Downwards

---

## Week 5

### Monday, February 9

### Lecture 7 – Dependency Inversion + Dependency Injection

- Dependency direction rules
- Stable abstractions vs volatile details
- Constructor injection vs service locator
- DI containers in Java and C#
- Testability as a design feedback mechanism
- Controllers depend on abstractions (adapters, not coordinators)

### Wednesday, February 11

### Lecture 8 – Cohesion & Coupling at Class and Module Level

- Cohesion as a design heuristic
- Temporal, logical, and data coupling
- Package-by-feature vs package-by-layer
- Avoiding cyclic dependencies at module scale

**Assignment 3 – Dependency Boundaries**  
Students will refactor an existing object-oriented codebase that uses a fat service and service-locator pattern into a design that applies Interface Segregation, deep modules with simple public surfaces, and Dependency Inversion with constructor-based Dependency Injection. The refactor must also improve cohesion and reduce coupling by organizing code package-by-feature, eliminating cyclic dependencies, and clearly separating stable abstractions from volatile implementation details.

**Reading for Week 5**

- *A Philosophy of Software Design* - Ousterhout  
  - Chapter 9: Better Together or Better Apart?  
  - Chapter 10: Define Errors Out of Existence?

---

## Week 6

### Monday, February 16

**Exam 1 – Written, In-Person**

### Wednesday, February 18

### Lecture 9 – Design It Twice

- Comparing multiple designs
- Tradeoff analysis
- Architecture Decision Records (ADRs)
- Avoiding premature commitment

**Reading for Week 6**

- *A Philosophy of Software Design* - Ousterhout  
  - Chapter 11: Design It Twice

---

## Week 7

### Monday, February 23

### Lecture 10 – Factory Patterns

- Factory Method vs Abstract Factory
- Creation as a volatility point
- Decoupling clients from construction logic
- Supporting extensibility and testability

### Wednesday, February 25

### Lecture 11 – Builder Pattern + Immutability

- Complex object construction
- Enforcing invariants
- Readable object creation
- Immutability as a design tool

**Assignment 4 – Object Creation Patterns**  
Students refactor construction logic using Factory and/or Builder patterns with an emphasis on clarity and invariants.

**Reading for Week 7**

- *Head First Design Patterns*  

  - Factory Method, Abstract Factory, Builder  

- *A Philosophy of Software Design* - Ousterhout  

  - Chapter 12: Why Write Comments? The Four Excuses  

  - Chapter 13: Comments Should Describe Things That Are Not Obvious  

  - Chapter 15: Write the Comments First

---

## Week 8

### Monday, March 2

### Lecture 12 – Singleton (Cautionary)

- Why Singleton is common
- Hidden coupling and global state
- Testing and lifecycle issues
- Safer alternatives using DI

### Wednesday, March 4

### Lecture 13 – Adapter, Facade, and Proxy (Compared)

- Adapter for interface compatibility
- Facade for simplifying subsystems
- Proxy for access control, lazy loading, caching
- Comparing responsibilities and intent

**Reading for Week 8**

- *Head First Design Patterns*  

  - Singleton, Adapter, Facade, Proxy  

- *A Philosophy of Software Design* - Ousterhout  

  - Chapter 14: Choosing Names  

  - Chapter 18: Code Should Be Obvious

---

## Week 9

### Monday, March 9

**Holiday**

### Wednesday, March 11

**Holiday**

---

## Week 10

### Monday, March 16

### Lecture 14 – Decorator Pattern

- Adding behavior without inheritance
- Cross-cutting concerns
- Decorator vs Proxy
- Composability and ordering

### Wednesday, March 18

### Lecture 15 – Strategy vs Template Method

- Strategy as preferred default
- Template Method tradeoffs
- Migration strategies

**Assignment 5 – Behavioral Composition**  
Students implement and compare Strategy and Template solutions and justify design choices.

**Reading for Week 10**

- *Head First Design Patterns*  
  - Decorator, Proxy, Strategy, Template Method 

---

## Week 11

### Monday, March 23

### Lecture 16 – Observer Pattern

- Event-driven decoupling
- Domain vs UI events
- Avoiding event sprawl

### Wednesday, March 25

### Lecture 17 – Command Pattern

- Encapsulating requests
- Queuing and logging
- Reducing temporal coupling
- Undo and Redo using Command

**Assignment 6 – Events and Commands**  
Students refactor a tightly coupled workflow using Observer and/or Command patterns.

**Reading for Week 11**

- *Head First Design Patterns*  
  - Observer, Command  

---

## Week 12

### Monday, March 30

**Exam 2 – Written, In-Person**

### Wednesday, April 1

### Lecture 18 – State Pattern + Invariants

- State-dependent behavior
- Eliminating conditionals
- Enforcing invariants
- State diagramming via Mermaid

**Reading for Week 12**

- *Head First Design Patterns*  
  - State

---

## Week 13

### Monday, April 6

### Lecture 19 – Value Objects & Primitive Obsession

- Value objects and equality
- Encapsulation of validation
- Readability and correctness

### Wednesday, April 8

### Lecture 20 – Web Architecture & MVC

- MVC review
- Controllers as adapters (no business logic)
- Angular SPA + API interaction

**Assignment 7 – Value Objects & MVC Boundaries**

Students refactor a small web-style codebase to replace primitive-heavy models with **value objects** and to restore clear **MVC boundaries**, ensuring that business rules live in the domain rather than in controllers or views. 

**Reading for Week 13**

- https://martinfowler.com/eaaDev/uiArchs.html

---

## Week 14

### Monday, April 13

### Lecture 21 – Repository Pattern & ORMs

- Persistence boundaries

- Repository vs Active Record

- Controllers exist to keep HTTP out of your application, just like repositories exist to keep the database out of your domain

- ORM pitfalls: hidden behavior (“magic”), inefficient data access, performance surprises,

   and misuse of ORMs for complex queries better handled by SQL/CTEs

### Wednesday, April 15

### Lecture 22 – OO Design in Angular & SPA Architecture

- Smart vs Dumb Components
- Angular Facades
- Reactive State
- SPA anti-patterns

**Reading for Week 15**

- https://martinfowler.com/eaaCatalog/repository.html
- https://en.wikipedia.org/wiki/Active_record_pattern
- https://martinfowler.com/bliki/OrmHate.html
- https://angular.dev/overview
- https://medium.com/@dan_abramov/smart-and-dumb-components-7ca2f9a7c7d0

---

## Week 15

### Monday, April 20

**Project Helpdesk (No Lecture)**

### Wednesday, April 22

**Project Helpdesk (No Lecture)**

**Reading for Week 15 OPTIONAL / RECOMMENDED**

- *A Philosophy of Software Design- Ousterhout*

  - Chapter 16, Modifying Existing Code

  - Chapter 17, Consistency

- https://refactoring.com/
- https://martinfowler.com/bliki/StranglerFigApplication.html
- https://www.joelonsoftware.com/2000/04/06/things-you-should-never-do-part-i/

## Week 16

### Monday, April 27

**Project Helpdesk (No Lecture)**

### Wednesday, April 29

**Project Helpdesk (No Lecture)**

**Reading for Week 16 OPTIONAL / RECOMMENDED**

- *A Philosophy of Software Design* - Ousterhout 

  - Chapters 19, Software Trends

  - Chapter 20, Designing for Performance

  - Chapter 21, Conclusion

---

## Week 17

### Monday, May 4

**Project Helpdesk (No Lecture)**

### Wednesday, May 6

**Semester Project Due by 11:59 PM EST**