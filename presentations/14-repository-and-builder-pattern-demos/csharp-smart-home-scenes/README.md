# C# Smart Home Device Scenes Demo

This demo is the C# companion for **Lecture 14: Repository and Builder Patterns**.

It demonstrates two separate design responsibilities in the same use case:

- `Builder` constructs a valid `DeviceScene`
- `Repository` persists and reloads that scene definition from SQLite

The domain matches the course project vocabulary:

- named scenes
- ordered scene actions
- specific-device targets
- group targets by device type and location

This demo intentionally stops at **scene definition construction and persistence**. It does **not** implement scene execution behavior, `Command`, or `Composite`.

## What the Demo Does

The program:

1. shows that the builder rejects an incomplete scene
2. builds an `Evening Arrival` scene
3. saves the scene through `ISceneRepository`
4. loads the scene back from SQLite
5. lists all persisted scenes

## Architecture

```mermaid
classDiagram
    class DeviceScene {
        +Id : Guid
        +Name : string
        +Actions : IReadOnlyList~SceneAction~
    }

    class SceneAction {
        +OrderIndex : int
        +Operation : string
        +Parameters : IReadOnlyDictionary~string,string~
    }

    class SceneTarget {
        <<abstract>>
    }

    class SpecificDeviceTarget {
        +DeviceId : Guid
    }

    class DeviceGroupTarget {
        +DeviceType : string
        +Location : string?
    }

    class DeviceSceneBuilder {
        +Named(name)
        +AddDeviceAction(...)
        +AddGroupAction(...)
        +Build()
    }

    class ISceneRepository {
        <<interface>>
        +Save(scene)
        +GetById(id)
        +ListAll()
    }

    class SqliteSceneRepository {
        +Save(scene)
        +GetById(id)
        +ListAll()
    }

    SceneTarget <|-- SpecificDeviceTarget
    SceneTarget <|-- DeviceGroupTarget
    DeviceScene --> SceneAction
    SceneAction --> SceneTarget
    DeviceSceneBuilder --> DeviceScene
    ISceneRepository <|.. SqliteSceneRepository
    SqliteSceneRepository --> DeviceScene
```

## Builder Flow

```mermaid
flowchart TB
    A[Start builder] --> B[Set scene name]
    B --> C[Add porch light action]
    C --> D[Add living room light group action]
    D --> E[Add brightness action]
    E --> F[Validate]
    F --> G[Build DeviceScene]
```

## Save / Load Sequence

```mermaid
sequenceDiagram
    participant Program
    participant Builder as DeviceSceneBuilder
    participant Repo as ISceneRepository
    participant Sqlite as SqliteSceneRepository
    participant DB as SQLite

    Program->>Builder: Build scene
    Builder-->>Program: DeviceScene
    Program->>Repo: Save(scene)
    Repo->>Sqlite: Save(scene)
    Sqlite->>DB: INSERT/UPDATE scenes + scene_actions
    Program->>Repo: GetById(scene.Id)
    Repo->>Sqlite: GetById(scene.Id)
    Sqlite->>DB: SELECT rows
    Sqlite-->>Program: DeviceScene
```

## SQLite Schema

```mermaid
erDiagram
    SCENES ||--o{ SCENE_ACTIONS : contains

    SCENES {
        text id PK
        text name
    }

    SCENE_ACTIONS {
        text id PK
        text scene_id FK
        integer order_index
        text target_kind
        text target_device_id
        text target_device_type
        text target_location
        text operation
        text parameters_json
    }
```

## How It Works

The builder is responsible for:

- requiring a scene name
- ensuring at least one action exists
- assigning action order when actions are added
- returning a finished `DeviceScene`

The repository is responsible for:

- creating the SQLite schema if needed
- storing the scene row and ordered action rows
- rehydrating a `DeviceScene` from the database

SQLite stays behind the repository boundary. The rest of the demo works with domain objects, not SQL rows.

## File Overview

- `Program.cs` program entrypoint and console output
- `Domain/` scene types and targets
- `Builder/` canonical builder interface plus fluent builder and director
- `Repository/` repository abstraction, SQLite implementation, and schema bootstrap
- `SmartHomeScenes.csproj` project file
- `Dockerfile` container build and run definition

## Run Locally

From this directory:

```bash
cd presentations/14-repository-and-builder-pattern-demos/csharp-smart-home-scenes
dotnet run
```

To store the database in a custom location:

```bash
cd presentations/14-repository-and-builder-pattern-demos/csharp-smart-home-scenes
SCENE_DB_PATH=./data/smart-home-scenes.db dotnet run
```

## Build and Run with Docker

From this directory:

```bash
cd presentations/14-repository-and-builder-pattern-demos/csharp-smart-home-scenes
docker build -t lecture14-csharp-scenes .
docker run --rm -v "$(pwd)/data:/data" lecture14-csharp-scenes
```

The mounted `data/` folder lets the SQLite database survive container recreation.

## Expected Output

```text
SMART HOME DEVICE SCENES DEMO (C#)
----------------------------------
1. Verifying that the builder rejects incomplete scenes...
   Expected validation error: Cannot build a DeviceScene without at least one action.

2. Building the Evening Arrival scene...
   Scene: Evening Arrival
   ...

4. Loading the scene back from SQLite...
   Scene: Evening Arrival
   ...
```
