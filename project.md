# Semester Project:<br/>Campus Event Manager

**Angular SPA · JSON API (C# or Java) · SQLite · OO Design Focus**

---

## 1. Project Goal

Design and implement a **small but realistic web application** that demonstrates **high-quality object-oriented design**. The emphasis is on:

- Clear responsibility boundaries  
- Low coupling and high cohesion  
- Intentional use of OO design patterns  
- Proper use of **Dependency Injection**  
- Code that is easy to understand, extend, and reason about  

This is **not** a security, DevOps, or testing course.

---

## 2. Functional Overview

The system manages **campus clubs and events**.

### Users can:
- View clubs and upcoming events
- View event details
- RSVP to events
- Cancel RSVPs
- See which events they are attending or waitlisted for

### Organizers can:
- Create and edit events
- Publish or cancel events
- Configure event capacity and RSVP deadlines
- Check in attendees

### System behavior:
- Events have lifecycles (Draft → Published → Closed/Cancelled)
- Capacity limits are enforced
- When events are cancelled, affected users are notified (in-app notification is sufficient)

---

## 3. Required Technology

### Frontend
- **Angular** (required)
- Must demonstrate:
  - Smart vs Dumb (Container vs Presentational) components
  - Angular **Facade** pattern
  - Reactive data flow (RxJS at a conceptual level)

### Backend
- **C# or Java** JSON API
- **SQLite** database
- ORM optional:
  - C#: Entity Framework Core
  - Java: Spring Data JPA

---

## 4. Dependency Injection (Required)

The backend **must use Dependency Injection via built-in framework capabilities**.

### Acceptable DI Mechanisms
- **C#**: ASP.NET Core built-in DI
- **Java**: Spring / Spring Boot DI

### Requirements
- Services, repositories, strategies, and handlers must be **constructed via DI**
- Direct use of `new` for dependencies is not allowed outside:
  - Composition root
  - Well-justified factories

Controllers must depend on **interfaces**, not implementations.

---

## 5. Authentication (Intentionally Simplified)

Authentication is **username-only**.

- No passwords
- No hashing
- No JWTs or OAuth
- No encryption requirements

The current user may be selected via:
- A username input
- A dropdown of sample users
- A simple request header (e.g., `X-Username`)

This project **does not evaluate security practices**.

---

## 6. Required Project Organization

### Required C# Project Structure – CampusEvent

This document describes the **required C# solution and project structure** for the *Cohesive Campus* semester project.  
This structure is **mandatory** and will be enforced during grading.

The intent is to enforce:

- Clear responsibility boundaries
- Proper dependency direction
- Isolation of business logic from frameworks
- Correct use of Dependency Injection

---

#### Solution Overview

Your C# solution **must** contain the following projects:

```
CampusEvent.sln
│
├── CampusEvent.Domain
├── CampusEvent.Application
├── CampusEvent.Infrastructure
└── CampusEvent.Api
```

Dependencies **must flow in one direction only**:

```
Domain
  ↑
Application
  ↑
Infrastructure
  ↑
Api
```

Reverse dependencies are **not allowed**.

---

#### 1. CampusEvent.Domain

**Purpose:** Business rules and domain model.

This project contains the **core business logic** of the system and must be completely independent of frameworks, databases, and web concerns.

**Must Contain**

- Entities (with behavior and invariants)
  - `Event`, `Club`, `User`, `Rsvp`, `CheckIn`
- Value Objects (immutable, invariant-enforcing)
  - `EmailAddress`
  - `Capacity`
  - `EventTimeRange`
  - `RsvpDeadline`
- Domain Services
  - Business rules spanning multiple entities
- Domain Events
  - `EventCancelled`, `RsvpCreated`, etc.
- State Pattern implementations
  - `EventState` and legal transitions
- Domain-specific exceptions

**Must NOT Contain**

- ASP.NET Core types
- EF Core or ORM attributes
- Database access logic
- API request/response models
- Configuration or environment logic

**Rule:** This project must compile without referencing any other project.

---

#### 2. CampusEvent.Application

**Purpose:** Application use cases and orchestration.

This project coordinates domain objects to fulfill user actions.

**Must Contain**

- Use Cases / Commands
  - `CreateEvent`
  - `PublishEvent`
  - `CancelEvent`
  - `RsvpToEvent`
- Command Handlers / Application Services
- Interfaces (Ports)
  - `IEventRepository`
  - `INotificationService`
  - `IClock`
- Application-level DTOs
- Factories or Builders (when justified)

**Must NOT Contain**

- Controllers or routing
- ORM mappings or SQL
- Framework configuration
- Service locator logic

**Rule:** Application code depends only on abstractions, never concrete infrastructure.

---

#### 3. CampusEvent.Infrastructure

**Purpose:** Infrastructure and external system implementations.

This project implements interfaces defined in the Application layer.

**Must Contain**

- Repository implementations
  - `SqliteEventRepository : IEventRepository`
- SQLite access (EF Core optional)
- ORM mappings and DbContext (if used)
- Adapters for infrastructure concerns
  - `SystemClock`
  - `InMemoryNotificationService`
- Decorators or Proxies (when appropriate)
- Sample data seeding logic
  - Must create data in multiple meaningful states

**Must NOT Contain**

- Controllers
- Domain business rules
- Use case orchestration

**Rule:** Infrastructure must be replaceable without changing Domain or Application code.

---

#### 4. CampusEvent.Api

**Purpose:** HTTP API and system composition root.

This project adapts HTTP requests to application use cases.

**Must Contain**

- ASP.NET Core controllers
- API request and response models
- Dependency Injection registration
- Minimal authentication logic (username-only)
- Error handling and HTTP status mapping
- CORS configuration for Angular

**Must NOT Contain**

- Domain logic
- Direct database access
- Instantiation of services using `new`

**Rule:** Controllers must be thin. Any significant logic belongs in Application or Domain.

---

#### Dependency Injection Rules

- ASP.NET Core built-in DI **must** be used
- All services, repositories, strategies, and handlers **must be injected**
- The DI container **must only appear in CampusEvent.Api**
- Other projects must not reference the container

---

### Required Java Project Structure – CampusEvent (Maven)

This document describes the **required Java solution and project structure** for the *Cohesive Campus* semester project.  
This structure is **mandatory** and will be enforced during grading.

The intent is to enforce:

- Clear responsibility boundaries
- Proper dependency direction
- Isolation of business logic from frameworks
- Correct use of Dependency Injection
- A consistent, reproducible build via **Maven**

---

#### Maven Requirement (Build System)

Your Java submission **must use Maven**.

- Your repository **must** include a top-level `pom.xml`.
- The project **must** build and run from the command line using Maven (**no** IDE-only builds):
  ```bash
  mvn -q clean verify
  ```
- If you use a multi-module build (recommended), the parent `pom.xml` **must** list all modules and manage dependency versions consistently.

> Maven is required to ensure consistent grading and predictable builds across environments.

---

#### Solution Overview

Your Java solution **must** contain the following Maven modules (same conceptual layout as the C# version):

```
campus-event/                 (repo root)
│
├── pom.xml                   (parent / aggregator POM)
│
├── campus-event-domain/
├── campus-event-application/
├── campus-event-infrastructure/
└── campus-event-api/
```

Dependencies **must flow in one direction only**:

```
domain
  ↑
application
  ↑
infrastructure
  ↑
api
```

Reverse dependencies are **not allowed**.

**Recommended packaging conventions**
Use a single base package such as:
- `edu.yourschool.campusevent` (or similar)

Example per module:
- `edu.yourschool.campusevent.domain.*`
- `edu.yourschool.campusevent.application.*`
- `edu.yourschool.campusevent.infrastructure.*`
- `edu.yourschool.campusevent.api.*`

---

#### 1. `campus-event-domain`

**Purpose:** Business rules and domain model.

This module contains the **core business logic** and must be independent of web, database, and framework concerns.

**Must Contain**
- Entities (with behavior and invariants)
  - `Event`, `Club`, `User`, `Rsvp`, `CheckIn`
- Value Objects (immutable, invariant-enforcing)
  - `EmailAddress`
  - `Capacity`
  - `EventTimeRange`
  - `RsvpDeadline`
- Domain Services
  - Business rules spanning multiple entities
- Domain Events
  - `EventCancelled`, `RsvpCreated`, etc.
- State Pattern implementations
  - `EventState` and legal transitions
- Domain-specific exceptions

**Must NOT Contain**
- Spring / Spring Boot annotations (e.g., `@RestController`, `@Service`, `@Component`)
- ORM annotations (e.g., `@Entity`, `@Table`, `@Column`, `@ManyToOne`, etc.)
- Database access logic
- API request/response models
- Configuration or environment logic

**Rule:** This module must compile without referencing any other project module.

---

#### 2. `campus-event-application`

**Purpose:** Application use cases and orchestration.

This module coordinates domain objects to fulfill user actions and workflows.

**Must Contain**
- Use Cases / Commands
  - `CreateEvent`
  - `PublishEvent`
  - `CancelEvent`
  - `RsvpToEvent`
- Command Handlers / Application Services
- Interfaces (Ports)
  - `EventRepository`
  - `NotificationService`
  - `Clock`
- Application-level DTOs (used between layers)
- Factories or Builders (when justified)

**Must NOT Contain**
- Spring MVC controllers or routing
- ORM mappings or SQL
- Spring configuration classes for wiring
- Service locator logic (no pulling dependencies from Spring context)

**Rule:** Application code depends only on abstractions, never concrete infrastructure implementations.

---

#### 3. `campus-event-infrastructure`

**Purpose:** Infrastructure and external system implementations.

This module implements interfaces defined in the Application layer.

**Must Contain**
- Repository implementations
  - Example: `SqliteEventRepository implements EventRepository`
- SQLite access (ORM optional)
  - Option A: Spring Data JPA (optional)
  - Option B: JDBC / JDBCTemplate / lightweight SQL access (allowed)
- ORM mappings and persistence configuration **only if using an ORM**
  - If using JPA, entity mappings belong here (not in `domain`)
- Adapters for infrastructure concerns
  - `SystemClock implements Clock`
  - `InMemoryNotificationService implements NotificationService` (or simple stub)
- Decorators or Proxies (when appropriate)
- Sample data seeding logic
  - Must create data in multiple meaningful states

**Must NOT Contain**
- Web controllers
- Domain business rules
- Use case orchestration logic

**Rule:** Infrastructure must be replaceable without changing Domain or Application code.

---

#### 4. `campus-event-api`

**Purpose:** HTTP API and system composition root.

This module adapts HTTP requests to application use cases and wires the system together using Spring.

**Must Contain**
- Spring Boot application entry point (e.g., `CampusEventApiApplication`)
- Spring MVC controllers (e.g., `@RestController`)
- API request and response models
- Dependency Injection wiring via Spring
  - Component scanning and/or configuration classes
- Minimal authentication logic (username-only)
  - e.g., `X-Username` header, query param, or simple “select user” endpoint
- Error handling and HTTP status mapping
  - e.g., `@ControllerAdvice`
- CORS configuration for Angular

**Must NOT Contain**
- Domain logic
- Direct database access logic in controllers
- Instantiation of services using `new` inside controllers

**Rule:** Controllers must be thin. Any significant logic belongs in Application or Domain.

---

#### Dependency Injection Rules (Spring)

- Spring Dependency Injection **must** be used
- All services, repositories, strategies, and handlers **must be injected**
- DI wiring and Spring configuration **must only appear in `campus-event-api`**
  - Other modules must not rely on Spring container access
- No service locator patterns (no pulling beans from `ApplicationContext` in application/domain code)

## Final Structure Guidance

If you are unsure where code belongs, ask:

> “Is this business logic, application workflow, infrastructure detail, or web delivery?”

Put the code **where that answer clearly belongs**.

---

## 7. Core Domain Model (Minimum)

### Entities
- Club
- Event
- User
- Rsvp
- CheckIn

### Event States
- Draft
- Published
- Closed
- Cancelled

---

## 8. Required Features

### User Features
- Browse clubs and events
- View event details
- RSVP or cancel RSVP
- See RSVP status (going / waitlisted)

### Organizer Features
- Create, edit, publish, cancel events
- Set capacity and RSVP deadline
- Check in attendees

---

## 9. Sample Data (Required)

The application **must start with pre-populated sample data** including:

- Multiple clubs
- Multiple users
- Events in different states:
  - Draft
  - Published (with space available)
  - Published (full, with waitlist)
  - Closed or Cancelled

- RSVP data showing:
  - Going
  - Waitlisted
  - Cancelled
- At least one event with check-in data

The goal is **immediate usability for grading**.

---

## 10. Required Design Patterns

Use **at least 7** of the following patterns meaningfully:

### Creational
- Factory
- Builder
- Singleton (discouraged; DI-scoped preferred)

### Structural
- Adapter
- Facade
- Proxy
- Decorator

### Behavioral
- Strategy
- State
- Observer
- Command

### Persistence
- Repository (required)

---

## 11. Value Objects (Required)

Define **at least three value objects**, such as:
- EmailAddress
- EventTimeRange
- Capacity
- RsvpDeadline

Value objects must be:
- Immutable
- Invariant-enforcing
- Compared by value

---

## 12. Design Quality Rules

### Backend
- Controllers contain no domain logic
- Domain does not depend on frameworks or ORM APIs
- Repositories are injected abstractions
- Factories must not act as service locators

### Frontend
- Components must not call HttpClient directly
- Business logic must not appear in templates
- API access must go through facades

---

## 13. Deliverables

### Working Application
- Angular SPA
- JSON API backend
- SQLite database
- Sample data loaded on startup

### Design Documentation
- 3–5 Mermaid UML diagrams
- 1 Mermaid Entity Relationship Diagram

### Pattern Evidence Index
A short document describing:
- Which patterns were used
- Where they appear
- Why they were appropriate
- Which patterns were intentionally not used

---

## 14. Docker Requirement (Minimal Scope)

The application must run in a Docker container.

### Requirements
- At least one Dockerfile
- Application runs via:
  ```bash
  docker run
  ```
  or
  ```bash
  docker compose up
  ```
- A single container is acceptable

Docker is used to:
1. Introduce container concepts
2. Simplify building and running your application from the command line (clone and go).

---

## 15. Explicitly Not Required
- Unit tests
- CI/CD pipelines
- Cloud deployment
- Security infrastructure (passwords, hashing, web tokens, etc.)

---

## 16. Evaluation Emphasis

Grades emphasize:
- Cohesion and coupling
- Dependency Injection usage
- Pattern intent and appropriateness
- Clarity and readability
- Ease of running the application

---

## 17. Final Note to Students

> This project rewards **clear thinking and good design judgment**.  
> Fewer features with strong boundaries will score higher than complex but tangled systems.
