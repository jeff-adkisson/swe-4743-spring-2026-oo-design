# Smart Home Simulator

The Smart Home Simulator is a semester project for SWE 4743 Object-Oriented Design. The application simulates a smart home environment where users can monitor and control various household devices through a web-based interface. The project emphasizes rigorous application of OO design principles, SOLID architecture, and formal state machine modeling.

Students will implement a full-stack application consisting of a modern single-page application (SPA) front end communicating with a RESTful back-end API. The project serves as a vehicle for demonstrating mastery of object-oriented design patterns, clean architecture, and software engineering best practices.

## Table of Contents

- [1. Functional Requirements](#1-functional-requirements)
  - [1.1 Device Types](#11-device-types)
  - [1.2 Device Metadata](#12-device-metadata)
  - [1.3 Environment Simulation](#13-environment-simulation)
  - [1.4 Simulation Settings](#14-simulation-settings)
- [2. Technical Requirements](#2-technical-requirements)
  - [2.1 Technology Stack](#21-technology-stack)
  - [2.2 Persistence](#22-persistence)
  - [2.3 State Machines](#23-state-machines)
  - [2.4 API Design](#24-api-design)
  - [2.5 Repository Structure](#25-repository-structure)
- [3. User Interface Requirements](#3-user-interface-requirements)
  - [3.1 Device Dashboard](#31-device-dashboard)
  - [3.2 Filtering](#32-filtering)
  - [3.3 Device Controls](#33-device-controls)
  - [3.4 Responsive / Mobile-Friendly Design](#34-responsive--mobile-friendly-design)
  - [3.5 Device Management](#35-device-management)
- [4. Design Requirements](#4-design-requirements)
  - [4.1 Separation of Concerns](#41-separation-of-concerns)
  - [4.2 SOLID Principles](#42-solid-principles)
  - [4.3 Dependency Injection](#43-dependency-injection)
  - [4.4 Abstraction Quality](#44-abstraction-quality)
  - [4.5 Design Pattern Identification](#45-design-pattern-identification)
  - [4.6 Validation Strategy](#46-validation-strategy)
  - [4.7 Error Handling Contract](#47-error-handling-contract)
- [5. Development Requirements](#5-development-requirements)
  - [5.1 Testing](#51-testing)
- [6. Delivery Requirements](#6-delivery-requirements)
  - [6.1 Setup and Run Instructions](#61-setup-and-run-instructions)
  - [6.2 Docker](#62-docker)
  - [6.3 Local Development (Optional Documentation)](#63-local-development-optional-documentation)
  - [6.4 Video Deliverables](#64-video-deliverables)
  - [6.5 Team Policy](#65-team-policy)
  - [6.6 Grading Criteria](#66-grading-criteria)
- [7. Extra Credit](#7-extra-credit)
  - [7.1 ORM](#71-orm)
  - [7.2 Server-Sent Events (SSE)](#72-server-sent-events-sse)
  - [7.3 LLM Natural Language Control via MCP](#73-llm-natural-language-control-via-mcp)
  - [7.4 JWT Authentication](#74-jwt-authentication)
  - [7.5 Device Scenes](#75-device-scenes)
  - [7.6 CI/CD Pipeline](#76-cicd-pipeline)
- [8. Glossary](#8-glossary)

---

## 1. Functional Requirements

### 1.1 Device Types

The simulator supports the following device types. Each device type is governed by a formal state machine with defined states and transitions.

Devices fall into two categories:

- **Powered devices** -- Have an explicit Off/On power state. The device's functional substates (e.g., brightness, speed, heating mode) are only meaningful when the device is On. Examples: Light, Fan, Thermostat.
- **Latch devices** -- Are always energized and have no power state. Their state machine operates entirely at the substate level (e.g., locked/unlocked). These devices are always considered "on" for filtering purposes. Examples: Door Lock.

**Note:** In a production smart home system, devices would also track an "online/connected" state to indicate whether the physical device is reachable on the network. This simulator does not require online/connected tracking -- all devices are assumed to be always reachable. Adding connectivity state would introduce complexity that is outside the scope of this project.

#### 1.1.1 Light

| Property | Detail |
|---|---|
| States | Off, On |
| Attributes | Brightness (10%--100%), Color (RGB value) |
| Transitions | Off -> On, On -> Off |
| Controls | Toggle power, set brightness, set color |
| "On" condition | State is On |

- Brightness is an integer percentage clamped to the range [10, 100].
- Color is represented as an RGB value (e.g., `#FF8800` or `{ r, g, b }`).
- Brightness and color may only be changed while the light is On.

#### 1.1.2 Fan

| Property | Detail |
|---|---|
| States | Off, On |
| Attributes | Speed (Low, Medium, High) |
| Transitions | Off -> On, On -> Off |
| Controls | Toggle power, set speed |
| "On" condition | State is On |

- Speed may only be changed while the fan is On.
- Default speed when turning on is Medium (if no prior speed is persisted).

#### 1.1.3 Thermostat

| Property | Detail |
|---|---|
| States | Off, Idle, Heating, Cooling |
| Attributes | Mode (Heat, Cool, Auto), Desired Temperature, Ambient Temperature |
| Transitions | Off -> Idle (on power-on), Idle -> Heating, Idle -> Cooling, Heating -> Idle, Cooling -> Idle, any -> Off |
| Controls | Toggle power, set mode, set desired temperature |
| "On" condition | State is Heating or Cooling |

- Desired temperature is clamped to the range [60, 80] (Fahrenheit).
- **Modes:**
  - **Heat** -- System may only heat. Transitions to Heating when ambient < desired.
  - **Cool** -- System may only cool. Transitions to Cooling when ambient > desired.
  - **Auto** -- System automatically heats or cools based on ambient vs. desired temperature.
- **Temperature simulation:** While in the Heating or Cooling state, the ambient temperature changes by 1 degree every 5 seconds toward the desired temperature. When ambient equals desired, the thermostat transitions to Idle.
- A thermostat in Idle state is **not** considered "on" for UI filtering purposes.
- **Invariant:** There may be only **one thermostat per location**. The API must enforce this constraint when registering a new thermostat and return an appropriate error if violated.

#### 1.1.4 Door Lock (Latch Device)

| Property | Detail |
|---|---|
| Category | Latch (always energized, no power state) |
| States | Locked, Unlocked |
| Transitions | Locked -> Unlocked, Unlocked -> Locked |
| Controls | Toggle lock |
| "On" condition | Always on |

- Door locks are **latch devices** -- they have no Off state and are always considered "on" for UI filtering purposes.
- The lock's state (Locked/Unlocked) represents the device's substate, not a power condition.

### 1.2 Device Metadata

Every device has the following metadata:

| Field | Type | Description |
|---|---|---|
| `id` | UUID / GUID | Unique identifier |
| `name` | string | Human-readable device name (e.g., "Living Room Overhead") |
| `location` | string | Room or area (e.g., "Kitchen", "Master Bedroom") |
| `type` | enum | Light, Fan, Thermostat, DoorLock |

### 1.3 Environment Simulation

Each location that contains a thermostat has an **ambient temperature** that represents the external or environmental temperature of that room. This ambient temperature drives the thermostat's behavior -- it is what the thermostat "sees" and reacts to.

- The ambient temperature for each thermostat's location is configurable via the API and the UI.
- **Example scenario:** A reviewer sets the ambient temperature of the "Living Room" to 90°F. The thermostat (in Cool or Auto mode, with a desired temperature of 72°F) detects that the ambient temperature exceeds the desired temperature, transitions to Cooling, and begins reducing the ambient temperature by 1 degree every 5 seconds until it reaches 72°F, at which point it transitions to Idle.
- The ambient temperature is persisted alongside device state and survives application restart.
- When a thermostat is Off, changes to the ambient temperature are still tracked but do not trigger state transitions. When the thermostat is powered on, it evaluates the current ambient temperature against the desired temperature and transitions accordingly.

The environment simulation is what makes the thermostat meaningful -- without it, the thermostat has no external stimulus to react to.

### 1.4 Simulation Settings

The UI must provide a **simulation settings panel** (accessible from the header) with the following controls:

| Setting | Description |
|---|---|
| **Ambient temperature per location** | Set the ambient temperature for each location that contains a thermostat. This is the primary mechanism for driving thermostat behavior during testing and demos. |
| **Simulation speed** | A multiplier (1x, 2x, 5x, 10x) that controls the thermostat tick rate. At 1x the thermostat changes 1°F every 5 seconds. At 10x it changes 1°F every 0.5 seconds. This allows reviewers to observe thermostat behavior without long waits. The current simulation clock and active speed multiplier are displayed in the header. The clock advances faster when the multiplier is above 1x, providing a clear visual indicator that the simulation is accelerated. |
| **Reset all devices** | Returns all devices to their factory default states (powered devices Off, door locks Unlocked, thermostats Off with default temperatures). Persisted state is overwritten. Useful for testing and grading. |

- Simulation settings are exposed via the API as well (e.g., `PUT /api/simulation/speed`, `POST /api/simulation/reset`).
- The simulation speed setting does **not** need to be persisted -- it can default to 1x on application restart.

## 2. Technical Requirements

### 2.1 Technology Stack

#### 2.1.1 Front End

The UI will be implemented using one of the following frameworks (student's choice):

- [**Angular**](https://angular.dev) (latest stable)
- [**React**](https://react.dev) (latest stable)

#### 2.1.2 Component Library

The UI must use a commercial or open-source component library for layout, form controls, data display, and theming. Hand-rolled HTML/CSS for standard UI elements is not acceptable. All of the libraries below are available at no cost and will ensure your project looks great on both desktop and mobile devices. Commercial component libraries provide massive productivity enhancements and help you avoid wasting valuable time.

**Recommended:**

- [**PrimeNG**](https://primeng.org) (Angular) -- rich component set with built-in theming, data tables, form controls, and layout primitives
- [**PrimeReact**](https://primereact.org) (React) -- same breadth of components as PrimeNG for the React ecosystem

The Prime libraries are recommended because they offer a wider range of components out of the box, which reduces the amount of custom UI work required.

*These are only recommended because Jeff is familiar with them... PrimeNG is used at his company. The primary reason for using a component library is to save you time.*

**Other component library options...**

| Library | Framework | Notes |
|---|---|---|
| [**NG-ZORRO**](https://ng.ant.design) | Angular | Ant Design for Angular. Large component set, strong enterprise adoption. |
| [**Clarity**](https://clarity.design) | Angular | VMware's design system. Enterprise-focused, battle-tested at scale. |
| [**Taiga UI**](https://taiga-ui.dev) | Angular | Modern, well-designed. Strong accessibility support. |
| [**Ant Design**](https://ant.design) | React | One of the most widely used React component libraries globally. Massive component catalog. |
| [**Chakra UI**](https://chakra-ui.com) | React | Excellent accessibility, clean API, strong community. |
| [**Mantine**](https://mantine.dev) | React | Modern, feature-rich, includes hooks library. |
| [**shadcn/ui**](https://ui.shadcn.com) | React | Copy-into-project components built on Radix primitives. Highly customizable. |

#### 2.1.3 Back End

The API will be implemented using one of the following frameworks (student's choice):

- [**C# / .NET**](https://dotnet.microsoft.com/en-us/download) (latest LTS, Web API)
- [**Java / Spring Boot**](https://spring.io/projects/spring-boot/) (latest stable, JDK 21+ LTS)

**Java-specific requirements:**

- **Maven** must be used as the build system.
- Use [**Spring Initializr**](https://start.spring.io) to bootstrap the project with the correct dependencies.

#### 2.1.4 API Tooling

| Tool | Purpose |
|---|---|
| [**Swagger / OpenAPI**](https://swagger.io/specification/) | API documentation and interactive exploration ([Swashbuckle for .NET](https://github.com/domaindrivendev/Swashbuckle.AspNetCore), [SpringDoc for Java](https://springdoc.org)) |
| [**Bruno**](https://www.usebruno.com) | API endpoint testing (collections committed to the repository) |

### 2.2 Persistence

#### 2.2.1 Storage Medium

Device configuration and state must be persisted using one of the following (student's choice):

| Option | Detail |
|---|---|
| **JSON file** | A structured JSON file read at startup and written on state change |
| **SQLite database** | A local SQLite database accessed through an ORM (extra credit -- see Section 7.1) |

#### 2.2.2 State Persistence

- The full device configuration (metadata + current state + attributes) is loaded on application startup.
- All state changes are written back to the persistence medium so they **survive an application restart**.
- State machines must support **dehydration** (serializing current state to the persistence medium) and **rehydration** (restoring a state machine to a previously persisted state on startup).

### 2.3 State Machines

#### 2.3.1 Formal State Machine Implementation

- Each device type must implement a formal state machine with explicitly defined states and transitions.
- Invalid transitions must be rejected (not silently ignored).
- The state machine infrastructure should be **generic and reusable** -- adding a new device type should require defining its states and transitions, not modifying the state machine engine.

#### 2.3.2 Dehydration and Rehydration

- State machines must be serializable to and deserializable from the chosen persistence medium.
- On startup, each device's state machine is restored to its last known state, including all attribute values (brightness, speed, temperature, etc.).

### 2.4 API Design

#### 2.4.1 RESTful Endpoints

The back-end API must expose RESTful endpoints for:

| Endpoint Category | Examples |
|---|---|
| **List devices** | `GET /api/devices` with optional query filters (location, type, state) |
| **Get device** | `GET /api/devices/{id}` |
| **Register device** | `POST /api/devices` with type, name, and location in the request body |
| **Remove device** | `DELETE /api/devices/{id}` |
| **Control device** | `PUT /api/devices/{id}/state` or `POST /api/devices/{id}/commands` |
| **Set ambient temperature** | `PUT /api/locations/{location}/ambient-temperature` with temperature in the request body |
| **Device metadata** | As needed for filtering, grouping, and display |

Specific endpoint design is left to the student, but must follow REST conventions and return appropriate HTTP status codes.

#### 2.4.2 API Documentation

The API must be documented using [**Swagger / OpenAPI**](https://swagger.io/specification/):

- **.NET:** [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) or [NSwag](https://github.com/RicoSuter/NSwag)
- **Java:** [SpringDoc OpenAPI](https://springdoc.org)

The Swagger UI must be accessible when the application is running (e.g., at `/swagger`). All endpoints, request/response schemas, and error responses must be documented. The API specification serves as a formal contract between the front end and back end.

#### 2.4.3 API Testing with Bruno

The project must include a [**Bruno**](https://www.usebruno.com) collection that exercises all API endpoints. Bruno is an open-source, git-friendly API client that stores collections as plain files.

- The Bruno collection must be committed to the repository (e.g., in a `/bruno` or `/api-tests` directory).
- The collection must include requests for every endpoint: listing, getting, registering, removing, and controlling devices, as well as retrieving command history.
- Requests should include example request bodies and demonstrate both success and error cases.
- The collection serves as both a testing tool during development and a deliverable for grading -- the instructor will use it to exercise the API.

#### 2.4.4 Device Command History

The API must maintain an **audit log** of operations performed on each device. Each log entry records:

| Field | Description |
|---|---|
| Timestamp | When the operation occurred |
| Device ID | Which device was affected |
| Operation | What was done (e.g., "power on", "set brightness to 80", "lock") |

- The audit log is persisted alongside device state and survives application restart.
- The API exposes an endpoint to retrieve the command history for a device (e.g., `GET /api/devices/{id}/history`).
- The UI displays a recent activity feed or per-device history view.

### 2.5 Repository Structure

The repository must follow a clean, organized structure consistent with industry conventions. The front end and back end are **sibling directories** at the repository root -- do not nest one inside the other. This keeps build tooling, CI pipelines, and Dockerfiles cleanly separated.

All repositories must include:

| File/Directory | Purpose |
|---|---|
| `README.md` | Project introduction, setup instructions, Loom video links, design pattern catalog |
| `.gitignore` | Language-appropriate ignores for build artifacts, dependencies, IDE files, and environment secrets |
| `docker-compose.yml` | Orchestrates all services (front end, back end, database, etc.) |
| `frontend/` | The SPA application (Angular or React) |
| `bruno/` | Bruno API test collection |

#### 2.5.1 C# / .NET Repository

```
├── README.md
├── .gitignore
├── docker-compose.yml
├── SmartHome.sln
├── src/
│   ├── SmartHome.Api/                # Web API project (controllers, middleware)
│   │   ├── SmartHome.Api.csproj
│   │   ├── Dockerfile
│   │   ├── Controllers/
│   │   └── Program.cs
│   ├── SmartHome.Domain/             # Domain models, state machines, interfaces
│   │   └── SmartHome.Domain.csproj
│   └── SmartHome.Infrastructure/     # Persistence, external services
│       └── SmartHome.Infrastructure.csproj
├── tests/
│   ├── SmartHome.Api.Tests/
│   │   └── SmartHome.Api.Tests.csproj
│   └── SmartHome.Domain.Tests/
│       └── SmartHome.Domain.Tests.csproj
├── frontend/
│   ├── Dockerfile
│   ├── package.json
│   ├── package-lock.json
│   └── src/
└── bruno/
```

- The `.sln` file lives at the repository root and references all `.csproj` files.
- Source projects go in `src/` with separate projects for API, Domain, and Infrastructure.
- Test projects go in `tests/` mirroring `src/` with a `.Tests` suffix.
- `.gitignore` must exclude: `bin/`, `obj/`, `*.user`, `.vs/`, `node_modules/`, `dist/`, `.angular/cache/`.
- Commit `package-lock.json` (or `pnpm-lock.yaml`).

#### 2.5.2 Java / Spring Boot Repository

```
├── README.md
├── .gitignore
├── docker-compose.yml
├── pom.xml                           # Parent POM (aggregator)
├── backend/
│   ├── pom.xml                       # Spring Boot module
│   ├── Dockerfile
│   └── src/
│       ├── main/
│       │   ├── java/
│       │   │   └── com/example/smarthome/
│       │   │       ├── controller/
│       │   │       ├── service/
│       │   │       ├── domain/
│       │   │       ├── repository/
│       │   │       └── SmartHomeApplication.java
│       │   └── resources/
│       │       └── application.yml
│       └── test/
│           └── java/
│               └── com/example/smarthome/
├── frontend/
│   ├── Dockerfile
│   ├── package.json
│   ├── package-lock.json
│   └── src/
└── bruno/
```

- The parent `pom.xml` at the repository root uses `<packaging>pom</packaging>` and declares `backend/` (and optionally `frontend/`) as modules.
- Source code follows the Maven standard directory layout: `src/main/java/` and `src/test/java/`.
- Tests live alongside source in `backend/src/test/java/` mirroring the main package structure.
- `.gitignore` must exclude: `target/`, `*.class`, `.idea/`, `*.iml`, `.settings/`, `.project`, `.classpath`, `node_modules/`, `dist/`.
- Commit the Maven wrapper (`mvnw`, `.mvn/`) and `package-lock.json`.

#### 2.5.3 Front-End Directory (Angular or React)

The `frontend/` directory is a standalone SPA project, independent of the back-end build system.

**Angular:**
```
frontend/
├── Dockerfile
├── package.json
├── package-lock.json
├── angular.json
├── tsconfig.json
├── src/
│   ├── app/
│   │   ├── components/
│   │   ├── services/
│   │   ├── models/
│   │   └── app.module.ts (or app.config.ts for standalone)
│   ├── assets/
│   ├── environments/
│   └── index.html
└── karma.conf.js (or jest.config.ts)
```

**React:**
```
frontend/
├── Dockerfile
├── package.json
├── package-lock.json
├── vite.config.ts (or next.config.js)
├── tsconfig.json
├── src/
│   ├── components/
│   ├── services/
│   ├── models/
│   ├── App.tsx
│   └── main.tsx
├── public/
└── vitest.config.ts (or jest.config.ts)
```

For both frameworks: `package.json` lives at the root of `frontend/`. All front-end dependencies, scripts, and configuration are self-contained within this directory.

## 3. User Interface Requirements

### 3.1 Device Dashboard

- The UI displays all devices grouped by **location**.
- Within each location group, devices are listed by **name**.
- Each device displays its current state and relevant attributes.
- Devices considered "on" (see Section 1.1, "On" condition per device type) are visually distinguished.

### 3.2 Filtering

The dashboard supports the following filters (combinable):

| Filter | Behavior |
|---|---|
| **On** | Show only devices in an "on" state (includes all latch devices, powered devices that are active) |
| **Off** | Show only powered devices that are currently off (latch devices are never "off") |
| **Location** | Show only devices in a selected location |
| **Device Type** | Show only devices of a selected type |

### 3.3 Device Controls

- Each device on the dashboard exposes inline controls appropriate to its type.
- Controls reflect the current state and update in real time (or near-real-time) upon state changes.
- Invalid operations (e.g., dimming a light that is off) are prevented by the UI.

### 3.4 Responsive / Mobile-Friendly Design

The UI must be **mobile-friendly** and usable on both desktop and mobile screen sizes. A smart home controller is the type of application a user is likely to operate from their phone.

- The layout must adapt to small screens using responsive design (CSS media queries, flexbox/grid, or the component library's responsive utilities).
- Device cards, controls, filters, and navigation must remain usable on a typical mobile viewport (375px width and up).
- This is not a request for a separate mobile app -- the same SPA must work well at all screen sizes.

### 3.5 Device Management

- The UI provides the ability to **register a new device** by specifying its type, name, and location. The new device is initialized in its default state (e.g., Off for powered devices, Unlocked for a door lock).
- The UI provides the ability to **remove an existing device** with a confirmation prompt. Removal deletes the device and its persisted state.

## 4. Design Requirements

### 4.1 Separation of Concerns

The back end must properly separate responsibilities across layers:

- **Controller / HTTP Layer** -- Handles request/response mapping, input validation, HTTP status codes, and routing. Controllers must remain thin and delegate all business logic to the service layer.
- **Service / Domain Layer** -- Contains all business logic, state machine orchestration, and domain rules. Services operate on domain models and are independent of HTTP concerns.

### 4.2 SOLID Principles

All classes, namespaces, packages, and modules must demonstrate adherence to SOLID principles:

| Principle | Requirement |
|---|---|
| **Single Responsibility (SRP)** | Each class has one reason to change. Controllers do not contain business logic. Services do not contain persistence logic. |
| **Open-Closed (OCP)** | New device types can be added without modifying existing device infrastructure. State machines are extensible by design. |
| **Liskov Substitution (LSP)** | All device subtypes are safely substitutable for their base type or interface. No subclass violates the contract of its parent. |
| **Interface Segregation (ISP)** | Clients depend only on the interfaces they use. Device capabilities are modeled through focused, cohesive interfaces (e.g., a door lock does not implement a dimming interface). |
| **Dependency Inversion (DIP)** | High-level modules depend on abstractions, not concretions. All service and repository dependencies are injected via the framework's DI container. |

### 4.3 Dependency Injection

- All dependencies must be registered in and resolved from the framework's built-in DI container (.NET `IServiceCollection` or Spring `ApplicationContext`).
- **Anti-patterns such as Service Locator are prohibited.** Classes must not resolve their own dependencies from the container.

### 4.4 Abstraction Quality

- All interfaces and abstract classes must include clear, meaningful documentation comments explaining their purpose and contract.
- Abstractions must follow Ousterhout's **deep module** concept: present a simple, narrow interface to consumers while encapsulating significant complexity internally. Complexity is pushed down into implementations, not leaked upward to callers.

### 4.5 Design Pattern Identification

Students must consciously identify and apply recognized design patterns where the project naturally demands them. A **Design Patterns** section must be included in the project documentation that maps each pattern used to its location in the codebase, with a brief rationale for why it was chosen.

#### Required Patterns (all teams)

These patterns are inherent to the core requirements. Every team must implement and document them.

| Pattern | Application | Why It's Required |
|---|---|---|
| **State** | Device state machines | Each device type has formally defined states and transitions. The State pattern encapsulates state-specific behavior and makes transitions explicit. |
| **Factory** | Device creation by type | The system must create different device types (Light, Fan, Thermostat, DoorLock) from a registration request. A Factory centralizes creation logic and supports OCP -- adding a new device type should not require modifying existing creation code. |
| **Strategy** | Thermostat heating/cooling/auto mode behavior | The thermostat's three modes (Heat, Cool, Auto) each define different rules for when to start heating or cooling. The Strategy pattern allows the mode to be selected at runtime without conditional branching in the thermostat logic. |

#### Required Patterns (conditional on extra credit)

These patterns become required when the corresponding extra credit feature is implemented.

| Pattern | Application | Required When |
|---|---|---|
| **Observer** | State change notification -- the back end notifies all connected UI clients when device state changes | SSE extra credit (Section 7.2) |
| **Command** | Device operations encapsulated as objects -- enables audit logging, undo potential, and scene composition | Scenes extra credit (Section 7.5) |
| **Composite** | A scene composes multiple command objects into a single executable unit that can span devices and locations | Scenes extra credit (Section 7.5) |

#### Additional Patterns (encouraged)

Students are not limited to the patterns above. The following are examples of patterns that may arise naturally depending on implementation choices. Identify and document any additional patterns wherever they are applied.

| Pattern | Potential Application |
|---|---|
| **Repository** | Abstracting persistence behind an interface so the service layer is independent of the storage medium |
| **Decorator** | Adding cross-cutting behavior (logging, validation, caching) to services without modifying them |
| **Singleton** | Managing shared state such as the SSE event broadcaster or thermostat simulation timer |
| **Template Method** | Defining a common device lifecycle (initialize, validate transition, apply state, persist) with device-specific steps overridden by subclasses |
| **Adapter** | Wrapping an external API (e.g., an LLM provider or identity provider) behind an internal interface |

The goal is to demonstrate that patterns are chosen deliberately to solve specific design problems, not applied superficially. Each pattern documented must include: the pattern name, where it is implemented (class/file references), and a brief rationale explaining why this pattern was the right choice for the problem.

### 4.6 Validation Strategy

**Never trust data from the browser.** Even though this project does not implement authentication or security controls, all data arriving at the API must be validated server-side. The UI may provide client-side validation for user experience, but client-side validation is **not a substitute** for server-side validation -- a browser's requests can always be forged or tampered with.

Input validation must be handled consistently and at the correct architectural layer:

- **Controller layer** -- Validates HTTP-level concerns: required fields present, correct data types, well-formed requests. Returns `400 Bad Request` for malformed input.
- **Service/domain layer** -- Validates business rules: brightness within [10, 100], temperature within [60, 80], valid state transitions. Returns domain-specific errors.
- Validation logic must **not** be scattered or duplicated across layers. Each validation concern belongs to exactly one layer.

**Recommended validation libraries:**

| Framework | Library | Notes |
|---|---|---|
| .NET | [**FluentValidation**](https://docs.fluentvalidation.net) | Fluent API for building strongly-typed validation rules. Separates validation logic from models. |
| Spring Boot | [**Jakarta Bean Validation (Hibernate Validator)**](https://hibernate.org/validator/) | Annotation-based validation (`@NotNull`, `@Min`, `@Max`, etc.) built into Spring Boot. |

Using a validation library keeps validation rules declarative, testable, and consistent. Avoid hand-written `if/else` validation scattered throughout controllers or services.

### 4.7 Error Handling Contract

The API must return errors in a **consistent, structured format** across all endpoints. The recommended format is [RFC 7807](https://datatracker.ietf.org/doc/html/rfc7807) Problem Details (`application/problem+json`) -- see [this accessible overview](https://swagger.io/blog/problem-details-rfc9457-doing-api-errors-well/) for a practical introduction. Any consistent schema is acceptable provided it includes at minimum:

- A machine-readable error type or code
- A human-readable message
- The HTTP status code

Error handling must be implemented as a cross-cutting concern (e.g., exception middleware or global error handler), not as ad-hoc try/catch blocks in individual controllers.

**Never leak implementation details to the client.** Raw exceptions, stack traces, internal class names, database error messages, and connection strings must **never** appear in API responses. Leaked internals provide attackers with a roadmap of the system -- table names, column names, framework versions, file paths, and query structure all help an attacker craft targeted exploits. Even in an unauthenticated application like this one, building the habit of guarding internal details is essential.

**Example -- what NOT to return:**

```json
{
  "error": "Microsoft.Data.Sqlite.SqliteException: SQLite Error 19: 'UNIQUE constraint failed: Devices.Id'. Query: INSERT INTO Devices (Id, Name, Location, Type, State) VALUES (@p0, @p1, @p2, @p3, @p4) at SmartHome.Repositories.DeviceRepository.Add(Device device) in /src/Repositories/DeviceRepository.cs:line 47"
}
```

This single error message reveals the database engine (SQLite), table and column names (`Devices.Id`, `Name`, `Location`, `Type`, `State`), the ORM parameterization style, the internal namespace and class structure (`SmartHome.Repositories.DeviceRepository`), and the exact file path and line number. An attacker now has a detailed picture of the system's internals.

**Instead, return:**

```json
{
  "type": "https://example.com/problems/duplicate-device",
  "title": "Duplicate device",
  "status": 409,
  "detail": "A device with this ID already exists."
}
```

The full exception is logged server-side where administrators can review it. The client receives only what it needs to understand and respond to the problem.

- Error responses sent to the client must contain a **user-friendly message** that describes the problem in terms the consumer can act on.
- Internal details (exception type, stack trace, SQL errors, etc.) must be **logged server-side** for administrative review and debugging -- not returned in the response body.
- In development mode, frameworks may display detailed errors by default. Ensure the production/Docker configuration suppresses these details.

## 5. Development Requirements

### 5.1 Testing

All integration and unit tests -- both back end and front end -- **may be generated by AI**. The use of AI tools for test generation is explicitly permitted and encouraged.

#### 5.1.1 Unit Tests

- **State machine transitions** -- Every valid transition is tested. Every invalid transition is tested and confirmed to be rejected.
- **Boundary conditions** -- Light brightness (10, 100, and out-of-range values), thermostat temperature (60, 80, and out-of-range values), fan speed values.
- **Service/domain logic** -- Business rules are tested independently of HTTP and persistence concerns.
- **Device creation and removal** -- Factory or creation logic produces correctly initialized devices.
- **Invariants** -- Registering a second thermostat in a location that already has one is rejected.

#### 5.1.2 Integration Tests

- **API contract** -- Each endpoint returns correct status codes and response shapes for both success and error cases.
- **Persistence round-trip** -- A device's state survives dehydration and rehydration (write, restart, read back).
- **Thermostat simulation** -- The temperature changes over time toward the desired temperature and the thermostat transitions to Idle when the target is reached.

#### 5.1.3 Front-End Tests

- **Component rendering** -- Device controls render correctly for each device type and state.
- **Filter behavior** -- Filtering by on/off, location, and device type shows the correct subset of devices.
- **Invalid input prevention** -- The UI prevents submission of out-of-range or invalid values.

## 6. Delivery Requirements

### 6.1 Setup and Run Instructions

The goal is **clone-and-go**: a reviewer should be able to clone the repository and have the application running with minimal effort. This is critical -- if the reviewer cannot get the application running quickly, it cannot be graded.

The repository must include clear, step-by-step instructions (in the README or a dedicated `SETUP.md`) covering:

- **Prerequisites** -- Only Docker and Docker Compose should be required. If any other tooling is needed (e.g., an API key for the LLM extra credit), it must be explicitly documented.
- **How to build and run from the CLI** -- The exact commands needed to start the application. Ideally a single `docker compose up` command.
- **How to access the application** -- The URL(s) for the UI, API, and Swagger documentation once running.
- **How to run tests** -- The exact commands to execute the back-end and front-end test suites.
- **How to use the Bruno collection** -- Where the collection is located and how to open it.
- **Test credentials** -- If JWT authentication is implemented, pre-configured test user credentials must be documented.

### 6.2 Docker

The entire application must be runnable locally using **Docker**.

- The project must include a `docker-compose.yml` (or equivalent) that builds and starts all required services (front end, back end, and any database or identity provider if applicable).
- A reviewer must be able to clone the repository and run the application with a single `docker compose up` command.
- The Docker configuration must not require the reviewer to install framework-specific tooling (e.g., .NET SDK, JDK, Node.js) on their host machine -- all build and runtime dependencies are handled inside containers.
- On first run, the application must be **fully seeded and ready to use** -- no manual migration commands, no manual data imports.

### 6.3 Local Development (Optional Documentation)

Students are encouraged (but not required) to also document how to run the front end and back end outside of Docker for local development (e.g., `dotnet run`, `mvn spring-boot:run`, `npm start`). This helps teammates who prefer to develop without rebuilding containers on every change.

### 6.4 Video Deliverables

The team must produce **two Loom videos** and include the links in the project README. Create a free account at [loom.com](https://www.loom.com).

#### 6.4.1 Application Demo Video

A walkthrough of the **working application** demonstrating:

- The device dashboard with devices grouped by location
- Filtering by on/off, location, and device type
- Controlling each device type (light, fan, thermostat, door lock)
- Registering a new device and removing an existing device
- The thermostat environment simulation (setting ambient temperature and watching the thermostat react)
- The device command history / activity feed
- Any extra credit features the team implemented

#### 6.4.2 Architecture Tour Video

A tour of the **codebase and architecture** covering:

- Project structure and layer organization (controllers, services, repositories/persistence)
- How SOLID principles are applied (with specific examples from the code)
- The state machine implementation and how new device types can be added
- Design patterns used and where they appear
- How dehydration/rehydration works
- How validation and error handling are structured
- Any architectural decisions the team is particularly proud of

Each video should be **5--10 minutes**. These videos serve as both a grading aid and practice for the kind of technical presentations expected in industry.

### 6.5 Team Policy

#### 6.5.1 Team Size

- The base team size is **1 or 2 developers**.
- A team may add additional developers **only if** the team also commits to implementing an extra credit feature for each additional member.
- The **maximum team size is 6**.

**A warning about large teams:** As team size increases, coordination overhead grows dramatically. The number of communication channels in a team follows the formula **N × (N - 1) / 2** -- a team of 2 has 1 channel, a team of 4 has 6, and a team of 6 has 15. Larger teams spend proportionally more time coordinating and less time building. Choose your team size carefully -- a focused team of 3 will almost always outperform a disorganized team of 6.

| Team Size | Communication Channels |
|---|---|
| 2 | 1 |
| 3 | 3 |
| 4 | 6 |
| 5 | 10 |
| 6 | 15 |

#### 6.5.2 Extra Credit and Team Size Trade-Off

When an extra credit feature is adopted to justify a larger team, that feature becomes a **mandatory requirement** for the team -- it is no longer extra credit.

| Team Size | Requirements |
|---|---|
| 1--2 | All core requirements (Sections 1--6) |
| 3 | All core requirements + 1 extra credit feature becomes mandatory |
| 4 | All core requirements + 2 extra credit features become mandatory |
| 5 | All core requirements + 3 extra credit features become mandatory |
| 6 | All core requirements + 4 extra credit features become mandatory |

Available extra credit features: **ORM** (Section 7.1), **SSE** (Section 7.2), **LLM/MCP** (Section 7.3), **JWT Authentication** (Section 7.4), **Scenes** (Section 7.5), **CI/CD** (Section 7.6).

A team of 6 adopts 4 features as mandatory, leaving 2 still available for extra credit.

#### 6.5.3 Examples

- A team of 2 implements all core requirements. All six extra credit features are optional.
- A team of 3 chooses to implement Scenes. Scenes is now mandatory for this team and no longer earns extra credit. The remaining five features are available for extra credit.
- A team of 4 must adopt 2 extra credit features as mandatory (e.g., ORM and SSE). The remaining four features are still available for extra credit.
- A team of 5 must adopt 3 extra credit features as mandatory. The remaining three features are still available for extra credit.
- A team of 6 must adopt 4 extra credit features as mandatory. The remaining two features are still available for extra credit.

#### 6.5.4 Dysfunctional Teams and Member Removal

If a team is not functioning effectively, team members may be **removed ("fired")** from the team under the following rules:

- Any team member may be removed by a majority decision of the remaining team members, or by instructor intervention.
- **Removal is not permitted during the final two weeks of the semester.** Teams must resolve conflicts or seek instructor mediation before that deadline.
- A removed member must either:
  - **Work independently** (team of 1), or
  - **Join with other removed members** to form a new team.
- The original team's scope requirements are **not reduced** after a member is removed. If the team adopted extra credit features to justify their original size, those features remain mandatory. Learning to effectively work in teams (including picking outstanding teammates) is a critical skill.
- The removed member's new team (solo or reformed) is held to the standard requirements for their new team size.

### 6.6 Grading Criteria

| Category | Weight | Key Evaluation Points |
|---|---|---|
| **OO Design & Implementation** | 40% | SOLID principles, design pattern identification and application, separation of concerns (controllers vs. services), dependency injection, state machine correctness and reusability, validation strategy, error handling contract, API design (REST conventions, Swagger/OpenAPI, Bruno collection), code quality (comments on abstractions, naming, deep modules) |
| **UI** | 20% | Component library usage, device dashboard (grouping, filtering, real-time state display), device controls and management, responsive/mobile-friendly design, input validation, all device types functional with correct behavior |
| **Documentation** | 20% | README with introduction and setup/deployment guide, design pattern catalog, Loom videos (application demo and architecture tour), Docker Compose clone-and-go experience |
| **Persistence** | 10% | Survives restart, correct serialization, state machine dehydration/rehydration, command history persistence |
| **Testing** | 10% | Unit tests (state machines, services, boundary conditions, invariants), integration tests (API contract, persistence round-trip, thermostat simulation), front-end tests. AI-generated tests are permitted. |
| **Extra Credit: ORM** | +5% | ORM usage (EF Core or Hibernate) |
| **Extra Credit: SSE** | +5% | Real-time push via Server-Sent Events enabling multi-client state synchronization |
| **Extra Credit: LLM/MCP** | +5% | Natural language device control via MCP server and a real LLM |
| **Extra Credit: JWT Auth** | +5% | JWT authentication with an external identity provider |
| **Extra Credit: Scenes** | +5% | Device scenes using Composite and Command patterns, CRUD, cross-location execution |
| **Extra Credit: CI/CD** | +5% | Automated pipeline (lint, build, test, deploy) with cloud deployment |

Extra credit features adopted to satisfy the team size policy (Section 6.5.2) are graded as mandatory requirements and do not earn bonus points.

## 7. Extra Credit

Each extra credit feature is worth **+5%**. Teams that adopt extra credit features to justify a larger team size (see Section 6.5.2) must implement those features as mandatory requirements -- they no longer earn bonus points.

### 7.1 ORM

Using an ORM instead of a JSON file for persistence earns extra credit:

- **.NET:** Entity Framework Core
- **Java:** Hibernate / Spring Data JPA

If an ORM is used, the database migration that seeds the initial device configuration **must run automatically** on application startup. The reviewer should not need to manually run migration commands -- `docker compose up` must produce a fully seeded, ready-to-use application.

### 7.2 Server-Sent Events (SSE)

The API may expose an SSE endpoint (e.g., `GET /api/devices/events`) that pushes device state change events to all connected UI clients in real time. This enables multiple users viewing the dashboard simultaneously to see the same device state without manual refresh.

- When any device state changes (user action, thermostat simulation tick, etc.), the server emits an event to all connected SSE clients.
- The UI subscribes to the SSE stream on load and updates the dashboard reactively as events arrive.
- Events should include enough information for the client to update its local state (e.g., device ID, new state, changed attributes).
- The SSE connection should handle reconnection gracefully (the `EventSource` API provides this by default).

### 7.3 LLM Natural Language Control via MCP

The application may implement an MCP (Model Context Protocol) server that exposes smart home device operations as tools, along with a prompt UI in the front end that allows the user to control devices using natural language.

**Requirements:**

- The back end implements an **MCP server** that exposes device operations (e.g., turn on/off, set brightness, lock/unlock, set temperature) as MCP tools.
- The UI provides a **chat/prompt input** where the user can type natural language commands (e.g., "turn on all of the lights", "set the thermostat to 72", "lock the front door").
- An LLM interprets the user's intent and invokes the appropriate MCP tools to fulfill the request.
- Results and confirmations are displayed back to the user in the chat interface.

**LLM Policy:**

- The LLM **must be a real, commercially available model** -- simulation or hard-coded responses are not permitted.
- Acceptable LLM providers include:
  - **OpenAI** (GPT-4, GPT-4o, etc.)
  - **Anthropic** (Claude)
  - **Self-hosted open-source model** (e.g., LLaMA running in a container on a cloud provider such as DigitalOcean)
- The team is **responsible for any costs incurred**, including providing a working API key to the instructor for testing and grading.

### 7.4 JWT Authentication

The application may implement JWT-based authentication using an external identity provider. API endpoints are protected and require a valid JWT bearer token. The UI handles the login flow and passes the token with each request.

**Requirements:**

- The back end validates JWT tokens issued by the identity provider and rejects unauthenticated requests with `401 Unauthorized`.
- The UI redirects unauthenticated users to the identity provider's login page and handles the token lifecycle (acquisition, storage, refresh, logout).
- At least one test user account must be pre-configured for the reviewer to log in. Credentials must be provided to the instructor.
- User registration (self-registration or registration by an existing user) is **not required**. Pre-configured test accounts are sufficient.

**Acceptable identity providers** (open-source or free-tier recommended):

| Provider | Notes |
|---|---|
| [**Keycloak**](https://www.keycloak.org) | Open source, self-hosted. Recommended -- the instructor's company uses Keycloak with good success. Can run as a Docker container alongside the application. |
| [**Auth0**](https://auth0.com) | Free tier available. Cloud-hosted. |
| [**Microsoft Entra ID**](https://www.microsoft.com/en-us/security/business/identity-access/microsoft-entra-id) | Free tier available for development. |

If the identity provider can be containerized (e.g., Keycloak), it should be included in the `docker-compose.yml` so the reviewer can run the full stack with authentication without creating external accounts.

### 7.5 Device Scenes

The application may implement **scenes** -- named presets that execute a batch of device operations in a single action. Scenes allow the user to control multiple devices across multiple locations at once.

**Examples:**

| Scene | Actions |
|---|---|
| "Good Night" | Lock all doors, turn off all lights, set thermostat to 68°F |
| "Movie Night" | Dim living room lights to 20%, turn off kitchen lights, lock front door |
| "Welcome Home" | Unlock front door, turn on hallway and kitchen lights to 100% |
| "All Lights Off" | Turn off every light in the house regardless of location |

**Requirements:**

- A scene has a **name** and an ordered list of **actions**. Each action targets a specific device (by ID) or a device group (by type, location, or both) and specifies the desired operation (e.g., "turn off", "set brightness to 50", "lock").
- Scenes can **span locations** -- a single scene can target devices across the entire house.
- Actions that target a group (e.g., "all lights") are resolved at execution time, so newly added devices are automatically included.
- The user can **create, edit, delete, and execute** scenes through the UI.
- Scenes are **persisted** and survive application restart.
- The API exposes endpoints for scene CRUD and execution (e.g., `GET /api/scenes`, `POST /api/scenes`, `POST /api/scenes/{id}/execute`).
- When a scene is executed, each action is applied in order. If an individual action fails (e.g., a device is already in the target state), execution continues -- the scene does not abort on partial failure. The response reports the outcome of each action.
- Scene execution is recorded in the device command history (Section 2.4.4) for each affected device.

**Design requirement:** Scenes **must** be implemented using the **Composite** and **Command** patterns. A scene is a composite command containing an ordered list of device commands. Each device operation is encapsulated as a command object, and the scene composes them into a single executable unit. This is not optional -- the design pattern usage is part of the grading criteria for this feature. Students must document this in their design pattern identification (Section 4.5).

### 7.6 CI/CD Pipeline

The project may implement a **CI/CD pipeline** using a platform such as [**GitHub Actions**](https://docs.github.com/en/actions), [**GitLab CI**](https://docs.gitlab.com/ee/ci/), or a similar service.

**Pipeline requirements (minimum):**

- **On every push / pull request:**
  - Lint / static analysis
  - Build the back end and front end
  - Run unit tests
  - Run integration tests
- **On merge to main:**
  - Build Docker images
  - Deploy to a cloud hosting target

**Deployment target:**

The team must deploy the application to a cloud provider. The team is **responsible for any costs incurred**. Low-cost and free-tier providers are recommended:

| Provider | Notes |
|---|---|
| [**Oracle Cloud Free Tier**](https://www.oracle.com/cloud/free/) | Always-free ARM instances (up to 4 OCPUs, 24 GB RAM). Generous for a student project. |
| [**Fly.io**](https://fly.io) | Free tier includes small VMs. Simple Docker-based deployment. |
| [**Render**](https://render.com) | Free tier for web services. Docker support. |
| [**Railway**](https://railway.app) | Free trial credits. Simple GitHub integration. |
| [**DigitalOcean**](https://www.digitalocean.com) | $200 free credit for students via GitHub Education. |
| [**AWS**](https://aws.amazon.com/free/) | Free tier (12 months). More complex but industry-standard. |

The deployed URL must be documented in the project README so the reviewer can access the live application.

**Video requirement:** If the CI/CD extra credit is implemented, the team must produce an additional **5-minute Loom video** demonstrating the pipeline. The video should show the pipeline configuration, walk through a code check-in triggering the pipeline, and show the build, test, and deployment stages completing successfully. Include the Loom link in the project README alongside the other video deliverables.

## 8. Glossary

| Term | Definition |
|---|---|
| **Ambient Temperature** | The simulated current temperature as tracked by a thermostat |
| **Audit Log** | A persistent record of operations performed on devices, capturing what was done, to which device, and when |
| **Bruno** | An open-source, git-friendly API client for testing HTTP endpoints. Collections are stored as plain files and committed to the repository |
| **CI/CD** | Continuous Integration / Continuous Deployment -- an automated pipeline that builds, tests, and deploys code on every push |
| **Command Pattern** | A behavioral design pattern that encapsulates a request as an object, allowing parameterization, queuing, and logging of operations |
| **Composite Pattern** | A structural design pattern that composes objects into tree structures so individual objects and compositions can be treated uniformly |
| **Dehydration** | Serializing a state machine's current state and context to a persistence medium |
| **Deep Module** | An abstraction that provides a simple interface while hiding significant internal complexity (Ousterhout) |
| **DI Container** | The framework-provided dependency injection container used to register and resolve service dependencies |
| **JWT (JSON Web Token)** | A compact, URL-safe token format used for transmitting authentication and authorization claims between parties |
| **MCP (Model Context Protocol)** | A protocol that allows LLMs to discover and invoke tools exposed by an MCP server, enabling natural language interaction with external systems |
| **[RFC 7807](https://datatracker.ietf.org/doc/html/rfc7807) Problem Details** | A standard format for representing machine-readable error responses in HTTP APIs (`application/problem+json`) |
| **Rehydration** | Restoring a state machine to a previously persisted state from a persistence medium |
| **Scene** | A named preset that executes a batch of device operations (commands) in a single action, potentially spanning multiple locations |
| **Server-Sent Events (SSE)** | A standard HTTP mechanism where the server pushes events to clients over a long-lived connection. Unlike WebSockets, SSE is unidirectional (server to client) and uses standard HTTP |
| **Service Locator** | An anti-pattern where classes resolve their own dependencies from a global registry rather than receiving them via injection |
| **State Machine** | A model of computation with a finite set of states, transitions between those states triggered by events, and actions associated with transitions |
| **Swagger / OpenAPI** | A specification and toolset for describing, documenting, and testing RESTful APIs |
